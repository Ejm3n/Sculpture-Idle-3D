using BG.UI.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRewardSubPanel : Panel
{
    [SerializeField] private UIButton claimButton;
    [SerializeField] private UIText info;
    [SerializeField] private MoneyBlastAnimation moneyBlast;
    [SerializeField] private Transform shine;
    private ulong reward;
    public override void Start()
    {
        claimButton.AddListener(Claim);
    }
    public void CheckReward()
    {
        PartyManager.Default.LoadTime();
        float time = PartyManager.Default.elapsedSeconds;
        Debug.Log($"Elapsed Seconds {time}");
        if(time >= GameData.Default.minOfflineTime)
        {
            time = Mathf.Clamp(time, GameData.Default.minOfflineTime, GameData.Default.maxOfflineTime);
            reward = (ulong)time * PartyManager.Default.income;
            if(reward > 0)
            {
                info.UpdateText(MoneyService.AmountToString(reward));
                ShowPanel(true);

            }
        }
    }
    public void Claim()
    {
        MoneyService.Default.AddMoney(reward);
        moneyBlast.Play();
        claimButton.Interactable = false;
        HidePanel();
    }
    private void LateUpdate()
    {
        shine.Rotate(Vector3.forward * 10f * Time.deltaTime);
    }
}
