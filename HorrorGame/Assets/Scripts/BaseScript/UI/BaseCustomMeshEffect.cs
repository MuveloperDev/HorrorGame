using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public abstract class BaseCustomMeshEffect : MonoBehaviour, IMeshModifier
{
    [NonSerialized]
    private Graphic _graphic;

    protected Graphic graphic
    {
        get
        {
            if (_graphic == null)
                _graphic = GetComponent<Graphic>();
            return _graphic;
        }
    }
    // �޽��� ��Ƽ���·� �����Ǹ� ���� �����ӿ��� �޽��� �ٽ� �����ϰ� ������ �Ѵ�.
    // �׷��� ����� �ð��� ��ȭ�� ȭ�鿡 �ݿ��� �� �ִ�.
    protected virtual void OnEnable()
    {
        if (graphic != null)
            graphic.SetVerticesDirty();
    }

    protected virtual void OnDisable()
    {
        if (graphic != null)
            graphic.SetVerticesDirty();
    }

    protected virtual void OnDidApplyAnimationProperties()
    {
        if (graphic != null)
            graphic.SetVerticesDirty();
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (graphic != null)
            graphic.SetVerticesDirty();
    }
#endif
    public void ModifyMesh(Mesh mesh)
    {
        using (var vh = new VertexHelper(mesh))
        {
            ModifyMesh(vh);
            vh.FillMesh(mesh);
        }
    }

    public abstract void ModifyMesh(VertexHelper verts);

}
