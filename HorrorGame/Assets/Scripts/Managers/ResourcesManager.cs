using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using Enum;
using EResourceScope = Enum.EResourceScope;
public partial class ResourcesManager : Singleton<ResourcesManager>
{
    public ResourcesManager() { }
    ~ResourcesManager() { }

    private Dictionary<EResourceScope, List<object>> handles = new();
    private Dictionary<string, object> loadedHandles = new();
    private Dictionary<ELanguage, Dictionary<ETMPFontType, TMP_FontAsset>> _fontDic = new();
    private Dictionary<ESoundTypes, Dictionary<ESoundsType_Button, AudioClip>> _audioClipsDic = new();

    protected override void Initialize()
    {
        base.Initialize();
        AllocatedDictionaries();
    }

    protected override void Dispose()
    {
        ReleaseAll();
        loadedHandles.Clear();
        loadedHandles = null;
        handles.Clear();
        handles = null;
    }

    private void AllocatedDictionaries()
    {
        foreach (var scope in System.Enum.GetValues(typeof(EResourceScope)))
        {
            handles[(EResourceScope)scope] = new List<object>();
        }
        foreach (var language in System.Enum.GetValues(typeof(ELanguage)))
        {
            _fontDic[(ELanguage)language] = new();
        }
        foreach (var soundType in System.Enum.GetValues(typeof(ESoundTypes)))
        {
            ESoundTypes type = (ESoundTypes)soundType;
            if (type is ESoundTypes.None) continue;
            _audioClipsDic[type] = new();
        }
    }
    private void ReleaseAll()
    {
        foreach (var list in handles.Values)
        {
            foreach (var handle in list)
            {
                Addressables.Release(handle);
            }
        }
    }


    public void ReleaseScope(EResourceScope scope)
    {
        foreach (var handle in handles[scope])
        {
            Addressables.Release(handle);
        }
        handles[scope].Clear();
    }

    public async UniTask<T> LoadAssetAsyncGo<T>(string path, EResourceScope scope = EResourceScope.Global) where T : class
    {
        if (loadedHandles.ContainsKey(path))
        {
            GameObject cachedAsset = loadedHandles[path] as GameObject;
            if (cachedAsset != null && cachedAsset.TryGetComponent<T>(out T type))
            {
                return type;
            }
        }

        GameObject asset = null;
        bool isProcessed = false;

        Addressables.LoadAssetAsync<GameObject>(path).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                asset = handle.Result;
                handles[scope].Add(handle.Result);
                loadedHandles[path] = handle.Result;
            }
            else
            {
                Debug.LogError("Failed to load asset: " + path);
            }
            isProcessed = true;
        };

        await UniTask.WaitUntil(() => isProcessed);
        if (asset == null)
            return default;

        if (asset.TryGetComponent<T>(out T component))
        {
            return component;
        }
        return default;
    }

    public async UniTask<T> LoadAssetAsyncGeneric<T>(string path) where T : class
    {
        if (loadedHandles.ContainsKey(path))
        {
            return loadedHandles[path] as T;
        }

        T type = null;
        bool isProcessed = false;
        Addressables.LoadAssetAsync<T>(path).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                type = handle.Result;
                loadedHandles[path] = handle.Result;
            }
            else
            {
                Debug.LogError("Failed to load asset: " + path);
            }
            isProcessed = true;
        };

        await UniTask.WaitUntil(() => isProcessed);
        return type;
    }

    public async UniTask<GameObject> GameObjectInstantiate(string path, Transform parent, EResourceScope scope = EResourceScope.Global, System.Action<GameObject> onComplete = null)
    {
        if (loadedHandles.ContainsKey(path))
        {
            GameObject cachedInstance = UnityEngine.Object.Instantiate(loadedHandles[path] as GameObject, parent);
            onComplete?.Invoke(cachedInstance);
            return cachedInstance;
        }

        GameObject assetInstance = null;
        bool isProcessed = false;

        if (string.IsNullOrEmpty(path))
            return null;

        Addressables.InstantiateAsync(path, parent).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                assetInstance = handle.Result;
                loadedHandles[path] = handle.Result;
                handles[scope].Add(handle.Result);
            }
            else
            {
                Debug.LogError("Failed to instantiate asset: " + handle.OperationException);
            }
            isProcessed = true;
        };

        await UniTask.WaitUntil(() => isProcessed);
        onComplete?.Invoke(assetInstance);
        return assetInstance;
    }

    public async UniTask<T> Instantiate<T>(string path, System.Action<T> onComplete = null) where T : class
    {
        if (loadedHandles.ContainsKey(path))
        {
            GameObject cachedInstance = UnityEngine.Object.Instantiate(loadedHandles[path] as GameObject);
            if (typeof(T) == typeof(GameObject))
            {
                onComplete?.Invoke(cachedInstance as T);
                return cachedInstance as T;
            }

            if (cachedInstance.TryGetComponent(out T cachedComponent))
            {
                onComplete?.Invoke(cachedComponent);
                return cachedComponent;
            }
            return null;
        }

        GameObject assetInstance = null;
        bool isProcessed = true;

        Addressables.InstantiateAsync(path).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                assetInstance = handle.Result;
                loadedHandles[path] = handle.Result;
            }
            else
            {
                Debug.LogError("Failed to instantiate asset: " + handle.OperationException);
            }
            isProcessed = false;
        };

        await UniTask.WaitUntil(() => !isProcessed);

        if (typeof(GameObject) == typeof(T))
        {
            onComplete?.Invoke(assetInstance as T);
            return assetInstance as T;
        }

        if (assetInstance.TryGetComponent(out T component))
        {
            onComplete?.Invoke(component);
            return component;
        }
        return null;
    }

    #region [ Sprite ]
    public async UniTask<Sprite> GetSprite(string atlasPath, string spriteName)
    {
        SpriteAtlas atlas = null;
        if (!loadedHandles.ContainsKey(atlasPath))
        {
            atlas = await LoadAssetAsyncGeneric<SpriteAtlas>(atlasPath);
        }
        else
        {
            atlas = loadedHandles[atlasPath] as SpriteAtlas;
        }

        if (atlas == null)
        {
            Debug.LogError($"NullException :: atlas is null... {atlasPath}");
            return null;
        }

        Sprite sprite = atlas.GetSprite(spriteName);
        if (sprite == null)
        {
            Debug.LogError($"Sprite does not exist in {atlas.name} atlas.");
            return null;
        }

        return sprite;
    }

    public async UniTask<SpriteAtlas> GetAtlas(string atlasPath)
    {
        SpriteAtlas atlas = null;
        if (!loadedHandles.ContainsKey(atlasPath))
        {
            atlas = await LoadAssetAsyncGeneric<SpriteAtlas>(atlasPath);
        }
        else
        {
            atlas = loadedHandles[atlasPath] as SpriteAtlas;
        }

        if (atlas == null)
        {
            Debug.LogError($"NullException :: atlas is null... {atlasPath}");
            return null;
        }

        return atlas;
    }
    #endregion

    #region [ Font ]
    public async UniTask<TMP_FontAsset> GetTMPFont(ETMPFontType type)
    {

        if (_fontDic[StringLocalizerManager.Instance.currentLanguage].ContainsKey(type))
            return _fontDic[StringLocalizerManager.Instance.currentLanguage][type];

        TMP_FontAsset fontAsset = null;
        switch (StringLocalizerManager.Instance.currentLanguage)
        {
            case ELanguage.En:
                {
                    switch (type)
                    {
                        case ETMPFontType.LIGHT:
                            fontAsset = await LoadAssetAsyncGeneric<TMP_FontAsset>("Assets/Resources/Fonts/En/HorrorFonts/Larke Sans Light SDF.asset");
                            break;
                        case ETMPFontType.MEDIUM:
                            fontAsset = await LoadAssetAsyncGeneric<TMP_FontAsset>("Assets/Resources/Fonts/En/HorrorFonts/Larke Sans Regular SDF.asset");
                            break;
                        case ETMPFontType.BOLD:
                            fontAsset = await LoadAssetAsyncGeneric<TMP_FontAsset>("Assets/Resources/Fonts/En/HorrorFonts/Larke Sans Bold SDF.asset");
                            break;
                        case ETMPFontType.ALTERNATIVE_1:
                            fontAsset = await LoadAssetAsyncGeneric<TMP_FontAsset>("Assets/Resources/Fonts/En/HorrorFonts/Roboto-Regular SDF.asset");
                            break;
                    }
                }
                break;
            case ELanguage.Kr:
                {
                    switch (type)
                    {
                        case ETMPFontType.LIGHT:
                            fontAsset = await LoadAssetAsyncGeneric<TMP_FontAsset>("Assets/Resources/Fonts/Kr/Godo/GodoM_SDF.asset");
                            break;
                        case ETMPFontType.MEDIUM:
                            fontAsset = await LoadAssetAsyncGeneric<TMP_FontAsset>("Assets/Resources/Fonts/Kr/Godo/GodoB_SDF.asset");
                            break;
                        case ETMPFontType.BOLD:
                            fontAsset = await LoadAssetAsyncGeneric<TMP_FontAsset>("Assets/Resources/Fonts/Kr/Godo/GodoB_SDF.asset");
                            break;
                        case ETMPFontType.ALTERNATIVE_1:
                            fontAsset = await LoadAssetAsyncGeneric<TMP_FontAsset>("Assets/Resources/Fonts/Kr/Godo/GodoM_SDF.asset");
                            break;
                    }

                }
                break;
            default:
                break;
        }

        _fontDic[StringLocalizerManager.Instance.currentLanguage][type] = fontAsset;
        return fontAsset;
    }
    #endregion

    #region [ Audio ]
    public AudioClip GetAudioClip(ESoundTypes st, int st_category)
    {
        switch (st)
        {
            case ESoundTypes.None:
                break;
            case ESoundTypes.In:
                {
                    ESoundsType_In type = (ESoundsType_In)st_category;
                    //[]
                }
                break;
            case ESoundTypes.Button:
                {
                    ESoundsType_Button type = (ESoundsType_Button)st_category;
                    var audioClip = _audioClipsDic[ESoundTypes.Button][type];
                    if (audioClip == null)
                    {
                        Debug.LogError($"[Null Exception] {type} audio clip is null.");
                        return null;
                    }

                    return audioClip;
                }
        }
        return null;

    }
    #endregion

    #region [ Editor ]
#if UNITY_EDITOR
    public static TMP_FontAsset GetTMPFontEditor(ETMPFontType type, ELanguage language)
    {
        TMP_FontAsset fontAsset = null;
        switch (language)
        {
            case ELanguage.En:
                {
                    switch (type)
                    {
                        case ETMPFontType.LIGHT:
                            fontAsset = LoadAssetAsyncGenericEditor<TMP_FontAsset>("Assets/Resources/Fonts/En/HorrorFonts/Larke Sans Light SDF.asset");
                            break;
                        case ETMPFontType.MEDIUM:
                            fontAsset = LoadAssetAsyncGenericEditor<TMP_FontAsset>("Assets/Resources/Fonts/En/HorrorFonts/Larke Sans Regular SDF.asset");
                            break;
                        case ETMPFontType.BOLD:
                            fontAsset = LoadAssetAsyncGenericEditor<TMP_FontAsset>("Assets/Resources/Fonts/En/HorrorFonts/Larke Sans Bold SDF.asset");
                            break;
                        case ETMPFontType.ALTERNATIVE_1:
                            fontAsset = LoadAssetAsyncGenericEditor<TMP_FontAsset>("Assets/Resources/Fonts/En/HorrorFonts/Roboto-Regular SDF.asset");
                            break;
                    }
                }
                break;
            case ELanguage.Kr:
                {
                    switch (type)
                    {
                        case ETMPFontType.LIGHT:
                            fontAsset = LoadAssetAsyncGenericEditor<TMP_FontAsset>("Assets/Resources/Fonts/Kr/Godo/GodoM_SDF.asset");
                            break;
                        case ETMPFontType.MEDIUM:
                            fontAsset = LoadAssetAsyncGenericEditor<TMP_FontAsset>("Assets/Resources/Fonts/Kr/Godo/GodoB_SDF.asset");
                            break;
                        case ETMPFontType.BOLD:
                            fontAsset = LoadAssetAsyncGenericEditor<TMP_FontAsset>("Assets/Resources/Fonts/Kr/Godo/GodoB_SDF.asset");
                            break;
                        case ETMPFontType.ALTERNATIVE_1:
                            fontAsset = LoadAssetAsyncGenericEditor<TMP_FontAsset>("Assets/Resources/Fonts/Kr/Godo/GodoM_SDF.asset");
                            break;
                    }

                }
                break;
            default:
                break;
        }

        return fontAsset;
    }

    public static T LoadAssetAsyncGenericEditor<T>(string path) where T : class
    {
        T type = null;
        Addressables.LoadAssetAsync<T>(path).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                type = handle.Result;
            }
            else
            {
                Debug.LogError("Failed to load asset: " + path);
            }
        };

        return type;
    }
#endif
    #endregion
}