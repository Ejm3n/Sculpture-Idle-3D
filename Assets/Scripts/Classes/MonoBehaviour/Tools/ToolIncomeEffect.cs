using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolIncomeEffect : MonoBehaviour
{
    private ToolBase tool;
    [SerializeField] private EffectInfoDynamic dynamic;

    private void Start()
    {
        tool = GetComponent<ToolBase>();
        tool.OnAction += OnAction;
    }
    private void OnDestroy()
    {
        tool.OnAction -= OnAction;

    }
    private void OnAction()
    {
        var ipo = PoolManager.Default.Pop(dynamic, null) as EffectInfoDynamic;
        ipo.SetPosition(tool.transform.position + Vector3.up * tool.Extents.y * 2.75f);
        ipo.SetText(MoneyService.AmountToString(tool.Property.Income));
    }
}
