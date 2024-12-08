using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarFragHolder : MonoBehaviour
{
    [SerializeField] private float middleRadius = 0.65f;
    public List<Rigidbody> middle = new List<Rigidbody>();
    public List<Rigidbody> frags = new List<Rigidbody>();

#if UNITY_EDITOR

    //[ContextMenu("Get Middle Frags")]
    private void EDITOR_GetMiddleFrags()
    {
        middle.Clear();
        Vector3 point = Vector3.zero;
        var children = GetComponentsInChildren<MeshFilter>();

        Vector3 closest = children[0].transform.position;

        point.y = closest.y;
        for (int i = 0; i < children.Length; i++)
        {
            point.y = children[i].transform.position.y;
            if ((point - children[i].transform.position).sqrMagnitude <= middleRadius * middleRadius)
            {
                middle.Add(children[i].GetComponent<Rigidbody>());
            }
        }
    }
    [ContextMenu("Get All Frags")]
    private void EDITOR_GetFrags()
    {
        EDITOR_GetMiddleFrags();
        EDITOR_GetDetachFrags();
    }
    [ContextMenu("Get Detach Frags")]
    private void EDITOR_GetDetachFrags()
    {
        frags.Clear();
        var children = GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < children.Length; i++)
        {
            Rigidbody rig = children[i].GetComponent<Rigidbody>();
            if (!middle.Contains(rig))
                frags.Add(rig);
        }
        UnityEditor.EditorUtility.SetDirty(this);
    }
    [ContextMenu("Select Middle Frags")]
    private void EDITOR_SelectMiddleFrags()
    {
        List<GameObject> selected = new List<GameObject>();
        Vector3 point = Vector3.zero;
        var children = GetComponentsInChildren<MeshFilter>();
        Vector3 closest = children[0].transform.position;
        point.y = closest.y;
        for (int i = 0; i < children.Length; i++)
        {
            point.y = children[i].transform.position.y;
            if ((point - children[i].transform.position).sqrMagnitude <= middleRadius * middleRadius)
            {
                selected.Add(children[i].gameObject);
            }
        }
        UnityEditor.Selection.objects = selected.ToArray();
    }
    [ContextMenu("Set Physics To All Frags")]
    private void EDITOR_SetPhysicsToAllFrags()
    {
        var children = GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < children.Length; i++)
        {
            Rigidbody rig = children[i].GetComponent<Rigidbody>();
            if (rig == null)
                rig = children[i].gameObject.AddComponent<Rigidbody>();
            rig.isKinematic = true;
            rig.angularDrag = 1f;
            SphereCollider box = children[i].GetComponent<SphereCollider>();
            if (box == null)
                box = children[i].gameObject.AddComponent<SphereCollider>();

            Vector3 s = children[i].transform.localScale;
            Vector3 ns = (Vector3.one * 0.125f);
            ns.x /= s.x;
            ns.y /= s.y;
            ns.z /= s.z;
            box.radius = ns.x;
        }
        UnityEditor.EditorUtility.SetDirty(gameObject);
    }
    [ContextMenu("Clear Physics To All Frags")]
    private void EDITOR_ClearPhysicsToAllFrags()
    {
        var rigs = GetComponentsInChildren<Rigidbody>();
        var coll = GetComponentsInChildren<Collider>();

        for (int i = 0; i < rigs.Length; i++)
        {
            DestroyImmediate(rigs[i]);
        }
        for (int i = 0; i < coll.Length; i++)
        {
            DestroyImmediate(coll[i]);
        }
    }
    [ContextMenu("Fix Y Pos")]
    private void EDITOR_FixYPos()
    {
        Vector3 pos = transform.position;
        transform.position = Vector3.zero;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);
            Vector3 tp = t.localPosition;
            tp.y += pos.y;
            t.localPosition = tp;
        }
    }
#endif
}
