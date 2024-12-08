using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour, PoolManager.IPoolObject
{
    [SerializeField] private string specifier = "base";
    [HideInInspector] private bool inPool;
    public string Specifier => specifier;
    public virtual bool CanPool => true;
    string PoolManager.IPoolObject.PoolTag => GetType().Name + specifier;
    bool PoolManager.IPoolObject.InPool { get => inPool; set => inPool = value; }
    GameObject PoolManager.IPoolObject.GameObject => gameObject;
    private void Awake()
    {
        LevelManager.Default.OnLevelPreLoad += OnLevelPreLoad;
    }
    private void OnDestroy()
    {
        LevelManager.Default.OnLevelPreLoad -= OnLevelPreLoad;
    }
    private void OnLevelPreLoad()
    {
        PoolManager.Default.Push(this);
    }
    public virtual void Pop()
    {
    }
    public virtual void Push()
    {
    }
    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
    public void SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
    }
}
