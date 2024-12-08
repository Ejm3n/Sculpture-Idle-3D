using BG.UI.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUpgradeSubPanel : Panel
{
    private bool hasTool;
    private ToolBase tool;
    [SerializeField] private UIButtonText upgradeButton;
    [SerializeField] private UIButtonText upgradeTimeButton;
    [SerializeField] private UIText incomeInfo;
    [SerializeField] private UIText levelInfo;
    [SerializeField] private UIActionProgress cooldown;
    public System.Action OnUpgrade;
    public UIButton UpgradeButton => upgradeButton; 
    public override void Start()
    {
        base.Start();
        MoneyService.Default.OnMoneyChanged += UpdateButton;
        upgradeButton.AddListener(Upgrade);
        upgradeTimeButton.AddListener(UpgradeTime);
        if (hasTool)
            UpdateButton();
        else
        {
            upgradeButton.Interactable = false;
            upgradeTimeButton.Interactable = false;
        }
    }
    private void OnDestroy()
    {
        MoneyService.Default.OnMoneyChanged -= UpdateButton;
        if (hasTool)
            this.tool.OnAction -= OnAction;
    }
    public override void HidePanel(bool animate = true)
    {
        base.HidePanel(animate);
        if (hasTool)
            this.tool.OnAction -= OnAction;

       // hasTool = false;
        enabled = false;
        if (hasTool)
            UpdateButton();
        else
        {
            upgradeButton.Interactable = false;
            upgradeTimeButton.Interactable = false;
        }
    }
    public bool GetTool(out ToolBase tool)
    {
        tool = this.tool;
        return hasTool;
    }
    public void UpdateButton()
    {
        if (hasTool)
        {
            ulong price = tool.Property.Price;
            ulong priceT = tool.Property.PriceTime;

            upgradeButton.UpdateText(MoneyService.AmountToString(price));
            upgradeButton.Interactable = MoneyService.Default.GetMoney() >= price;

            upgradeTimeButton.UpdateText(MoneyService.AmountToString(priceT));
            upgradeTimeButton.Interactable = MoneyService.Default.GetMoney() >= priceT;


            incomeInfo.UpdateText(MoneyService.AmountToString(tool.Property.Income));
            levelInfo.UpdateText((tool.Level+1).ToString());
        }
    }
    public void SetTool(ToolBase tool)
    {
        if(hasTool)
            this.tool.OnAction -= OnAction;
        this.tool = tool;
        this.tool.OnAction += OnAction;
        hasTool = true;
        enabled = true;
        UpdateButton();
    }
    private void Upgrade()
    {
        if (hasTool && upgradeButton.Interactable && MoneyService.Default.GetMoney() >= tool.Property.Price)
        {
            tool.Upgrade();
            MoneyService.Default.SpendMoney(tool.Property.Price);
            UpdateButton();
            OnUpgrade?.Invoke();
        }
    }
    private void UpgradeTime()
    {
        if (hasTool && upgradeTimeButton.Interactable && MoneyService.Default.GetMoney() >= tool.Property.PriceTime)
        {
            tool.UpgradeTime();
            MoneyService.Default.SpendMoney(tool.Property.PriceTime);
            UpdateButton();
            OnUpgrade?.Invoke();
        }
    }
    private void OnAction()
    {
        if(enabled)
        cooldown.PlayBubble();
    }
    private void LateUpdate()
    {
        if (hasTool)
        {
            cooldown.SetProgress(tool.GetActionProgress());
        }
        else
            enabled = false;
    }
}
