using Enum;
using Michsky.UI.Dark;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager<T> : Singleton<T> where T : UIManager<T>, new()
{
    public UIManager() {}
    ~UIManager() { Dispose(); }

    public bool isInitialized { get; private set; } = false;

    private GameObject _root;
    private AudioSource _audioSource;
    private Dictionary<EResourceScope, Dictionary<Type, GameObject>> _resourcesUI = new();
    private Dictionary<EUICanvasLayer, RectTransform> _canvases = new();

    protected string canvasName = string.Empty;
    protected override void Initialize()
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
        _root = new GameObject($"{canvasName}Canvas");
        foreach (var layer in System.Enum.GetValues(typeof(EUICanvasLayer)))
        {
            if (true == (EUICanvasLayer)layer is EUICanvasLayer.None or EUICanvasLayer.Max) continue;

            var canvas = new GameObject($"{layer}Canvas", typeof(RectTransform)).GetComponent<RectTransform>();
            var cv = canvas.AddComponent<Canvas>();
            cv.renderMode = RenderMode.ScreenSpaceOverlay;
            
            var cs = canvas.AddComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920, 1080);
            
            var gr = canvas.AddComponent<GraphicRaycaster>();
            gr.ignoreReversedGraphics = true;
            gr.blockingObjects = GraphicRaycaster.BlockingObjects.None;

            canvas.transform.SetParent(_root.transform);

            _canvases[(EUICanvasLayer)layer] = canvas.GetComponent<RectTransform>();
        }
    }

    protected virtual void AddAudioSource()
    { 
        _root.AddComponent<AudioSource>();
    }

    public async Task<T> CreateUI<T>(string path, EUICanvasLayer layer, EResourceScope scope = EResourceScope.Global) where T : Component
    {
        var asset = await ResourcesManager.Instance.Instantiate<T>(path);
        if (asset == null) 
            return null;

        var go = asset.gameObject;
        UIBase outUIBase = null;
        if (true == go.TryGetComponent<UIBase>(out outUIBase))
        {
            EUICanvasLayer uiLayer = outUIBase.GetLayer();
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
