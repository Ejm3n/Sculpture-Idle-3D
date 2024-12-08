using BG.UI.Elements;
using BG.UI.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStartPanel : Panel
{
    private bool isActive = true;
    [SerializeField] private UIRewardSubPanel rewardPanel;
    [SerializeField] private UIButton startButton;
    public bool IsActive { get => isActive; set => isActive = value; }
    public UIButton StartButton => startButton;

    public override void Start()
    {
        base.Start();
        startButton.AddListener(OnStartButtonClick);
        LevelManager.Default.OnLevelLoaded += OnLevelLoaded;
        rewardPanel.CheckReward();

    }

    private void OnLevelLoaded()
    {

    }

    public override void ShowPanel(bool animate = true)
    {
        base.ShowPanel(animate);
    }

    public override void HidePanel(bool animate = true)
    {
        base.HidePanel(animate);
    }

    private void OnStartButtonClick()
    {
        LevelManager.Default.CurrentLevel.GameStart();
        startButton.Click();
    }
    private void LateUpdate()
    {
        InputController.Default.ResetDelta();
    }
}
