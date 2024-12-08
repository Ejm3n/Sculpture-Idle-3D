using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolGrid : MonoBehaviour
{
    [System.Serializable]
    public class Cell
    {
        public Vector3 point;
        public Quaternion rotation;
        public ToolBase tool;
        public bool hasTool;
        public GameObject ghost;
        public bool hasGhost;
        public GridPlatform platform;
        public Cell(Vector3 from, Vector3 to)
        {
            rotation = Quaternion.LookRotation((from - to).normalized, Vector3.up);
            point = to;
        }

        public void Select()
        {
            if (hasGhost)
                ghost.SetActive(true && !hasTool);
            platform.Select();
        }
        public void Deselect()
        {
            if (hasGhost)
                ghost.SetActive(false);
            platform.Deselect();
        }
    }

    [SerializeField, Min(1)] protected int segments = 1;
    [SerializeField] protected float radius = 1f;
    [SerializeField] private GridPlatform[] platforms;
    protected float cellStep;
    private List<Cell> cells = new List<Cell>();

    public int Segments => segments;
    public float Radius => radius;
    public float CellStep => cellStep;
    public int Count => cells.Count;

    public Cell this[int id] { get => cells[id]; }
#if UNITY_EDITOR

    private void OnValidate()
    {
        if (transform.childCount >= 1 && !Application.isPlaying && enabled && gameObject.activeSelf && gameObject.activeInHierarchy && platforms != null && platforms.Length >=1)
        {

            int i = 0;
            float step = 360.0f / segments;

            for (i = 0; i < platforms.Length && i < segments; i++)
            {
                Vector3 pos = (Quaternion.AngleAxis(-step * i, Vector3.up) * Vector3.back) * radius;
                Quaternion rot = Quaternion.LookRotation((Vector3.zero - pos).normalized, Vector3.up);
                platforms[i].transform.position = transform.position + pos;
                platforms[i].transform.rotation = rot;
            }
            UnityEditor.EditorUtility.SetDirty(transform.root);
        }


    }


#endif
    public int GetToolCount()
    {
        int count = 0;
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].hasTool)
                count++;
        }
        return count;
    }
    public void SetPillar(PillarObject pillar)
    {
        //for (int i = 0; i < cells.Count; i++)
        //{
        //    Cell cell = cells[i];
        //    if (cell.hasTool)
        //        cell.tool.SetPillar(pillar, i);
        //}
    }
    public void Initialize()
    {
        cellStep = 360.0f / segments;
        for (int i = 0; i < segments; i++)
        {
            Cell cell = new Cell(transform.position, transform.position + (Quaternion.AngleAxis(-cellStep * i, Vector3.up) * Vector3.back) * radius);
            cell.platform = platforms[i];
            cells.Add(cell);

        }
    }
    public void Execute()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            Cell cell = cells[i];
            if (cell.hasTool)
            {
                cell.tool.enabled = true;
                cell.tool.Execute();
            }
        }
    }
    public void Stop()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            Cell cell = cells[i];
            if (cell.hasTool)
                cell.tool.enabled = false;
        }
    }

    public void Finish()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            Cell cell = cells[i];
            if (cell.hasTool)
            {
                cell.tool.enabled = false;
                cell.tool.Finish();
                cell.Deselect();
            }
        }
    }
    public bool HasEmptyCells()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (!cells[i].hasTool)
                return true;
        }
        return false;
    }
    public bool HasFilledCells()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].hasTool)
                return true;
        }
        return false;
    }
    public bool HasSleepTool()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].hasTool && cells[i].tool.InSleep)
            {
                return true;
            }
        }
        return false;
    }
    public bool GetEmptyCell(out Cell cell)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (!cells[i].hasTool)
            {
                cell = cells[i];
                return true;
            }
        }
        cell = default;
        return false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        if (Application.isPlaying)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                Gizmos.DrawLine(transform.position, cells[i].point);
                Gizmos.DrawWireSphere(cells[i].point, 0.25f);
            }
        }
        else
        {
            float step = 360.0f / segments;
            for (int i = 0; i < segments; i++)
            {
                Vector3 pos = (Quaternion.AngleAxis(step * i, Vector3.up) * Vector3.back) * radius;
                Gizmos.DrawLine(transform.position, transform.position + pos);
                Gizmos.DrawWireSphere(transform.position + pos, 0.25f);
            }
        }
    }
}
