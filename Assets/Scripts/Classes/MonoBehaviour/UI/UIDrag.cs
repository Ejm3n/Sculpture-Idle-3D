using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{

    private bool isTouch;
    private float downTime, clickTime;
    private Vector2 delta, startPosition, position,upPosition;
    public bool IsTouch { get => isTouch; }
    public Vector2 Delta { get => delta; }
    public Vector2 Position { get => position; }
    public Vector2 UpPosition { get => upPosition; }
    public float ClickTime => clickTime;
    public Action PointerDown;
    public Action PointerUp;
    //public Action EndDrag;
    private void OnDisable()
    {
        isTouch = false;
        startPosition = delta = position = Vector2.zero;
    }
    public void ResetDelta()
    {
        startPosition = position;
        delta = Vector2.zero;
    }
    public void OnDrag(PointerEventData eventData)
    {
        position = eventData.position;
        delta = position - startPosition;

        delta.x /= Screen.width;
        delta.y /= Screen.height;

    }
    public void OnEndDrag(PointerEventData eventData)
    {
        //startPosition = delta = position = Vector2.zero;
        //EndDrag?.Invoke();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isTouch = true;
        startPosition = position = eventData.position;
        downTime = Time.time;
        PointerDown?.Invoke();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isTouch = false;
        position = Vector2.zero;
        upPosition = eventData.position;
        clickTime = Time.time - downTime;
        PointerUp?.Invoke();

    }
}
