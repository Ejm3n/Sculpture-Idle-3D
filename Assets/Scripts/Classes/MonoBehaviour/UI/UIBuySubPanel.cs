using BG.UI.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBuySubPanel : Panel
{
    private ToolController toolController;
    [SerializeField] private UIButtonText buyButton;
    public System.Action<ToolBase> OnBuy;

    public UIButton BuyButton => buyButton;
    public override void ShowPanel(bool animate = true)
    {
        base.ShowPanel(animate);
        UpdateButton();
    }
    public override void Start()
    {
        base.Start();
        MoneyService.Default.OnMoneyChanged += UpdateButton;
        buyButton.AddListener(Buy);
        UpdateButton();
    }
    private void OnDestroy()
    {
        MoneyService.Default.OnMoneyChanged -= UpdateButton;
    }
    public void UpdateButton()
    {
        int index = ToolHolder.Default.GetCorrectIndex(toolController.ActiveIndex);
        ToolHolder.Tool tool = ToolHolder.Default.GetTool(index);
        ulong price = tool.price;
        buyButton.UpdateText(MoneyService.AmountToString(price));
        buyButton.Interactable = MoneyService.Default.GetMoney() >= price;
    }
    public void SetToolController(ToolController toolController)
    {
        this.toolController = toolController;
    }
    private void Buy()
    {
        int index = ToolHolder.Default.GetCorrectIndex(toolController.ActiveIndex);
        ToolHolder.Tool tool = ToolHolder.Default.GetTool(index);
        //Debug.Log(toolController.Selected.hasTool + " " + (tool.price));
        if(!toolController.Selected.hasTool && buyButton.Interactable && MoneyService.Default.GetMoney() >= tool.price)
        {
            ToolBase t = toolController.InstalToolInActiveCell(tool, index);
            MoneyService.Default.SpendMoney(tool.price);
            UpdateButton();
            OnBuy?.Invoke(t);
        }
    }
}
