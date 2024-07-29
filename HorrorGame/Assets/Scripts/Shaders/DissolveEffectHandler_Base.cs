using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class DissolveEffectHandler_Base : MonoBehaviour, IMeshModifier
{
    //public enum ColorMode
    //{
    //    None = 0,
    //    Set,
    //    Add,
    //    Sub,
    //}

    [Header("[ RESOURCES ]")]
    [SerializeField] protected Material _effectMaterial;
    [SerializeField] protected Graphic _graphic;
    [SerializeField] protected Image _image;

    [Header("[ DISSOLVE EFFECT OPTIONS ]")]
    [SerializeField][Range(0, 1)] protected float _progress = 0.5f;
    [SerializeField][Range(0, 1)] protected float _width = 0.5f;
    [SerializeField][Range(0, 1)] protected float _softness = 0.5f;
    [SerializeField][ColorUsage(false)] protected Color _edgeColor;
    //[SerializeField][HideInInspector] protected ColorMode _colorMode = ColorMode.Add;
    [SerializeField][Range(0.25f, 3)] protected float _animationSpeed = 25;

    [Header("[ INFO ]")]
    [SerializeField] protected Color _backgroundColor;
    [SerializeField] protected Color _originEdgeColor;

    protected CancellationTokenSource _cts;
    public float progress
    {
        get { return _progress; }
        set
        {
            _progress = Mathf.Clamp(value, 0, 1); _SetDirty();
        }
    }

    protected virtual void Awake()
    {
        _cts = new();
        _cts.RegisterRaiseCancelOnDestroy(this);

        if (null == _graphic)
            _graphic = GetComponent<Graphic>();

        _image = GetComponent<Image>();
        if (null == _image)
            return;
        { 
            if (_image.material != null)
                _effectMaterial = _image.material;
            _backgroundColor = _image.color;
        }

        _originEdgeColor = _edgeColor;
    }
    protected virtual void Start() {}
    protected virtual void Update() {}

    protected void _SetDirty()
    {
        if (_graphic)
            _graphic.SetVerticesDirty();
    }

    public async UniTask DissolveIn()
    {
        progress = 1;
        _edgeColor = _originEdgeColor;
        while (progress > 0)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, _cts.Token);

            progress -= Time.deltaTime * _animationSpeed;

            if (progress <= 0)
            {
                _edgeColor = _backgroundColor;
                progress = 0f;
            }
        }
    }
    public async UniTask DissolveOut()
    {
        progress = 0;
        _edgeColor = _originEdgeColor;
        while (progress < 1)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, _cts.Token);

            progress += Time.deltaTime * _animationSpeed;

            if (progress >= 1)
            {
                _edgeColor = _backgroundColor;
                progress = 1f;
            }
        }
    }

    private float _PackToFloat(float x, float y, float z, float w)
    {
        // 셰이더에 최적화방식으로 전달하기 위해 0~1 사이의 값을 float으로 패킹하는 함수.
        const int PRECISION = (1 << 6) - 1;
        return (Mathf.FloorToInt(w * PRECISION) << 18)
        + (Mathf.FloorToInt(z * PRECISION) << 12)
        + (Mathf.FloorToInt(y * PRECISION) << 6)
        + Mathf.FloorToInt(x * PRECISION);
    }


    #region [ IMeshModifier ]
    public void ModifyMesh(VertexHelper vh)
    {
        // 메쉬의 정점을 수정
        if (!isActiveAndEnabled)
            return;

        Rect rect = _graphic.rectTransform.rect;

        UIVertex vertex = default(UIVertex);
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);

            var x = Mathf.Clamp01(vertex.position.x / rect.width + 0.5f);
            var y = Mathf.Clamp01(vertex.position.y / rect.height + 0.5f);
            vertex.uv1 = new Vector2(_PackToFloat(x, y, progress, _width), _PackToFloat(_edgeColor.r, _edgeColor.g, _edgeColor.b, _softness));

            vh.SetUIVertex(vertex, i);
        }
    }
    public void ModifyMesh(Mesh mesh)
    {
        // 메쉬 객체를 직접 수정
        using (var vh = new VertexHelper(mesh))
        {
            ModifyMesh(vh);
            vh.FillMesh(mesh);
        }
    }
    #endregion


#if UNITY_EDITOR
    protected void OnValidate()
    {
        _SetDirty();
    }
#endif
}