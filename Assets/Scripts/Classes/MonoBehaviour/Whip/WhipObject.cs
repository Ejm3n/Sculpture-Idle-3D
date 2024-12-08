using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipObject : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Quaternion rotation = Quaternion.identity;
    [SerializeField] private string sound;
    [SerializeField] private EffectDynamic effect;
    [SerializeField] private Animator animator;
    private int whipHash = Animator.StringToHash("Whip");
    public Vector3 Offset => offset;
    public Quaternion Rotation => rotation;
    public string Sound => sound;
    public EffectDynamic Effect => effect;
    public void Play()
    {
        animator.Play(whipHash);
    }
}
