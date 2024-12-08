using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UITutorialBase : MonoBehaviour
{
    
    private bool isActive;
    protected UITutorialCursor cursor;
    [SerializeField] private float timeToRepeat = 5f;
    private float nextRepeat;
    public bool IsActive => isActive;

    public virtual void Init(UITutorialCursor cursor)
    {
        this.cursor = cursor;
    }
    public abstract bool ShowCondition();
    public abstract void Select();
    public abstract void Deselect();
    public virtual void Show()
    {
        isActive = true;
    }
    public virtual void Hide()
    {
        isActive = false;
        nextRepeat = timeToRepeat + Time.time;
    }
    public virtual void Repeat()
    {
        if (!isActive && Time.time >= nextRepeat)
        {
            Show();
        }
    }
    private void LateUpdate()
    {
        Repeat();
    }
}
