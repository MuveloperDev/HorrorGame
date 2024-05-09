using Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public UIManager() {}
    ~UIManager() { Dispose(); }

    public bool isInitialized { get; private set; } = false;

    private Dictionary<ResourceScope, Dictionary<Type, GameObject>> _resourcesUI = new();
    private Dictionary<UILayer, RectTransform> _canvases = new();

    protected string canvasName = string.Empty;
    protected override void InitializeTemplate()
    {
        CreateCanvas();
        AfterInitialize();
        isInitialized = true;
    }
    protected virtual void AfterInitialize()
    {
    }

    protected override void Dispose()
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

    public async Task<T> CreateUI<T>(string path, UILayer layer, ResourceScope scope = ResourceScope.Global) where T : Component
    {
        var asset = await ResourcesManager.Instance.Instantiate<T>(path);
        if (asset == null) 
            return null;

        var go = asset.gameObject;
        UIBase outUIBase = null;
        if (true == go.TryGetComponent<UIBase>(out outUIBase))
        {
            UILayer uiLayer = outUIBase.GetLayer();
            go.transform.SetParent(_canvases[uiLayer]);
            go.transform.SetAsLastSibling();
        }

        _resourcesUI[scope][typeof(T)] = go;
        return asset;
    }

    public T GetUI<T>()  where T : class
    {
        foreach (var dictionary in _resourcesUI.Values)
        {
            if (false == dictionary.ContainsKey(typeof(T)))
                continue;

            return dictionary[typeof(T)] as T;
        }

        Debug.LogError("Invaild type. does not exist in _UIsDictionary.");
        return default(T);
    }
}
