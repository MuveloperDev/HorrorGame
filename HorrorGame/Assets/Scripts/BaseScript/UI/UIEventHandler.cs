using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UIEventHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action<PointerEventData> onPointerDownEvent;
    public Action<PointerEventData> onPointerUpEvent;
    public Action<PointerEventData> onPointerEnterEvent;
    public Action<PointerEventData> onPointerExitEvent;
    public Action<PointerEventData> onBeginDragEvent;
    public Action<PointerEventData> onDragEvent;
    public Action<PointerEventData> onEndDragEvent;

    public virtual void OnPointerDown(PointerEventData eventData) 
        => onPointerDownEvent?.Invoke(eventData);
    public virtual void OnPointerUp(PointerEventData eventData) 
        => onPointerUpEvent?.Invoke(eventData);

    public virtual void OnPointerEnter(PointerEventData eventData) 
        => onPointerEnterEvent?.Invoke(eventData);

    public virtual void OnPointerExit(PointerEventData eventData)
        => onPointerExitEvent?.Invoke(eventData);

    public virtual void OnBeginDrag(PointerEventData eventData) 
        => onBeginDragEvent?.Invoke(eventData);

    public virtual void OnDrag(PointerEventData eventData) 
        => onDragEvent?.Invoke(eventData);

    public virtual void OnEndDrag(PointerEventData eventData) 
        => onEndDragEvent?.Invoke(eventData);

}
