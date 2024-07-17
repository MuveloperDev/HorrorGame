using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using EResourceScope = Enum.EResourceScope;
public class ResourcesManager : Singleton<ResourcesManager>
{
    public ResourcesManager() { }
    ~ResourcesManager() { }

    private Dictionary<EResourceScope, List<object>> handles = new();
    private Dictionary<string, object> loadedHandles = new();

    protected override void InitializeTemplate()
    {
        base.InitializeTemplate();
        Initialize();
    }
    private void Initialize()
    {
        foreach (var scope in System.Enum.GetValues(typeof(EResourceScope)))
        {
            handles[(EResourceScope)scope] = new List<object>();
        }
    }

    protected override void Dispose()
    {
        ReleaseAll();
        loadedHandles.Clear();
        loadedHandles = null;
        handles.Clear();
        handles = null;
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
}