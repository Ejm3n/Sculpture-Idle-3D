﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MoneyAnimation : MonoBehaviour
{

    [SerializeField] private RectTransform _moneyTarget;
    [SerializeField] private RectTransform _arcAnchor;
    [SerializeField] private PoolObject moneyPrefab;
    [Space]
    [SerializeField] private AnimationCurve _scaleOverLifetime;
    [SerializeField] private AnimationCurve _alphaOverLifetime;

    public Action onAnimCompleted;
    public RectTransform MoneyTarget => _moneyTarget;
    public void PlayMoneyUpAnim(Vector3 position, float scale, Action onComplete, bool worldPosition = true, bool arcTrajectory = false)
    {
        int seed = UnityEngine.Random.Range(0, 10000);

        PoolObject money = PoolManager.Default.Pop(moneyPrefab,transform) as PoolObject;
        money.SetParent(transform);
        money.transform.localScale = Vector3.zero;
        money.transform.SetParent(_moneyTarget);
        money.transform.position = worldPosition ? Camera.main.WorldToScreenPoint(position) : position;
        money.transform.DOScale(Vector3.one * scale, 0.25f).SetEase(Ease.OutBack);

        Vector2 startLocalPos = money.transform.localPosition;
        Vector2 arcAnchorPosition = _moneyTarget.InverseTransformPoint(_arcAnchor.position);
        var animTween = DOTween.To(
            () => 0f,
            (v) =>
            {
                Vector2 pos = Vector2.zero;
                if (arcTrajectory)
                    pos = QuadraticLerp(startLocalPos, arcAnchorPosition, Vector2.zero, v);
                else
                    pos = Vector2.Lerp(startLocalPos, Vector3.zero, v);
                Vector2 perp = Vector2.Perpendicular(-startLocalPos);
                float noiseValue = (Mathf.PerlinNoise(Time.time * 1.2f, seed) - 0.5f);
                noiseValue *= v <= 0.5f ? (v * 2f) : (1f - ((v - 0.5f) * 2f));
                pos += perp * noiseValue;
                money.transform.localPosition = pos;
                money.transform.localScale = Vector3.one * _scaleOverLifetime.Evaluate(v) * scale;
                //money.color = new Color(money.color.r, money.color.g, money.color.b, _alphaOverLifetime.Evaluate(v));
            },
            1f, 1f)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    money.transform.DOScale(Vector3.one *scale * 0.25f, 0.25f).SetEase(Ease.InBack)
                        .OnComplete(() =>
                        {
                            onAnimCompleted?.Invoke();
                            onComplete?.Invoke();
                            //money.gameObject.SetActive(false);
                            //coins.Enqueue(money);
                            //Destroy(money.gameObject);
                            PoolManager.Default.Push(money);
                            DoDing();
                        });
                });
    }

    private void DoDing()
    {
        SoundHolder.Default.PlayFromSoundPack("CoinDing");
    }
    private Vector2 QuadraticLerp(Vector2 A, Vector2 B, Vector2 C, float t)
    {
        Vector2 AB = Vector2.Lerp(A, B, t);
        Vector2 BC = Vector2.Lerp(B, C, t);
        return Vector2.Lerp(AB, BC, t);
    }
}