using BG.UI.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolCursor : ToolBase
{
    private Vector2 inputPosition;
    public float input = 10f;
    private void Start()
    {
        InputController.Default.PointerDown += OnPointerDown;
        InputController.Default.PointerUp += Action;
    }
    private void OnDestroy()
    {
        InputController.Default.PointerDown -= OnPointerDown;
        InputController.Default.PointerUp -= Action;
    }

    private void OnPointerDown()
    {
        inputPosition = InputController.Default.Position;
    }
    protected override void Action()
    {
        Vector2 delta = InputController.Default.UpPosition - inputPosition;
        delta.x /= Screen.width;
        delta.y /= Screen.height;
        Vector2 zs = GameData.Default.inputClickZoneSizeScale;
        zs.x *= Screen.width;
        zs.y *= Screen.height;
        Vector2 zo = GameData.Default.inputClickZoneOffsetScale;
        zo.x *= Screen.width;
        zo.y *= Screen.height;
        zo.x += Screen.width * 0.5f;
        zo.y += Screen.height * 0.5f;

        if (enabled &&
            Mathf.Abs(delta.x) < GameData.Default.inputDeltaLessToClick &&
            inputPosition.x >= zo.x - zs.x && inputPosition.x <= zo.x + zs.x &&
            inputPosition.y >= zo.y - zs.y && inputPosition.y <= zo.y + zs.y
            )
        {
            //Camera cam = CinemachineBrain.Default.Camera;
            //Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * input) + cam.transform.forward;
            
            pillar.Hurt(property.Damage);
            MoneyService.Default.AddMoney(property.Income);
            OnAction?.Invoke();
            //if(pillar.IsAlive)
            //CinemachineBrain.Default.Shake();
        }
    }
    protected override bool CanAction()
    {
        return false;
    }
    protected override bool Sleep()
    {
        return false;
    }
    public override void Execute(){}

    public override void Install(){}

    public override void Finish(){}
}
