using Cysharp.Threading.Tasks;
using Enum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS1998 
public class UIBase : MonoBehaviour
{
    [SerializeField] private UILayer _layer = UILayer.None;

    public bool isActive { get; private set; } = false;

    protected async virtual UniTask BeforeShow()
    { }
    protected virtual void AfterShow()
    { }
    public async void Show()
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
    public async void Hide()
    {
        if (false == gameObject.activeSelf)
            return;
        //await BeforeHide();
        gameObject.SetActive(false);
        isActive = false;
        AfterHide();
    }

    public void SetLayer(UILayer argLayer) => _layer = argLayer;
    public UILayer GetLayer() => _layer;
}
#pragma warning restore CS1998