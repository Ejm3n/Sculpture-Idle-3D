using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolBase : MonoBehaviour
{
    protected bool inAction;
    protected bool inSleep,prepareWakeup;
    protected bool canSleep = true;
    protected PillarObject pillar;
    [SerializeField] protected ToolProperty property;
    private int globalIndex;
    private int index;
    private float nextAction;
    private int curretnActionCount;
    private float nextSleep;
    private float duration;
    protected int specialHitCount = 0;

    [SerializeField] private Vector3 extents;
    [SerializeField] private Vector3 center;

    public System.Action<int> OnUpgrade;
    public System.Action OnFinish;
    public System.Action OnSleep;
    public System.Action OnWakeup;
    public System.Action OnAction;
    public ToolProperty Property => property;
    public int Level => property.CurrentLevel(index);
    public int Index { get => index; }
    public int GlobalIndex { get => globalIndex; }
    public bool InAction => inAction;
    public bool InSleep => inSleep;
    public bool InPrepareWakeup => prepareWakeup;
    public bool CanSleep { get => canSleep; set => canSleep = value; }
    public PillarObject Pillar => pillar;
    public Vector3 Extents => extents;
    public Vector3 Center => center;

#if UNITY_EDITOR
    [ContextMenu("Get Extents")]
    private void EDITOR_GetExtents()
    {
        SkinnedMeshRenderer smr = GetComponentInChildren<SkinnedMeshRenderer>();
        if(smr != null)
        {
            Bounds b = smr.bounds;
            if (!extents.Equals(b.extents) || !center.Equals(b.center))
            {
                extents = b.extents;
                center = b.center;

                UnityEditor.EditorUtility.SetDirty(this);
            }
            
        }
    }
#endif
    public virtual float GetActionProgress()
    {
        return Mathf.Clamp01(1f - (nextAction - Time.time) / duration);
    }
    public virtual float GetSleepProgress()
    {
        return Mathf.Clamp01(1f - (nextSleep - Time.time) / (property.ActionToSleep * property.TimeToAction));
    }
    public virtual void Setup(PillarObject pillar, ToolUpgrade upgrade, int index, int globalIndex)
    {
        this.pillar = pillar;
        this.index = index;
        this.globalIndex = globalIndex;
        property.SetUpgrade(upgrade);
        property.Restore(index);
        duration = property.TimeToAction;
    }

    protected virtual bool CanAction()
    {
        return Time.time >= nextAction && inAction && !inSleep;
    }
    protected void ResetActionTimer()
    {
        duration = property.TimeToAction;
        nextAction = Time.time + duration;
    }
    protected void SetActionTimer(float time = 0f)
    {
        duration = time;
        nextAction = Time.time + duration;
    }
    protected virtual void Action()
    {
        if (!inSleep)
        {
            pillar.Hurt(property.Damage);
            MoneyService.Default.AddMoney(property.Income);
            OnAction?.Invoke();
            ResetActionTimer();
            curretnActionCount++;
            if(Sleep())
                inAction = false;
        }
    }
    protected virtual void SpecialAction()
    {
        if (specialHitCount < GameData.Default.maxSpecialHitCount)
        {
            if(pillar.SpecialHurt(property.Damage, SpecialActionPosition()))
            specialHitCount++;
        }
    }
    protected virtual Vector3 SpecialActionPosition()
    {
        return transform.position + Vector3.up * 1.25f;
    }
    public virtual void Upgrade()
    {
        OnUpgrade?.Invoke(property.Upgrade(index));
    }
    public virtual void UpgradeTime()
    {
        OnUpgrade?.Invoke(property.UpgradeTime(index));

        float p = curretnActionCount / property.ActionToSleep;

        nextSleep = Time.time + (property.ActionToSleep * property.TimeToAction)* (1f-p);

    }
    public virtual void Install()
    {
        Execute();
    }
    public virtual void Execute()
    {
        inAction = true;
        ResetActionTimer();
        curretnActionCount = 0;
        nextSleep = Time.time + property.ActionToSleep * property.TimeToAction;

    }
    public virtual void Finish()
    {
        inAction = false;
        OnFinish?.Invoke();
    }

    public virtual void Wakeup()
    {
        if (inSleep)
        {
            inSleep = false;
            //prepareWakeup = false;
            curretnActionCount = 0;
            nextSleep = Time.time + property.ActionToSleep * property.TimeToAction;
            OnWakeup?.Invoke();
            Execute();
        }
    }
    public void PrepareWakeup(bool a = true)
    {
        prepareWakeup = a;
    }
    public void RestartSleep()
    {
        curretnActionCount = 0;
    }
    protected virtual bool Sleep()
    {
        bool lastSleep = inSleep;
        inSleep = curretnActionCount >= property.ActionToSleep && property.CanSleep && canSleep;
        if (lastSleep != inSleep)
            OnSleep?.Invoke();
        return inSleep;
    }
    private void LateUpdate()
    {
        if(CanAction())
        {
            Action();
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(center, extents * 2f);
        }
    }
}
