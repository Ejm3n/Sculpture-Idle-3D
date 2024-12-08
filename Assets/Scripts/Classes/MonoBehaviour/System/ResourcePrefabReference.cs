using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourcePrefabReference
{
    [SerializeField] private string resourcePath;
    public T Get<T>() where T : Object
    {
        GameObject go = Resources.Load(resourcePath) as GameObject;
        return go.GetComponent<T>();
    }
}
