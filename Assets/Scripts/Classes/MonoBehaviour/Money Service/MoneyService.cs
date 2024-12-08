using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyService : MonoBehaviour
{
    #region Singleton
    private static MoneyService _default;
    public static MoneyService Default => _default;
    #endregion

    public Action OnMoneyChanged;

    private void Awake()
    {
        _default = this;
    }
    public static string AmountToString(ulong amount)
    {
        string number = amount.ToString();

        if (number.Length >= 19)
        {
            number = number.Remove(number.Length - 16) + "S";
        }
        else if (number.Length >= 16)
        {
            number = number.Remove(number.Length - 13) + "Q";
        }
        else if (number.Length >= 13)
        {
            number = number.Remove(number.Length - 10) + "T";
        }
        else if (number.Length >= 10)
        {
            number = number.Remove(number.Length - 7) + "B";
        }
        else if (number.Length >= 7)
        {
            number = number.Remove(number.Length - 4) + "M";
        }
        else if (number.Length >= 4)
        {
            number = number.Remove(number.Length - 1) + "K";
        }
        else
            return number;
        number = number.Insert(number.Length - 3, ",");
        if (number[number.Length - 2].Equals('0'))
            number = number.Remove(number.Length - 2,1);
        if (number[number.Length - 2].Equals('0'))
            number = number.Remove(number.Length - 3,2);

        return number;
    }
    public static string NumberToString(ulong amount)
    {
        string number = amount.ToString();

        if (number.Length >= 19)
        {
            number = number.Remove(number.Length - 18) + "S";
        }
        else if (number.Length >= 16)
        {
            number = number.Remove(number.Length - 15) + "Q";
        }
        else if (number.Length >= 13)
        {
            number = number.Remove(number.Length - 12) + "T";
        }
        else if (number.Length >= 10)
        {
            number = number.Remove(number.Length - 9) + "B";
        }
        else if (number.Length >= 7)
        {
            number = number.Remove(number.Length - 6) + "M";
        }
        else if (number.Length >= 4)
        {
            number = number.Remove(number.Length - 3) + "K";
        }
        else
            return number;
        return number;
    }
    public ulong GetMoney() 
    {
        return ulong.Parse(PlayerPrefs.GetString("MoneyCount", "0"));
    }

    public void AddMoney(ulong count)
    {
        ulong gm = GetMoney();
        ulong money = gm + count;
        PlayerPrefs.SetString("MoneyCount", money.ToString());
        OnMoneyChanged?.Invoke();
    }

    public void SpendMoney(ulong count)
    {
        ulong gm = GetMoney();
        ulong money = 0L;
        if (gm >= count)
        {
            money = gm - count;
            //if (money >= 1844674407370955161L)
            //{
            //    Debug.Log($"money {gm} - count {count} = {gm - count} ");
            //    Debug.Break();
            //}
        }
        PlayerPrefs.SetString("MoneyCount", money.ToString());
        OnMoneyChanged?.Invoke();
    }


    [ContextMenu("Add Money - 100")]
    private void AddMoney100() 
    {
        AddMoney(100);
    }

    [ContextMenu("Add Money - 1000")]
    private void AddMoney1000()
    {
        AddMoney(1000);
    }
}
