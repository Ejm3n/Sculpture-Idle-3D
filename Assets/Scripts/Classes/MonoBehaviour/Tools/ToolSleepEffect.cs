using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSleepEffect : MonoBehaviour
{
    private ToolBase tool;
    [SerializeField] private ParticleSystem particle;
    private bool hasStuff;
    [SerializeField] private Transform stuff;
    private Vector3 stuffDefPos;
    private Quaternion stuffDefRot;
    private Transform stuffParent;
    private Tween tween;

    private void Start()
    {
        tool = GetComponent<ToolBase>();
        tool.OnSleep += OnSleep;
        tool.OnWakeup += OnWakeup;
        tool.OnFinish += OnSleep;
        hasStuff = stuff != null;
        if (hasStuff)
        {
            stuffParent = stuff.parent;
            stuffDefPos = stuff.localPosition;
            stuffDefRot = stuff.localRotation;
        }
    }
    private void OnDestroy()
    {
        tool.OnSleep -= OnSleep;
        tool.OnWakeup -= OnWakeup;
        tool.OnFinish -= OnSleep;
    }
    private void OnSleep()
    {
        //particle.Play();
        if (hasStuff)
        {
            Vector3 sp = stuff.position;
            Quaternion sr = stuff.rotation;
            Vector3 tp = tool.transform.position + tool.transform.forward*0.5f + tool.transform.right * Mathf.Min(tool.Extents.x, tool.Extents.z);
            Quaternion tr =
            Quaternion.LookRotation(tool.transform.forward, tool.transform.up) *
            Quaternion.AngleAxis(90f, Vector3.right);
            stuff.SetParent(transform, true);
            tween?.Kill();
            tween = DOTween.To(() => 0f, (v) =>
            {
                stuff.position = Vector3.Lerp(sp, tp, v);
                stuff.rotation = Quaternion.Lerp(sr, tr, v);
            }
            , 1f, 0.25f);
        }
    }
    private void OnWakeup()
    {
        //particle.Stop();
        //particle.Clear();
        if (hasStuff)
        {
            stuff.SetParent(stuffParent, true);

            Vector3 sp = stuff.localPosition;
            Quaternion sr = stuff.localRotation;

            tween?.Kill();
            tween = DOTween.To(() => 0f, (v) =>
            {
                stuff.localPosition = Vector3.Lerp(sp, stuffDefPos, v);
                stuff.localRotation = Quaternion.Lerp(sr, stuffDefRot, v);
            }
            , 1f, 0.25f);
        }
    }
}
