using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIActionProgress : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image progress;
    [SerializeField] private Ease ease = Ease.OutElastic;
    [SerializeField] private bool useGradient;
    [SerializeField] private Gradient gradient;
    //[SerializeField] private MoneyBlastAnimation moneyBlast;
    private Tween tween;
    private bool inAnim;
    public Canvas Canvas => canvas;
    private void Start()
    {
        progress.fillAmount = 0f;

    }
    
    public void SetProgress(float percent)
    {
            progress.fillAmount  = percent;
        if (useGradient)
            progress.color = gradient.Evaluate(percent);
    }
    public void PlayBubble()
    {
        tween?.Kill();
        tween = DOTween.To(() => 0f,
            (v) =>
            {
                transform.localScale = Vector3.LerpUnclamped(Vector3.one * 0.5f, Vector3.one, v);
            },
            1f, 0.75f).SetEase(ease, 0.25f);
       // moneyBlast.Play();
    }


    public void PlayIdle()
    {
        if (!inAnim)
        {
            tween?.Kill();
            transform.localScale = Vector3.one;
            tween = transform.DOScale(1.2f, 0.5f).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
            inAnim = true;
        }
    }
    public void Stop()
    {
        if (inAnim)
        {
            tween?.Kill();
            transform.DOScale(1f, 0.25f);
            inAnim = false;

        }
    }
}
