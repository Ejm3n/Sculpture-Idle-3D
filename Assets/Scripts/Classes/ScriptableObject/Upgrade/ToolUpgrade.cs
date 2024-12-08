using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Tool Upgrade")]
public class ToolUpgrade : ScriptableObject
{
    private const string PREF_UPGRADE_LEVEL = "ToolUpgradeLevel";
    private const string PREF_TIME_UPGRADE_LEVEL = "ToolTimeUpgradeLevel";
    //[Header("Base Value")]
    [SerializeField] private ulong price = 50;
    [SerializeField] private ulong priceTime = 50;
    [SerializeField, Min(0.001f)] private float damage = 1f;
    [SerializeField, Min(0.001f)] private float timeToAction = 1f;
    [SerializeField] private ulong income = 5;
    [SerializeField] private bool canSleep;
    [SerializeField] private int actionToSleep = 5;
    //[Header("Multipliers")]
    [SerializeField] private double priceMultiplier = 1.1;
    [SerializeField] private double priceTimeMultiplier = 1.1;
    [SerializeField] private ulong priceFistIncomeAddMultiplier = 5L;
    [SerializeField] private int  levelCountToSecondAnd = 5;
    [SerializeField] private ulong priceSecondIncomeAddMultiplier = 3L;
    [SerializeField] private ulong incomeIncrease = 5;
    [SerializeField] private float timeMultiplier = 1f;
    [SerializeField] private float damageIncrease = 5;
    [SerializeField] private float sleepMultiplier = 1f;


    public System.Action OnUpgrade;
    public bool CanSleep => canSleep;
    public int CurrentLevel(int index)
    {
        string pref = PREF_UPGRADE_LEVEL + index.ToString();
        return PlayerPrefs.GetInt(pref, 0);
    }
    public int CurrentLevelTime(int index)
    {
        string pref = PREF_TIME_UPGRADE_LEVEL + index.ToString();
        return PlayerPrefs.GetInt(pref, 0);
    }
    public int Upgrade(int index)
    {
        string pref = PREF_UPGRADE_LEVEL + index.ToString();
        int level = PlayerPrefs.GetInt(pref, 0) + 1;
        PlayerPrefs.SetInt(pref, level);
        return level;
    }
    public int UpgradeTime(int index)
    {
        string pref = PREF_TIME_UPGRADE_LEVEL + index.ToString();
        int level = PlayerPrefs.GetInt(pref, 0) + 1;
        PlayerPrefs.SetInt(pref, level);
        return level;
    }
    public ulong EvaluatePrice(int level)
    {
        //return (ulong)(price * (Math.Pow(priceMultiplier, level)));
        //return price + EvaluateIncome(level) * 3L - (level == 0?  income * 3L :0L);
        ulong p = (ulong)(price * (Math.Pow(priceMultiplier, level)) + 0.01);
        ulong a = EvaluateIncome(level) * priceFistIncomeAddMultiplier;
        if (level == 0)
        {
            a = 0L;
        }
        else if( level >= levelCountToSecondAnd)
        {
            a = EvaluateIncome(level) * priceSecondIncomeAddMultiplier;
        }
        return p + a;
    }
    public ulong EvaluatePriceTime(int level)
    {
        ulong p = (ulong)(priceTime * (Math.Pow(priceTimeMultiplier, level)) + 0.01);
        ulong a = EvaluateIncome(level) * priceFistIncomeAddMultiplier;
        if (level == 0)
        {
            a = 0L;
        }
        else if (level >= levelCountToSecondAnd)
        {
            a = EvaluateIncome(level) * priceSecondIncomeAddMultiplier;
        }
        return p + a;
    }

    public ulong EvaluateIncome(int level)
    {
        return (ulong)((income + (ulong)level * incomeIncrease)* Math.Pow(2, Math.Floor((level + 1) / 25D)));
    }
    public float EvaluateTime(int level)
    {
        return timeToAction * (Mathf.Pow(timeMultiplier, level));
    }
    public float EvaluateDamage(int level)
    {
        return  damage + level*damageIncrease;
    }
    public int EvaluateSleep(int level)
    {
        return Mathf.RoundToInt(actionToSleep * Mathf.Pow(sleepMultiplier, level));
    }

    
    [ContextMenu("Debug Upgrades")]
    public void DebugUpgrades()
    {
        StringBuilder strB = new StringBuilder();
        strB.Append("\n");
        for (int i = 0; i < 100; i++)
        {
            strB.
                Append("    Level: ").
                Append(i+1).
                Append("    Price: ").
                Append(EvaluatePrice(i).ToString("F2")).
                Append("    Price Time: ").
                Append(EvaluatePriceTime(i).ToString("F2")).
                Append("    Income: ").
                Append(EvaluateIncome(i)).
                Append("    Boost: ").Append((int)Mathf.Pow(2f, Mathf.Floor((i+1) / 25f))).
                Append("    Income Pre Second: ").
                Append((EvaluateIncome(i)/ EvaluateTime(i)).ToString("F2")).
                Append("    Time For Upgrade: ").
                Append((EvaluatePrice(i)/(EvaluateIncome(i) / EvaluateTime(i))).ToString("F2")).
                Append("    Time: ").
                Append(EvaluateTime(i).ToString("F2")).
                Append("    Damage: ").
                Append(EvaluateDamage(i)).
                Append("\n");
        }
        Debug.Log(strB.ToString());
    }
}
