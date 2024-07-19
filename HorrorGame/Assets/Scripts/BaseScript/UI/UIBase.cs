using Cysharp.Threading.Tasks;
using Enum;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

#pragma warning disable CS1998 
public class UIBase : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private EUICanvasLayer _layer = EUICanvasLayer.None;

    protected CancellationTokenSource _cts;
    public bool isActive { get; protected set; } = false;

    protected virtual void Awake() { }
    protected virtual void Start() { }
    protected virtual void Update() { }
    protected virtual void OnDestroy() { }

    protected async virtual UniTask BeforeShow()
    { }
    protected virtual void AfterShow()
    { }
    public virtual async void Show()
    {
        if (true == gameObject.activeSelf)
            return;
        await BeforeShow();
        gameObject.SetActive(true);
        isActive = true;
        AfterShow();
    }
    protected async virtual UniTask BeforeHide()
    { }
    protected virtual void AfterHide()
    { }
    public virtual async void Hide()
    {
        if (false == gameObject.activeSelf)
            return;
        await BeforeHide();
        gameObject.SetActive(false);
        isActive = false;
        AfterHide();
    }

    public void SetLayer(EUICanvasLayer argLayer) => _layer = argLayer;
    public EUICanvasLayer GetLayer() => _layer;
}
#pragma warning restore CS1998