using BG.UI.Main;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorialSubSleep : UITutorialBase
{
    private Sequence anim;
    private Vector2 buttonPosition;
    private Vector2 cursorStartOffset;
    private UISleepSubPanel sleepSubPanel;
    private ToolController controller;
    private PillarObject pillar;
    public override void Init(UITutorialCursor cursor)
    {
        base.Init(cursor);
        UIProcessPanel process = UIManager.Default[UIState.Process] as UIProcessPanel;
        process.CanRotate = false;
        sleepSubPanel = process.ToolPanel.SleepPanel;
        sleepSubPanel.OnWakeup += OnWakeup;
        buttonPosition = sleepSubPanel.WakeupButton.transform.position;
        cursorStartOffset = new Vector2(Screen.width / 2f, Screen.height / 2f);
        LevelMaster lvlm = LevelManager.Default.CurrentLevel;
        controller = lvlm.ToolController;
        lvlm.GetPillar(out pillar);
    }
    private void OnDestroy()
    {
        if (sleepSubPanel != null)
        {
            sleepSubPanel.OnWakeup -= OnWakeup;
        }
    }
    public override bool ShowCondition()
    {
        ToolBase tool;
        if (sleepSubPanel.GetTool(out tool))
            return tool.InSleep;
        return false;

    }
    public override void Show()
    {
        base.Show();
        anim?.Kill();
        anim = DOTween.Sequence();

        anim.Append(cursor.Transform.DOScale(1f, 0f));
        anim.Append(cursor.Transform.DOMove(cursorStartOffset, 0f));
        anim.Append(cursor.PlayFade(1f, 0.2f));
        anim.Append(cursor.Transform.DOMove(buttonPosition, 0.5f));
        anim.Append(cursor.PlayClick(0.5f));
        anim.Append(cursor.PlayFade(0f, 0.2f));
        anim.Append(DOTween.To(() => 0f, (v) => { }, 0f, 0.5f));
        anim.SetLoops(-1, LoopType.Restart);

    }
    public override void Hide()
    {
        base.Hide();
        anim?.Kill();
        cursor.PlayFade(0f, 0.2f);
    }
    private void OnWakeup()
    {
        if (enabled)
        {
            Hide();
            Deselect();
            enabled = false;
        }
    }
    public override void Select()
    {
        controller.StartTool.enabled = false;
        pillar.Damageable = false;
    }

    public override void Deselect()
    {
        controller.StartTool.enabled = true;
        pillar.Damageable = true;
    }
}
