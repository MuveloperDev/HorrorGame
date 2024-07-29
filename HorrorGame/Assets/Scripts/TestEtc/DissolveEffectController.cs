using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class DissolveEffectController : MonoBehaviour, IMeshModifier
{
    public enum ColorMode
    {
        None = 0,
        Set,
        Add,
        Sub,
    }

    [SerializeField][Range(0, 1)] float m_Location = 0.5f;
    [SerializeField][Range(0, 1)] float m_Width = 0.5f;
    [SerializeField][Range(0, 1)] float m_Softness = 0.5f;
    [SerializeField][ColorUsage(false)] Color m_Color = new Color(0f, 0f, 0f);
    [SerializeField][HideInInspector] ColorMode m_ColorMode = ColorMode.Add;
    [SerializeField] Material m_EffectMaterial;
    [Range(0.25f, 3)] public float animationSpeed = 25;
    public bool mainPanelMode;

    public float dissolveAmount = 0.5f;
    private Material material;

    public Graphic graphic;

    public float location
    {
        get { return m_Location; }
        set
        {
            m_Location = Mathf.Clamp(value, 0, 1); _SetDirty();
        }
    }
    void Start()
    {
        if (null == graphic)
        {
            graphic = GetComponent<Graphic>();
        }
        Image image = GetComponent<Image>();
        if (image.material != null)
        {
            material = image.material;
        }
    }

    public void DissolveUpdate()
    {
        if (material != null)
        {
            material.SetFloat("_DissolveAmount", dissolveAmount);
        }
    }

    public void ModifyMesh(VertexHelper vh)
    {
        if (!isActiveAndEnabled)
            return;

        Rect rect = graphic.rectTransform.rect;

        UIVertex vertex = default(UIVertex);
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);

            var x = Mathf.Clamp01(vertex.position.x / rect.width + 0.5f);
            var y = Mathf.Clamp01(vertex.position.y / rect.height + 0.5f);
            vertex.uv1 = new Vector2(_PackToFloat(x, y, location, m_Width), _PackToFloat(m_Color.r, m_Color.g, m_Color.b, m_Softness));

            vh.SetUIVertex(vertex, i);
        }
    }
    public void ModifyMesh(Mesh mesh)
    {
        using (var vh = new VertexHelper(mesh))
        {
            ModifyMesh(vh);
            vh.FillMesh(mesh);
        }
    }

    public void _SetDirty()
    {
        if (graphic)
            graphic.SetVerticesDirty();
    }

    private void OnValidate()
    {
        _SetDirty();
        Debug.Log("Update");
    }

    static float _PackToFloat(float x, float y, float z)
    {
        const int PRECISION = (1 << 8) - 1;
        return (Mathf.FloorToInt(z * PRECISION) << 16)
        + (Mathf.FloorToInt(y * PRECISION) << 8)
        + Mathf.FloorToInt(x * PRECISION);
    }

    // 셰이더에 최적화방식으로 전달하기 위해 0~1 사이의 값을 float으로 패킹하는 함수.
    float _PackToFloat(float x, float y, float z, float w)
    {
        const int PRECISION = (1 << 6) - 1;
        return (Mathf.FloorToInt(w * PRECISION) << 18)
        + (Mathf.FloorToInt(z * PRECISION) << 12)
        + (Mathf.FloorToInt(y * PRECISION) << 6)
        + Mathf.FloorToInt(x * PRECISION);
    }



    



}

#if UNITY_EDITOR
[CustomEditor(typeof(DissolveEffectController))]
public class DissolveEffectControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DissolveEffectController script = (DissolveEffectController)target;

        script.dissolveAmount = EditorGUILayout.Slider("Dissolve Amount", script.dissolveAmount, 0, 1);
        if (GUILayout.Button("Update Dissolve"))
        {
            script.DissolveUpdate();
            script._SetDirty();
        }
    }
}
#endif