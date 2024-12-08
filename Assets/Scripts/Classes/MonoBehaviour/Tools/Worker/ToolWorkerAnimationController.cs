using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolWorkerAnimationController : ToolWorkerAnimationControllerBase
{
    [SerializeField] private Animator animator;
    //[SerializeField] private AnimationClip actionClip;
    //[SerializeField] private AnimationClip appearClip;
    //private float speed;
    [SerializeField] private float actionSpeedScale = 1.0f;
    private int speedHash = Animator.StringToHash("Speed");
    private int idleHash = Animator.StringToHash("Idle");
    private int actionHash = Animator.StringToHash("Action");
    private int appearHash = Animator.StringToHash("Appear");
    private int winHash = Animator.StringToHash("Win");
    private int sleepHash = Animator.StringToHash("Sleep");
    public System.Action OnAnimationAction;



    private void Start()
    {
        animator.SetFloat(speedHash, actionSpeedScale);
    }
    public void PlayAction()
    {
        animator.CrossFade(actionHash,0.1f);
    }
    public void PlayIdle()
    {
        animator.Play(idleHash);
    }   
    public void PlayAppear()
    {
        animator.Play(appearHash);
    }
    public void PlayWin()
    {
        animator.CrossFade(winHash, 0.1f);
    }
    public void PlaySleep()
    {
        animator.CrossFade(sleepHash, 0.25f);
    }

    protected override void Message(string message)
    {
        if (message.Equals("action"))
            OnAnimationAction?.Invoke();
    }
}
