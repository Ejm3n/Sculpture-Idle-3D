using BG.UI.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToolSubPanel : Panel
{
    private bool hasTool;
    private ToolBase tool;
    [SerializeField] private UIUpgradeSubPanel upgradePanel;
    [SerializeField] private UISleepSubPanel sleepPanel;
    [SerializeField] private UIActionProgress sleep;

  //  bool lastSleep,lastPrepare;

    public UIUpgradeSubPanel UpgradePanel => upgradePanel;
    public UISleepSubPanel SleepPanel => sleepPanel;
    public override void HidePanel(bool animate = true)
    {
        base.HidePanel(animate);
        upgradePanel.HidePanel(false);
        sleepPanel.HidePanel(false);
        enabled = false;     
    }
    public override void ShowPanel(bool animate = true)
    {
        base.ShowPanel(animate);
        //if (hasTool)
        //    SetTool(tool);
    }
    public void SetTool(ToolBase tool)
    {
        this.tool = tool;
        hasTool = true;
        enabled = true;
        upgradePanel.SetTool(tool);
        sleepPanel.SetTool(tool);
        //lastSleep = !tool.InSleep || tool.InPrepareWakeup;
        //lastPrepare = !tool.InPrepareWakeup;
        if (tool.InSleep)
        {
            upgradePanel.enabled = false;
            upgradePanel.HidePanel(false);
            sleepPanel.ShowPanel(false);
        }
        else
        {
            upgradePanel.enabled = true;
            sleepPanel.HidePanel(false);
            upgradePanel.ShowPanel(false);
        }

    }
    private void LateUpdate()
    {
        if (hasTool)
        {
            //if (lastSleep != (tool.InSleep && !tool.InPrepareWakeup))
            //{
            //    if (tool.InSleep && !tool.InPrepareWakeup)
            //    {
            //        upgradePanel.HidePanel(false);
            //        sleepPanel.enabled = true;
            //        sleepPanel.ShowPanel(false);
            //    }
            //    else
            //    {
            //        sleepPanel.HidePanel(false);
            //        upgradePanel.SetTool(tool);
            //        upgradePanel.enabled = true;
            //        if (!tool.InPrepareWakeup)
            //            upgradePanel.ShowPanel(false);
            //    }
            //    lastSleep = tool.InSleep && !tool.InPrepareWakeup;
            //}
            //if (lastPrepare != tool.InPrepareWakeup)
            //{
            //    if (!tool.InSleep && !tool.InPrepareWakeup)
            //        upgradePanel.ShowPanel(false);
            //    lastPrepare = tool.InPrepareWakeup;
            //}


            if (tool.InSleep)
            {
                upgradePanel.HidePanel(false);
                if (tool.InPrepareWakeup)
                {
                    sleepPanel.HidePanel(false);
                }
                else
                {
                    sleepPanel.enabled = true;
                    sleepPanel.ShowPanel(false);
                }
            }
            else
            {
                sleepPanel.HidePanel(false);
                if (!tool.InPrepareWakeup)
                {
                    upgradePanel.enabled = true;
                    upgradePanel.ShowPanel(false);
                }
            }              



                sleep.Canvas.enabled = !tool.InPrepareWakeup;
            float progress = tool.GetSleepProgress();           
            sleep.SetProgress(progress);
            if (progress >= 1f)
                sleep.PlayIdle();
            else
                sleep.Stop();

        }
        else
            enabled = false;
    }
}
