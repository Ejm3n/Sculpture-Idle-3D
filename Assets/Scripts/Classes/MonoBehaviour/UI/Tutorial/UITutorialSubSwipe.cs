using BG.UI.Main;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorialSubSwipe : UITutorialBase
{
    private Sequence anim;
    private Vector2 buttonPosition;
    private Vector2 cursorStartOffset;
    private Vector2 rightOffset,leftOffset;
    private UIBuySubPanel buySubPanel;
    private UIProcessPanel process;
    private UIUpgradeSubPanel upgrade;
    private ToolController controller;
    private PillarObject pillar;

    private int state;
    public override void Init(UITutorialCursor cursor)
    {
        base.Init(cursor);
        InputController.Default.PointerDown += OnPointerDown;
        process = UIManager.Default[UIState.Process] as UIProcessPanel;
        process.CanRotate = false;
        process.LimitRotate(1);
        upgrade = process.ToolPanel.UpgradePanel;
        buySubPanel = process.BuyPanel;
        buySubPanel.OnBuy += OnBuy;
        buttonPosition = buySubPanel.BuyButton.transform.position;
        cursorStartOffset = buttonPosition + new Vector2(Screen.width / 5f, Screen.height / 5f);
        rightOffset  = new Vector2(Screen.width / 2f, Screen.height / 2f) + new Vector2(Screen.width / 4f,0f);
        leftOffset = new Vector2(Screen.width / 2f, Screen.height / 2f) - new Vector2(Screen.width / 4f, 0f);

        LevelMaster lvlm = LevelManager.Default.CurrentLevel;
        controller = lvlm.ToolController;
        controller = LevelManager.Default.CurrentLevel.ToolController;
        controller.OnChange += OnChange;
        lvlm.GetPillar(out pillar);


    }
    private void OnDestroy()
    {
        if (buySubPanel != null)
            buySubPanel.OnBuy -= OnBuy;
        if (controller != null)
            controller.OnChange -= OnChange;
        InputController.Default.PointerDown -= OnPointerDown;

    }
    public override bool ShowCondition()
    {
        int index = ToolHolder.Default.GetCorrectIndex(1);
        ToolHolder.Tool tool = ToolHolder.Default.GetTool(index);

        if(MoneyService.Default.GetMoney() >= tool.price && !controller.Grid.HasSleepTool())
        {
            process.CanRotate = true;
            return true;
        }

        return false;

    }
    public override void Show()
    {
        base.Show();
        if(state == 0)
        {
            Swipe();
        }
        else if(state == 1)
        {
            Click();
        }
    }
    private void Swipe()
    {
        anim?.Kill();
        anim = DOTween.Sequence();

        anim.Append(cursor.Transform.DOScale(1f, 0f));
        anim.Append(cursor.Transform.DOMove(rightOffset, 0f));
        anim.Append(cursor.PlayFade(1f, 0.2f));
        anim.Append(cursor.PlayClick(0.5f));
        anim.Append(cursor.Transform.DOMove(leftOffset, 0.5f));
        anim.Append(cursor.PlayFade(0f, 0.2f));
        anim.Append(DOTween.To(() => 0f, (v) => { }, 0f, 0.5f));
        anim.SetLoops(-1, LoopType.Restart);
    }
    private void Click()
    {
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
    private void OnPointerDown()
    {
        
    }
    private void OnChange(ToolGrid.Cell cell)
    {
        if (enabled)
        {
            process.CanRotate = false;
            state = 1;
            Show();
            upgrade.UpgradeButton.Lock = false;
        }
    }
    private void OnBuy(ToolBase tool)
    {
        if (enabled)
        {
            Hide();
            Deselect();
            enabled = false;
            process.CanRotate = true;
            process.LimitRotate(-1);
        }
    }
    public override void Select()
    {
        controller.StartTool.enabled = false;
        upgrade.UpgradeButton.Interactable = false;
        upgrade.UpgradeButton.Lock = true;
        pillar.Damageable = false;
        process.CanRotate = true;

    }

    public override void Deselect()
    {
        controller.StartTool.enabled = true;
        pillar.Damageable = true;


    }
}


