using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractive
{
    public void Interaction();
    public void OnPointerEnter();
    public void OnPointerExit();
    public BaseInteractive GetBaseInteractive();
}

public interface IStorageableItem
{
    public void Store();
    public BaseStoreagebleItem GetBaseStoreagebleItem();
}

public interface IInventoryItem
{
    
}