using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enum;
using Unity.VisualScripting;
public class BaseStoreagebleItem : BaseInteractive, IStorageableItem
{
    [Header("[ Mesh ]")]
    [SerializeField] protected MeshRenderer _mesh;

    [Header("[ Info ]")]
    [SerializeField] protected EStoragebleItemType _type;
    [SerializeField] protected Sprite _sprite; // �κ��丮 ����� ���� �̹���.
    public int itemDifinitionId = 1;

    private ItemDefinitionData _itemDefinitionData;
    protected override void Awake()
    {
        base.Awake();
        _mesh = GetComponent<MeshRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        _itemDefinitionData = ItemDefinitionData.table[itemDifinitionId];
    }
    public virtual void Store()
    {
        if (!_mesh)
            return;

        gameObject.SetActive(false);
        //_mesh.gameObject.SetActive(false);
        //PlaySoundOneshot(SoundsType.Store, OnEventStore);

        Debug.Log($"[{gameObject.name}] - Store!");
    }

    public virtual void OnEventStore()
    {
        gameObject.SetActive(false);
        // ���⼭ Ǯ�� �Ǵ� ����.
    }
    public override void OnPointerEnter()
    {
        base.OnPointerEnter();
    }

    public override void OnPointerExit()
    {
        base.OnPointerExit();
    }

    public ItemDefinitionData GetItemDefinitionData()
    {
        if (null == _itemDefinitionData)
        {
            Debug.LogError("Item definition data is null");
            return null;
        }
        return _itemDefinitionData;
    }
    public BaseStoreagebleItem GetBaseStoreagebleItem() => this;
}
