using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDynamicSimpleMoveAction : EffectDynamicAction
{
    [SerializeField] private Transform target;
    private float speed = 1.0f;
    public override void Execute()
    {
        enabled = true;
    }

    public override void Stop()
    {
        enabled = false;
    }
    private void LateUpdate()
    {
        target.Translate(Vector3.up * speed * Time.deltaTime,Space.World);
    }
}
