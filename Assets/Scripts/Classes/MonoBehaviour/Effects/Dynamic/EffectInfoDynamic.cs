using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffectInfoDynamic : EffectDynamic
{
    [SerializeField] private TextMeshPro info;
    [SerializeField, TextArea] private string preText = "+", afterText = "<sprite index=0>";

    public void SetText(string text)
    {
        info.SetText($"{preText}{text}{afterText}");
    }
}
