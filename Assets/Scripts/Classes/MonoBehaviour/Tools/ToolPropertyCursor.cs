using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPropertyCursor : ToolProperty
{
    private const string PREF_UPGRADE = "CursorUpgrade";
    private ToolController toolController;
    private void Start()
    {
        toolController = LevelManager.Default.CurrentLevel.ToolController;
        toolController.OnAdd += OnAdd;
    }
    private void OnDestroy()
    {
        if(toolController != null)
            toolController.OnAdd -= OnAdd;

    }
    private void OnAdd(ToolBase tool)
    {
        var up = tool.Property.ToolUpgrade;
        currentIncome = ulong.Parse(PlayerPrefs.GetString(PREF_UPGRADE, upgrade.EvaluateIncome(0).ToString()));
        currentIncome += (ulong)(up.EvaluateIncome(0) / up.EvaluateTime(0) * GameData.Default.cursorUpgradeMultiplier);
        PlayerPrefs.SetString(PREF_UPGRADE, currentIncome.ToString());
    }
    public override int CurrentLevel(int index)
    {
        string pref = PREF_UPGRADE;
        return PlayerPrefs.GetInt(pref, 0);
    }
    public override void Restore(int index)
    {
        currentIncome = ulong.Parse(PlayerPrefs.GetString(PREF_UPGRADE, upgrade.EvaluateIncome(0).ToString()));
        currentDamage = upgrade.EvaluateDamage(0);
    }
    public override int Upgrade(int index)
    {

        return 0;
    }
    public override int UpgradeTime(int index)
    {
        return 0;
    }
}
