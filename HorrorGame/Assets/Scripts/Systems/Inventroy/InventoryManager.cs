using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<InventroyElement> _items;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Acquire(IStorageableItem item)
    {

        Debug.Log($"[{item.GetBaseStoreagebleItem().gameObject.name}] - Acquire!");
    }

    private void AddItem(BaseStoreagebleItem item)
    {
        InventroyElement element = new();
        _items.Add(element);
        
        element.Initailize(item);
    }

   // private void RemoveItem();
}
