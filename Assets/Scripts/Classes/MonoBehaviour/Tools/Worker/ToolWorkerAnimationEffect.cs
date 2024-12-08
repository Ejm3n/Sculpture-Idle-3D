using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolWorkerAnimationEffect : MonoBehaviour
{
  [SerializeField] private ToolWorkerAnimationController controller;
    [SerializeField] private EffectDynamic dynamic;
    [SerializeField] private float offset = 1.5f;

    private void Start()
    {
        controller.OnAnimationAction += Spawn;
    }
    private void OnDestroy()
    {
        controller.OnAnimationAction -= Spawn;
    }
    public void Spawn()
    {
        var ipo = PoolManager.Default.Pop(dynamic, null);
        Vector3 pos = transform.position + transform.forward * offset;
        ipo.SetPosition(pos);
    }

}
