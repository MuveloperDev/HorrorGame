using Enum;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerTemplate<T> : Singleton<T> where T : new()
{
    public bool isInitialized { get; private set; } = false;

    private Dictionary<ResourceScope, Dictionary<System.Type, GameObject>> _resourcesUI = new();
    private Dictionary<UILayer, RectTransform> _canvases = new();

    protected string canvasName = string.Empty;
    protected override void InitializeTemplate()
    {
        foreach (var scope in System.Enum.GetValues(typeof(ResourceScope)))
        {
            if ((ResourceScope)scope is ResourceScope.None or ResourceScope.Max)
                continue;

            _resourcesUI[(ResourceScope)scope] = new();
        }
        CreateCanvas();

        isInitialized = true;
    }

    public void Dispose()
    {
        base.Dispose();
    }

    protected virtual void CreateCanvas()
    {
        var root = new GameObject($"{canvasName}Canvas");
        foreach (var layer in System.Enum.GetValues(typeof(UILayer)))
        {
            if (true == (UILayer)layer is UILayer.None or UILayer.Max) continue;

            var canvas = new GameObject($"{layer}Canvas", typeof(RectTransform)).GetComponent<RectTransform>();
            var cv = canvas.AddComponent<Canvas>();
            cv.renderMode = RenderMode.ScreenSpaceOverlay;

            var cs = canvas.AddComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(2560, 1440);

            var gr = canvas.AddComponent<GraphicRaycaster>();
            gr.ignoreReversedGraphics = true;
            gr.blockingObjects = GraphicRaycaster.BlockingObjects.None;

            canvas.transform.SetParent(root.transform);

            _canvases[(UILayer)layer] = canvas.GetComponent<RectTransform>();
        }
    }

    public async Task<T1> CreateUI<T1>(string path, UILayer layer, ResourceScope scope = ResourceScope.Global) where T1 : Component
    {
        var asset = await ResourcesManager.Instance.Instantiate<T1>(path);
        if (asset == null)
            return null;

        var go = asset.gameObject;
        UIBase outUIBase = null;
        if (true == go.TryGetComponent<UIBase>(out outUIBase))
        {
            UILayer uiLayer = outUIBase.GetLayer();
            go.transform.SetParent(_canvases[uiLayer]);
            go.transform.SetAsLastSibling();
            //go.transform.localPosition = Vector3.zero;
            //go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            RectTransform rectTransform = go.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
        }

        _resourcesUI[scope][typeof(T1)] = go;
        return asset;
    }

    public T1 GetUI<T1>() where T1 : class
    {
        foreach (var dictionary in _resourcesUI.Values)
        {
            if (false == dictionary.ContainsKey(typeof(T)))
                continue;

            return dictionary[typeof(T1)] as T1;
        }

        Debug.LogError("Invaild type. does not exist in _UIsDictionary.");
        return default(T1);
    }
}
