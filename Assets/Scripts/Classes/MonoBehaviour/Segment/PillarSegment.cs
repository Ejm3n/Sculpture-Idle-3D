using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarSegment : MonoBehaviour
{

    private float health = 100f;
    private float maxHealth = 100f;
    private float healthPercent;
    [SerializeField] private ResourcePrefabReference fragResourcePrefab;
    [SerializeField] private Vector3 extents = Vector3.one;
    [SerializeField] private Vector3 offset = Vector3.zero;
    private bool hasHolder;
    private PillarFragHolder holder;
    private int targetCount;
    private int maxFragCount;


    public System.Action OnHurt;
    public System.Action OnDeath;
    public System.Action<Rigidbody> OnFragDetach;

    public bool IsAlive => healthPercent > 0;
    public float HealthPercent => healthPercent;
    public Vector3 Extents => extents;
    public Vector3 Offset => offset;
    public Vector3 Position => transform.position + offset;
    public float Health => health;

    public void LoadFragsFromResources()
    {
        if(!hasHolder)
        {           
            ClearChilds();
            holder = Instantiate(fragResourcePrefab.Get<PillarFragHolder>(), transform);
            maxFragCount = holder.frags.Count;           
            hasHolder = holder != null;
        }
    }
    private void ClearChilds()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }
    public void SetHeath(float health)
    {
        maxHealth = this.health = health;
        healthPercent = 1f;
    }
    public void SpecialHurt(float damage, Vector3 point)
    {
        if (hasHolder)
        {
            DetachSpecial(Damage(damage), point);
            if (Check())
                DetachMiddle();
        }
    }
    public void Hurt(float damage)
    {
        if (hasHolder)
        {
            DetachFrags(Damage(damage));
            if (Check())
                DetachMiddle();
        }
    }
    public void Hurt(float damage,Vector3 point)
    {
        if (hasHolder)
        {
            DetachFragsAtPoint(Damage(damage), point);
            if (Check())
                DetachMiddle();
        }
    }
    public void RandomHurt(float damage)
    {
        if (hasHolder)
        {
            DetachFragsAllRandom(Damage(damage));
            if (Check())
                DetachMiddle();
        }
    }
    private int Damage(float damage)
    {
            health -= damage;
            healthPercent = Mathf.Clamp01(health / maxHealth);
            targetCount = (int)(maxFragCount * healthPercent);
            return holder.frags.Count - targetCount;
    }
    private bool Check()
    {
        if (healthPercent <= 0)
        {
            OnDeath?.Invoke();
            return true;
        }
        else
        {
            OnHurt?.Invoke();
            return false;
        }
    }
    public void Kill()
    {
        Hurt(maxHealth);
    }
    private Vector3 GetOnUnitCircle()
    {
        Vector3 rand = Random.insideUnitCircle;
        rand.z = rand.y;
        rand.y = 0;
        rand.Normalize();
        return transform.position + rand * extents.x;
    }
    private void DetachFrags(int count)
    {
        if (count > 0 && holder.frags.Count > 0)
        {
            Vector3 point = GetOnUnitCircle();
            for (int i = 0; i < count; i++)
            {
                RemoveFrag(point);
            }
        }
    }
    private void DetachFragsAtPoint(int count, Vector3 point)
    {
        if (count > 0 && holder.frags.Count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                RemoveFrag(point);
            }
        }
    }
    private void DetachFragsAllRandom(int count)
    {
        if (count > 0 && holder.frags.Count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                RemoveFrag(GetOnUnitCircle());
            }
        }
    }
    private void DetachMiddle()
    {
        if (holder.middle.Count > 0)
        {
            for (int i = holder.middle.Count-1; i >= 0; i--)
            {
                RemoveMiddle(i);
            }
            holder.middle.Clear();
        }
    }
    private void DetachSpecial(int count, Vector3 point)
    {
        float sqrRadius = 0f;
        List<Rigidbody> targets = new List<Rigidbody>(targetCount);
        if (count > 0 && holder.frags.Count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                targets.Add(RemoveFrag(point));
            }
        }
        if (targets.Count > 0)
        {
            point = Vector3.zero;
            for (int i = 0; i < targets.Count; i++)
                point += targets[i].transform.position;
            point /= targets.Count;
            for (int i = 0; i < targets.Count; i++)
                sqrRadius = Mathf.Max(sqrRadius, (targets[i].transform.position - point).sqrMagnitude);
            if (sqrRadius < 1f)
                sqrRadius = 1f;
            else
                sqrRadius *= 1.5f;

            for (int i = 0; i < holder.frags.Count; i++)
            {
                if ((point - holder.frags[i].transform.position).sqrMagnitude <= sqrRadius)
                {
                    holder.frags[i].transform.localScale *= GameData.Default.specialHitScaleChange;
                }
            }
        }
    }
    private int GetClosest(Vector3 point)
    {
        int id = 0;
        Vector3 closest = holder.frags[0].transform.position;
        for (int i = 1; i < holder.frags.Count; i++)
        {
            if ((point - closest).sqrMagnitude >= (point - holder.frags[i].transform.position).sqrMagnitude)
            {
                closest = holder.frags[i].transform.position;
                id = i;
            }
        }
        return id;
    }

    private Rigidbody RemoveFrag(Vector3 point)
    {
        int id = GetClosest(point);
        Rigidbody go = holder.frags[id];
        holder.frags.RemoveAt(id);
        go.transform.SetParent(transform.parent.parent);
        OnFragDetach?.Invoke(go);
        return go;
    }
    private Rigidbody RemoveMiddle(int i)
    {
        Rigidbody go = holder.middle[i];
        go.transform.SetParent(transform.parent.parent);
        OnFragDetach?.Invoke(go);
        return go;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position + Vector3.Scale(offset,transform.localScale), extents * 2f);
    }
#endif
}
