using BG.UI.Main;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorialSubUpgrade : UITutorialBase
{
    private Sequence anim;
    private Vector2 buttonPosition;
    private Vector2 cursorStartOffset;
    private UIUpgradeSubPanel upgradeSubPanel;
    private bool hasTool;
    private ToolBase tool;
    private ToolController controller;
    private PillarObject pillar;



    public override void Init(UITutorialCursor cursor)
    {
        base.Init(cursor);
        UIProcessPanel process = UIManager.Default[UIState.Process] as UIProcessPanel;
        process.CanRotate = false;
        upgradeSubPanel = process.ToolPanel.UpgradePanel;
        upgradeSubPanel.OnUpgrade += OnUpgrade;
        buttonPosition = upgradeSubPanel.UpgradeButton.transform.position;
        cursorStartOffset = new Vector2(Screen.width / 2f, Screen.height / 2f);
        LevelMaster lvlm = LevelManager.Default.CurrentLevel;
        controller = lvlm.ToolController;
        lvlm.GetPillar(out pillar);
    }
    private void OnDestroy()
    {
        if (upgradeSubPanel != null)
            upgradeSubPanel.OnUpgrade -= OnUpgrade;
    }
    public override bool ShowCondition()
    {
        if(hasTool = upgradeSubPanel.GetTool(out tool))
        {
            tool.CanSleep = false;
            tool.RestartSleep();
            return MoneyService.Default.GetMoney() >= tool.Property.Price;
        }
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
    private void OnUpgrade()
    {
        if (enabled)
        {
            Hide();
            Deselect();
            enabled = false;
            if (hasTool)
                tool.CanSleep = true;
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
