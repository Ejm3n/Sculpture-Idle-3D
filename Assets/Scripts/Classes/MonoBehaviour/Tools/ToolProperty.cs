using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolProperty : MonoBehaviour
{
    protected ToolUpgrade upgrade;
    protected ulong currentPrice;
    protected ulong currentPriceTime;
    protected float currentDamage;
    protected float currentTimeToAction;
    protected ulong currentIncome;
    protected int currentActionToSleep;
    protected ulong incomePreSecond;

    public ulong Price => currentPrice;
    public ulong PriceTime => currentPriceTime;
    public float Damage => currentDamage * GameData.Default.damageScale;
    public float TimeToAction => currentTimeToAction;
    public ulong Income => currentIncome;
    public ulong IncomePreSecond => incomePreSecond;
    public bool CanSleep => upgrade.CanSleep;
    public float ActionToSleep => currentActionToSleep;
    public ToolUpgrade ToolUpgrade => upgrade;

    public virtual int CurrentLevel(int index)
    {
        return upgrade.CurrentLevel(index);
    }
    public virtual void SetUpgrade(ToolUpgrade upgrade)
    {
        this.upgrade = upgrade;
    }
    public virtual void Restore(int index)
    {
        int level = upgrade.CurrentLevel(index);
        int levelT = upgrade.CurrentLevelTime(index);
        currentPrice = upgrade.EvaluatePrice(level);
        currentPriceTime = upgrade.EvaluatePriceTime(levelT);
        currentIncome = upgrade.EvaluateIncome(level);
        currentTimeToAction = upgrade.EvaluateTime(levelT);
        currentDamage = upgrade.EvaluateDamage(level);
        currentActionToSleep = upgrade.EvaluateSleep(level);
        incomePreSecond = (ulong)(currentIncome / currentTimeToAction + 0.01f);
    }
    public virtual int Upgrade(int index)
    {
        int level = upgrade.Upgrade(index);
        currentPrice = upgrade.EvaluatePrice(level);
        currentIncome = upgrade.EvaluateIncome(level);
        //currentTimeToAction = upgrade.EvaluateTime(level);
        currentDamage = upgrade.EvaluateDamage(level);
        currentActionToSleep = upgrade.EvaluateSleep(level);
        incomePreSecond = (ulong)(currentIncome / currentTimeToAction + 0.01f);
        return level;
    }
    public virtual int UpgradeTime(int index)
    {
        int level = upgrade.UpgradeTime(index);
        currentPriceTime = upgrade.EvaluatePriceTime(level);
        currentTimeToAction = upgrade.EvaluateTime(level);
        incomePreSecond = (ulong)(currentIncome / currentTimeToAction + 0.01f);

        return level;
    }
}
