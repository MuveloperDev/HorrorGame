using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventroyElement : UIEventHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private BaseStoreagebleItem _item;
    [SerializeField] private InventoryItemInfoData _inventoryItemInfoData;

    public void Initailize(BaseStoreagebleItem item)
    {
        _item = item;
        var itemDefinitionData = _item.GetItemDefinitionData();
        if (null == itemDefinitionData)
            return;
        _inventoryItemInfoData = InventoryItemInfoData.table[itemDefinitionData.Ref_InventoryItemInfo];
        if (null == _inventoryItemInfoData)
            return;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        //Debug.Log();
    }
}
