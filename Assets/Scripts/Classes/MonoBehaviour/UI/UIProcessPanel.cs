using BG.UI.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProcessPanel : Panel
{
    private bool isActive;
    private bool canRotate = true;
    private LevelMaster levelMaster;
    private ToolController toolController;
    private PillarObject pillar;
    [SerializeField] private UIText incomeInfo;
    [SerializeField] private UIToolSubPanel toolPanel;
    [SerializeField] private UIBuySubPanel buyPanel;
    [SerializeField] private UICameraVelocity cameraVelocity;
    [SerializeField] private UIButton sleepButton;
    [SerializeField] private UIActionProgress levelProgress;

    public UIToolSubPanel ToolPanel => toolPanel;
    public UIBuySubPanel BuyPanel => buyPanel;
    public bool CanRotate { get => canRotate; set => canRotate = value; }
    public void LimitRotate(int id)
    {
        cameraVelocity.Limit(id);
    }

    protected override void Awake()
    {
        base.Awake();
        LevelManager.Default.OnLevelLoaded += OnLevelLoaded;
        LevelManager.Default.OnLevelPreLoad += OnLevelPreLoad;
        LevelManager.Default.OnLevelStarted += OnLevelStarted;
        MoneyService.Default.OnMoneyChanged += OnMoneyChanged;
        InputController.Default.PointerUp += OnPointerUp;
    }
    public override void Start()
    {
        base.Start();

        sleepButton.AddListener(OnSleepButtonClick);
    }
    private void OnDestroy()
    {
        LevelManager.Default.OnLevelLoaded -= OnLevelLoaded;
        LevelManager.Default.OnLevelPreLoad -= OnLevelPreLoad;
        LevelManager.Default.OnLevelStarted -= OnLevelStarted;
        MoneyService.Default.OnMoneyChanged -= OnMoneyChanged;
        InputController.Default.PointerUp -= OnPointerUp;
    }
    public override void ShowPanel(bool animate = true)
    {
        base.ShowPanel(animate);
        sleepButton.gameObject.SetActive(false);
        isActive = true;
        incomeInfo.UpdateText(MoneyService.NumberToString(toolController.GetFullIncome()));
   
    }
    public override void HidePanel(bool animate = true)
    {
        base.HidePanel(animate);
        toolPanel.HidePanel();
        buyPanel.HidePanel();
        isActive = false;

    }
    private void OnLevelLoaded()
    {
        levelMaster = LevelManager.Default.CurrentLevel;
        levelMaster.GetPillar(out pillar);
        toolController = levelMaster.ToolController;
        toolController.OnChange += OnToolChange;
        buyPanel.SetToolController(toolController);
        cameraVelocity.SetLevelMaster(levelMaster);
    }
    private void OnLevelPreLoad()
    {
        if (LevelManager.Default.CurrentLevel != null)
        {
            toolController.OnChange -= OnToolChange;

        }
    }

    private void OnLevelStarted()
    {
        OnToolChange(toolController.Selected);
    }
    private void OnMoneyChanged()
    {
        incomeInfo.UpdateText(MoneyService.NumberToString(toolController.GetFullIncome()));
    }
    private void OnToolChange(ToolGrid.Cell cell)
    {
        if (isActive && levelMaster.State == LevelMaster.LevelState.process)
        {
            if (cell.hasTool)
            {
                toolPanel.SetTool(cell.tool);
                toolPanel.ShowPanel(false);
                buyPanel.HidePanel(false);
            }
            else
            {
                buyPanel.UpdateButton();
                buyPanel.ShowPanel(false);
                toolPanel.HidePanel(false);
            }
            incomeInfo.UpdateText(MoneyService.NumberToString(toolController.GetFullIncome()));

        }
    }
    private void OnSleepButtonClick()
    {
        int ai = toolController.ActiveIndex;
        int ci = ai;
        float dist = float.PositiveInfinity;
        var grid = toolController.Grid;
        for (int i = 0; i < grid.Count; i++)
        {
            if (grid[i].hasTool && grid[i].tool.InSleep)
            {
                float d = (grid[ai].point - grid[i].point).sqrMagnitude;
                if (d <= dist)
                {
                    dist = d;
                    ci = i;
                }
            }
        }

        cameraVelocity.SetCellIndex(ci);
    }
    public void OnPointerUp()
    {
        if (isActive && levelMaster.State == LevelMaster.LevelState.process)
            cameraVelocity.Unblock();
    }
    private void LateUpdate()
    {
        if (isActive && levelMaster.State == LevelMaster.LevelState.process)
        {

            InputController input = InputController.Default;
            if (Mathf.Abs(input.Delta.x) >=
            GameData.Default.inputDeltaMinToRotate)
            {
                if (canRotate)
                    cameraVelocity.DirectRotate(input.Delta.x);
                input.ResetDelta();
            }
            bool hs = toolController.Grid.HasSleepTool();
            if(hs != sleepButton.gameObject.activeSelf)
            {
                sleepButton.gameObject.SetActive(hs);
               // OnSleepButtonClick();
            }
        }
        else
        {
            InputController.Default.ResetDelta();
        }
        levelProgress.SetProgress(1f - pillar.Percent);

    }
}
