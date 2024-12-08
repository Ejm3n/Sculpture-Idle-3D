using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarObject : MonoBehaviour
{
    private bool isAlive = true;
    private bool damageable = true;
    [SerializeField] private float maxHealth = 5000;
    private float percent = 1f;
    [SerializeField] private PillarFragDetachHandler detachHandler;
    [SerializeField] private List<PillarSegment> segments = new List<PillarSegment>();
    private int activeSegment;
    private float segmentHeath;
    public System.Action OnPillarFinish;
    public System.Action OnHurt;
    public System.Action<Vector3,Vector3> OnSegmentPositionChange;
    public int ActiveSegmentId => activeSegment;
    public bool IsAlive => isAlive;
    public PillarSegment ActiveSegment => segments[activeSegment];
    public float Percent => percent;
    public virtual bool Damageable
    {
        get => damageable;
        set => damageable = value;
    }
    public void Init()
    {
        activeSegment = 0;
        SetupSegments();
        detachHandler.SetPillar(this);
    }
    public Vector3 GetTopSegmentPosition()
    {
        return segments[segments.Count - 1].Position;
    }
    public Vector3 GetBottomPosition()
    {
        return segments[0].Position - Vector3.up * segments[0].Extents.y;
    }
    private void SetupSegments()
    {
        if (segments.Count > 0)
        {
            segmentHeath = maxHealth / segments.Count;

            for (int i = 0; i < segments.Count; i++)
            {
                //segments[i].Init();
                segments[i].SetHeath(segmentHeath);
            }
            //segments[0].LoadFragsFromResources();
        }
    }
    public void SetSegmentAndHealth(int segment ,float health)
    {
        Vector3 offset = Vector3.up * (segments[activeSegment].Offset.y - segments[segment].Offset.y);
        Vector3 delta = segments[activeSegment].transform.position - segments[segment].transform.position;
        Vector3 target = transform.position + delta + offset;
        transform.position = target;
        if (activeSegment != segment)
            segments[activeSegment].gameObject.SetActive(false);
        activeSegment = segment;       
        segments[activeSegment].LoadFragsFromResources();
        if(health > 0f)
        segments[activeSegment].RandomHurt(segmentHeath - health);

        SetPercent();

    }


    public bool SpecialHurt(float damage, Vector3 point)
    {
        if (damageable)
        {
            var segment = segments[activeSegment];
            segment.SpecialHurt(damage, point);
            if (segment.HealthPercent <= GameData.Default.pillarSegmentHealthPercentToChange)
            {
                segment.Kill();
                ChangeSegment();
            }
            else
                OnHurt?.Invoke();
            return true;
        }
        return false;
    }

    internal void AddHealth(float v)
    {
        maxHealth = GameData.Default.pillarHealth + v;
    }

    public void Hurt(float damage)
    {
        if (damageable)
        {
            var segment = segments[activeSegment];
            segment.Hurt(damage);
            if (segment.HealthPercent <= GameData.Default.pillarSegmentHealthPercentToChange)
            {
                segment.Kill();
                ChangeSegment();
            }
            else
            OnHurt?.Invoke();
            SetPercent();
        }
    }
    public void Hurt(float damage,Vector3 point)
    {
        if (damageable)
        {
            var segment = segments[activeSegment];
            segment.Hurt(damage,point);
            if (segment.HealthPercent <= GameData.Default.pillarSegmentHealthPercentToChange)
            {
                segment.Kill();
                ChangeSegment();
            }
            else
                OnHurt?.Invoke();
            SetPercent();
        }
    }
    private void SetPercent()
    {
        float hp = 0f;
        for (int i = activeSegment; i < segments.Count; i++)
        {
            hp += segments[i].Health;
        }
        percent = Mathf.Clamp01(hp / maxHealth);

    }
    public void ChangeSegment()
    {
        int prev = activeSegment;
        activeSegment = Mathf.Min(activeSegment + 1,segments.Count-1);

        if (prev != activeSegment)
        {

            segments[activeSegment].LoadFragsFromResources();
            OnSegmentPositionChange?.Invoke(
                segments[prev].transform.position + Vector3.up * segments[prev].Offset.y,
                segments[activeSegment].transform.position + Vector3.up * segments[activeSegment].Offset.y);
        }
        else
        {
            isAlive = false;
            OnPillarFinish?.Invoke();
        }
    }

    #region Delegate
    public void BindSegmentFragDetach(System.Action<Rigidbody> action)
    {
        for (int i = 0; i < segments.Count; i++)
            segments[i].OnFragDetach += action;
    }
    public void UnbindSegmentFragDetach(System.Action<Rigidbody> action)
    {
        for (int i = 0; i < segments.Count; i++)
            segments[i].OnFragDetach -= action;
    }
    public void BindSegmentHurt(System.Action action)
    {
        for (int i = 0; i < segments.Count; i++)
            segments[i].OnHurt += action;
    }
    public void UnbindSegmentHurt(System.Action action)
    {
        for (int i = 0; i < segments.Count; i++)
            segments[i].OnHurt -= action;
    }
    public void BindSegmentDeath(System.Action action)
    {
        for (int i = 0; i < segments.Count; i++)
            segments[i].OnDeath += action;
    }
    public void UnbindSegmentDeath(System.Action action)
    {
        for (int i = 0; i < segments.Count; i++)
            segments[i].OnDeath -= action;
    }
    #endregion


#if UNITY_EDITOR
    private void OnValidate()
    {
        EDITOR_SortSegments();
    }
    private void EDITOR_SortSegments()
    {
        if (segments != null && segments.Count >= 1) 
        {
            if (segments[0] == null)
                return;
            Vector3 offset = Vector3.up * 
                (segments[0].Extents.y - 
                segments[0].Offset.y* segments[0].transform.localScale.y);
            Vector3 shift = segments[0].transform.position;
            shift.y = 0;
            segments[0].transform.position = transform.position +offset+ shift;
            if (segments.Count >= 2)
            {
                for (int i = 1; i < segments.Count; i++)
                {
                    if (segments[i] == null)
                        return;
                    shift = segments[i].transform.position;
                    shift.y = 0;
                    offset += Vector3.up * 
                        (segments[i].Extents.y - 
                        segments[i].Offset.y * segments[i].transform.localScale.y + 
                        segments[i - 1].Extents.y+
                        segments[i - 1].Offset.y * segments[i-1].transform.localScale.y);
                    segments[i].transform.position = transform.position + offset + shift;
                }
            }
        }
    }
#endif
}
