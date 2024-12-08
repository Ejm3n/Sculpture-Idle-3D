using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolUpgradeEffect : MonoBehaviour
{
    private ToolBase tool;
    [SerializeField] private EffectDynamic dynamic;
    [SerializeField] private EffectDynamic levelText;
    [SerializeField] private Transform target;
    [SerializeField] private float delay = 1f;
    [SerializeField] private float scale = 1f;
    private float nextDelay;
    private void Start()
    {
        tool = GetComponent<ToolBase>();
        tool.OnUpgrade += Spawn;
    }
    private void OnDestroy()
    {
        tool.OnUpgrade -= Spawn;
    }
    public void Spawn(int i)
    {
        var ipo = PoolManager.Default.Pop(dynamic, null);
        ipo.SetPosition(tool.transform.position);
        ipo.SetScale(new Vector3(scale, 1f, scale));
        if (Time.time >= nextDelay)
        {
            var ipo2 = PoolManager.Default.Pop(levelText, null);
            ipo2.SetPosition(target.position + Vector3.up * 0.35f);
            nextDelay = Time.time + delay;
        }

        
       // if (ipo.transform.childCount > 0)
        //    ipo.transform.GetChild(0).position = target.position+Vector3.up*0.35f;
    }
}
