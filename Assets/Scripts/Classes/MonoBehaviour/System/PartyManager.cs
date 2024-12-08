using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
public class PartyManager : MonoBehaviour
{
    #region Singleton

    private static PartyManager _default;
    public static PartyManager Default => _default;

    #endregion

    public const string PREFS_PILLAR = "PillarState";
    public const string PREFS_PARTY = "PartyStruct";
    public const string PREFS_INCOME = "Income";
    public const string PREFS_EXIT = "ExitTime";
    public float elapsedSeconds;
    public ulong income;
    private void Awake()
    {
        _default = this;

        //LevelManager.Default.OnLevelLoaded += OnLevelLoaded;
        //LevelManager.Default.OnLevelStarted += OnLevelStarted;

    }

    private void OnApplicationQuit()
    {
        SaveTime();
    }
    public void SaveIncome(ToolController controller)
    {
        PlayerPrefs.SetString(PREFS_INCOME, controller.GetFullIncome().ToString());
    }
    public void LoadTime()
    {
        DateTime now = DateTime.Now;
        DateTime date = DateTime.Parse(PlayerPrefs.GetString(PREFS_EXIT, now.ToString()));
        Debug.Log($"Load time {date}");
        TimeSpan elapsedSpan = now - date;
        elapsedSeconds = (float)elapsedSpan.TotalSeconds;
        income = ulong.Parse(PlayerPrefs.GetString(PREFS_INCOME, "0"));
    }
    public void SaveTime()
    {
        string value = DateTime.Now.ToString();
        PlayerPrefs.SetString(PREFS_EXIT, value);
    }
    public void LoadParty(ToolController controller)
    {
        ToolGrid grid = controller.Grid;
        string party = PlayerPrefs.GetString(PREFS_PARTY, "");
        //Debug.Log(party);
        string[] tools = party.Split(',');
        for (int i = 0; i < tools.Length - 1; i++)
        {
            string[] tool = tools[i].Split('.');
            int id = int.Parse(tool[0]);
            int global = int.Parse(tool[1]);

            if (id < grid.Count)
            {
                var toolH = ToolHolder.Default.tools[global];
                var cell = grid[id];
                controller.SpawnTool(cell, toolH, id, global);
            }
        }

    }
    public void SaveParty(ToolController controller)
    {
        ToolGrid grid = controller.Grid;
        StringBuilder strB = new StringBuilder();
        for (int i = 0; i < grid.Count; i++)
        {
            var cell = grid[i];
            if (cell.hasTool)
            {
                strB.Append(i).Append('.').Append(cell.tool.GlobalIndex).Append(",");
            }
        }
        //Debug.Log(strB.ToString());

        PlayerPrefs.SetString(PREFS_INCOME, controller.GetFullIncome().ToString());
        PlayerPrefs.SetString(PREFS_PARTY, strB.ToString());
    }

    public void LoadPillar(PillarObject pillar)
    {
        string party = PlayerPrefs.GetString(PREFS_PILLAR, "");
        //Debug.Log(party);

        if (!string.IsNullOrEmpty(party))
        {
            string[] pillarState = party.Split('.');
            pillar.SetSegmentAndHealth(int.Parse(pillarState[0]), float.Parse(pillarState[1]));
        }
        else
        {
            pillar.SetSegmentAndHealth(0, 0);

        }
    }
    public void SavePillar(PillarObject pillar)
    {
        StringBuilder strB = new StringBuilder();
        strB.Append(pillar.ActiveSegmentId).Append('.').Append(pillar.ActiveSegment.Health);
        //Debug.Log(strB.ToString());
        PlayerPrefs.SetString(PREFS_PILLAR, strB.ToString());
    }
    public void ClearPillar()
    {
        PlayerPrefs.SetString(PREFS_PILLAR, "");

    }

}
