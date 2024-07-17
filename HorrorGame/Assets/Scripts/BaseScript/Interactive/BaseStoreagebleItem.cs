using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enum;
public class BaseStoreagebleItem : BaseInteractive, IStorageableItem
{
    [Header("[ Mesh ]")]
    [SerializeField] protected MeshRenderer _mesh;

    [Header("[ Info ]")]
    [SerializeField] protected EStoragebleItemType _type;
    [SerializeField] protected Sprite _sprite; // �κ��丮 ����� ���� �̹���.

    protected override void Awake()
    {
        base.Awake();
        _mesh = GetComponent<MeshRenderer>();
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

    public BaseStoreagebleItem GetBaseStoreagebleItem() => this;
}
