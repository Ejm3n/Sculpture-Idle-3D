using BG.UI.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISleepSubPanel : Panel
{
    private bool hasTool;
    private ToolBase tool;
    [SerializeField] private UIButton wakeupButton;
    private bool canWakeup = true;
    public System.Action OnSleep;
    public System.Action OnWakeup;
    private Vector2 inputPosition;


    public UIButton WakeupButton => wakeupButton;

    public bool CanWakeup { get => canWakeup; set => canWakeup = value; }

    public bool GetTool(out ToolBase tool)
    {
        tool = this.tool;
        return hasTool;
    }
    public override void Start()
    {
        base.Start();
        wakeupButton.AddListener(Wakeup);
        InputController.Default.PointerUp += OnPointerUp;
        InputController.Default.PointerDown += OnPointerDown;
    }
    private void OnDestroy()
    {
        InputController.Default.PointerDown -= OnPointerDown;
        InputController.Default.PointerUp -= OnPointerUp;
    }
    public override void ShowPanel(bool animate = true)
    {
        base.ShowPanel(animate);
        OnSleep?.Invoke();
    }
    public override void HidePanel(bool animate = true)
    {
        base.HidePanel(animate);
        enabled = false;
    }
    public void SetTool(ToolBase tool)
    {
        this.tool = tool;
        hasTool = true;
        enabled = true;
    }
    private void Wakeup()
    {
        if (hasTool)
        {
            if(canWakeup)
            tool.Wakeup();
            OnWakeup?.Invoke();
        }
    }
    private void OnPointerDown()
    {
        inputPosition = InputController.Default.Position;
    }
    private void OnPointerUp()
    {
        Vector2 delta = InputController.Default.UpPosition - inputPosition;
        delta.x /= Screen.width;
        delta.y /= Screen.height;

        if (hasTool && enabled && gameObject.activeSelf &&
            Mathf.Abs(delta.x) < GameData.Default.inputDeltaLessToClick &&
            InputController.Default.ClickTime <= 0.2f)
        {
            Vector2 upPos = InputController.Default.UpPosition;
            Vector2 zs = GameData.Default.inputWakeupZoneSizeScale;
            zs.x *= Screen.width;
            zs.y *= Screen.height;
            Vector2 zo = GameData.Default.inputWakeupZoneOffsetScale;
            zo.x *= Screen.width;
            zo.y *= Screen.height;
            zo.x += Screen.width * 0.5f;
            zo.y += Screen.height * 0.5f;

            if (upPos.x >= zo.x - zs.x && upPos.x <= zo.x + zs.x &&
                upPos.y >= zo.y - zs.y && upPos.y <= zo.y + zs.y
                )
            {
                Wakeup();



            }
        }
    }
}