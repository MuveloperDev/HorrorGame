using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventroyElement : UIEventHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private BaseStoreagebleItem _item;

    public void Initailize(BaseStoreagebleItem item)
    {
        _item = item;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
}
