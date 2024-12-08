using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolController : MonoBehaviour
{
    private bool canChange = true;
    private PillarObject pillar;
    [SerializeField] ToolGrid grid;
    [SerializeField] private ToolBase startTool;
    private int activeCell;
    public Action<ToolGrid.Cell> OnChange;
    public Action<ToolBase> OnAdd;
    public ToolGrid.Cell StartCell => grid[0];
    public ToolGrid.Cell Selected => grid[activeCell];
    public ToolGrid.Cell Prev => grid[(activeCell - 1 + grid.Count) % grid.Count];
    public ToolGrid.Cell Next => grid[(activeCell + 1 + grid.Count) % grid.Count];

    public float CellStep => grid.CellStep;
    public int CellsCount => grid.Count;
    public bool CanChange { get => canChange; set => canChange = value; }
    public ToolGrid Grid => grid;
    public int ActiveIndex => activeCell;
    public int PrevIndex => (activeCell - 1 + grid.Count) % grid.Count;
    public int NextIndex => (activeCell + 1 + grid.Count) % grid.Count;
    public ToolBase StartTool => startTool;
    private void OnEnable()
    {
        Execute();
    }
    private void OnDisable()
    {
        Stop();
    }
    private void Start()
    {
        if (pillar != null)
            grid.SetPillar(pillar);
    }
    public ulong GetFullIncome()
    {
        ulong income = 0;
        for (int i = 0; i < grid.Count; i++)
        {
            if (grid[i].hasTool)
            {
                income += grid[i].tool.Property.IncomePreSecond;
            }
        }
        return income;
    }
    public void SetPillar(PillarObject pillar)
    {
        this.pillar = pillar;
        startTool.Setup(this.pillar, ToolHolder.Default.tools[0].upgrade, -1, -1);
        //grid.SetPillar(pillar);
    }
    public void Initialize()
    {
        grid.Initialize();
        for (int i = 0; i < grid.Count; i++)
        {
            int c = ToolHolder.Default.GetCorrectIndex(i);
            SpawnGhost(grid[i], ToolHolder.Default.GetTool(c));
        }

    }
    public void Execute()
    {
        startTool.enabled = true;
        startTool.Execute();
        grid.Execute();
    }
    public void Stop()
    {
        startTool.enabled = false;
        grid.Stop();
    }

    public void Select(int currentCell)
    {
        if (canChange && activeCell != currentCell)
        {
            Selected.Deselect();
            activeCell = currentCell;
            Selected.Select();
            OnChange?.Invoke(Selected);
        }
    }

    public void Finish()
    {
        startTool.enabled = false;
        startTool.Finish();
        grid.Finish();
    }
    public ToolBase InstalToolInActiveCell(ToolHolder.Tool tool, int globalIndex)
    {
        ToolGrid.Cell cell = Selected;
        if (SpawnTool(cell, tool, activeCell, globalIndex))
        {
            cell.tool.Install();
            OnChange?.Invoke(cell);
            OnAdd?.Invoke(cell.tool);
            return cell.tool;
        }
        return null;
    }
    public bool SpawnTool(ToolGrid.Cell cell, ToolHolder.Tool tool, int cellId, int globalIndex)
    {
        if (!cell.hasTool)
        {
            cell.tool = Instantiate(tool.prefab, cell.point, cell.rotation);
            cell.tool.transform.parent = transform;
            cell.tool.Setup(pillar, tool.upgrade, cellId, globalIndex);
            cell.hasTool = true;
            cell.ghost.SetActive(false);
            cell.hasGhost = true;
            //cell.Deselect();
            return true;
        }
        return false;
    }
    public bool SpawnGhost(ToolGrid.Cell cell, ToolHolder.Tool tool)
    {
        cell.ghost = Instantiate(tool.ghost, cell.point, cell.rotation);
        cell.ghost.transform.parent = transform;
        cell.ghost.SetActive(false);
        cell.hasGhost = true;
        return true;
    }
}
