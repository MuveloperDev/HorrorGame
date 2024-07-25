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
    // 메쉬가 더티상태로 설정되면 다음 프레임에서 메쉬를 다시 빌드하고 렌더링 한다.
    // 그래픽 요소의 시각적 변화가 화면에 반영될 수 있다.
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
