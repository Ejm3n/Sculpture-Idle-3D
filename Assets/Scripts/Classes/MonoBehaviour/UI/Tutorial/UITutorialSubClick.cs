using BG.UI.Main;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorialSubClick : UITutorialBase
{
    private Sequence anim;
    private Vector2 screenCenter;
    private Vector2 cursorStartOffset;
    private ToolController controller;
    private void OnDestroy()
    {
        InputController.Default.PointerDown -= OnPointerDown;
    }

    public override void Init(UITutorialCursor cursor)
    {
        base.Init(cursor);
        UIProcessPanel process = UIManager.Default[UIState.Process] as UIProcessPanel;
        process.CanRotate = false;
        InputController.Default.PointerDown += OnPointerDown;
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        cursorStartOffset = screenCenter + new Vector2(Screen.width / 4f, Screen.height / 4f);
        LevelMaster lvlm = LevelManager.Default.CurrentLevel;
        controller = lvlm.ToolController;

    }

    public override bool ShowCondition()
    {
        return true;
    }

    public override void Show()
    {
        base.Show();
        anim?.Kill();
        anim = DOTween.Sequence();

        anim.Append(cursor.Transform.DOScale(1f, 0f));
        anim.Append(cursor.Transform.DOMove(cursorStartOffset, 0f));
        anim.Append(cursor.PlayFade(1f, 0.2f));
        anim.Append(cursor.Transform.DOMove(screenCenter, 0.5f));
        for (int i = 0; i < 3; i++)
            anim.Append(cursor.PlayClick(0.5f));
        anim.Append(cursor.PlayFade(0f, 0.2f));
        anim.Append(DOTween.To(() => 0f, (v) => { }, 0f, 0.5f));
        anim.SetLoops(-1, LoopType.Restart);

    }
    public override void Hide()
    {
        base.Hide();
        anim?.Kill();
        cursor.PlayFade(0f,0.2f);
    }
    private void OnPointerDown()
    {
        if(enabled)
        Hide();
    }

    public override void Select()
    {
    }

    public override void Deselect()
    {
    }
}
