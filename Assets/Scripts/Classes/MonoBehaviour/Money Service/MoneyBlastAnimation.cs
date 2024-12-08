using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyBlastAnimation : MonoBehaviour
{

    protected class Target
    {
        private PoolManager.IPoolObject target;
        private Transform transform;
        private Vector2 momentum;
        private float gravityScale;

        public Target(PoolManager.IPoolObject target, Vector2 momentum,float gSacle)
        {
            this.target = target;
            this.momentum = momentum;
            transform = this.target.GameObject.transform;
            gravityScale = gSacle;
        }

        public void Update()
        {
            transform.Translate(momentum*Time.deltaTime);
            momentum += Physics2D.gravity * gravityScale * Time.deltaTime;
        }
        public void Clear()
        {
            PoolManager.Default.Push(target);
        }
    }

    private bool inAction;
    [SerializeField] private PoolObject moneyPrefab;
    [SerializeField] private float force = 1f;
    [SerializeField] private int count = 15;
    [SerializeField] private float timeToClear = 4f;
    [SerializeField] private float gravityScale = 10f;
    private float nextClear;
    private List<Target> targets = new List<Target>();
    [ContextMenu("Play")]
    public void Play()
    {
        if (inAction)
        {
            Clear();
        }
        for (int i = 0; i < count; i++)
        {
            Vector2 force = Random.insideUnitCircle * this.force;
            force.y = Mathf.Abs(force.y);
            targets.Add(new Target(PoolManager.Default.Pop(moneyPrefab,transform), force, gravityScale));
        }
        nextClear = timeToClear + Time.time;
        inAction = true;
        enabled = true;
    }
    private void Clear()
    {
        for (int i = 0; i < targets.Count; i++)
            targets[i].Clear();
        targets.Clear();
        inAction = false;
        enabled = false;
    }
    private void LateUpdate()
    {
        if (inAction)
        {
            for (int i = 0; i < targets.Count; i++)
                targets[i].Update();
            if (Time.time >= nextClear)
                Clear();
        }
        else
            enabled = false;
    }
}
