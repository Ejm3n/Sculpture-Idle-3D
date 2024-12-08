using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace BG.UI.Main
{
    [RequireComponent(typeof(CanvasGroup), typeof(Canvas))]
    public class Panel : MonoBehaviour
    {
        [SerializeField] protected bool _hideOnStart;
        [SerializeField] protected float _animDuration = 0.25f;
        private Tween tween;
        protected CanvasGroup _group;
        protected Canvas canvas;

        public Action onPanelShow = () => { };
        public Action onPanelHide = () => { };
        protected virtual void Awake()
        {
            canvas = GetComponent<Canvas>();
            _group = GetComponent<CanvasGroup>();
            _group.alpha = !_hideOnStart ? 1f : 0f;
            canvas.enabled = enabled = !_hideOnStart;
            _group.blocksRaycasts = !_hideOnStart;

        }
        public virtual void Start()
        {
            //gameObject.SetActive(!_hideOnStart);
            if (!_hideOnStart)
                ShowPanel(false);
        }

        public virtual void ShowPanel(bool animate = true)
        {
            if (_group.blocksRaycasts == false)
            {
                onPanelShow.Invoke();
                _group.blocksRaycasts = true;
                enabled = true;
                canvas.enabled = true;
                tween?.Kill();
                if (animate)
                {
                    _group.alpha = 0f;
                    tween = DOTween.To(
                        () => 0f,
                        (v) => _group.alpha = v,
                        1f, _animDuration);
                }
                else
                {
                    
                    _group.alpha = 1.0f;
                }
            }
        }
        public virtual void HidePanel(bool animate = true)
        {
            if (_group.blocksRaycasts == true)
            {
                onPanelHide.Invoke();
                transform.localScale = Vector3.one;
                enabled = _group.blocksRaycasts = false;
                tween?.Kill();
                if (animate)
                {
                    tween = DOTween.To(
                        () => 1f,
                        (v) => _group.alpha = v,
                        0f, _animDuration)
                            .OnComplete(() => canvas.enabled =  false);
                }
                else
                {
                    _group.alpha = 0.0f;
                    canvas.enabled = false;
                }
            }
        }
    }
}