using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Enum;
using UnityEngine.U2D;

public class ResourcesManager : Singleton<ResourcesManager>
{
    public ResourcesManager() { }
    ~ResourcesManager() {  }

    private Dictionary<ResourceScope, List<object>> handles = new();
    private Dictionary<string, object> loadedHandles = new();

    protected override void InitializeTemplate()
    {
        base.InitializeTemplate();
        Initialize();
    }
    private void Initialize()
    {
        foreach (var scope in System.Enum.GetValues(typeof(ResourceScope)))
        {
            handles[(ResourceScope)scope] = new();
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

    public void ReleaseScope(ResourceScope scope)
    {
        foreach (var handle in handles[scope])
        {
            Addressables.Release(handle);
        }
    }

    public async UniTask<T> LoadAssetAsyncGo<T>(string path, ResourceScope scope = ResourceScope.Global) where T : class
    {
        GameObject asset = null;
        bool isProcessed = false;
        Addressables.LoadAssetAsync<GameObject>(path).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 애셋 로드 성공
                asset = handle.Result;
                handles[scope].Add(handle.Result);
                loadedHandles[path] = handle.Result;
            }
            else
            {
                // 애셋 로드 실패
                Debug.LogError("Failed to load asset: ");
            }
            isProcessed = true;
        };


        await UniTask.WaitUntil(() => false != isProcessed);
        if (null == asset)
            return default;

        T type = null;
        if (true == asset.TryGetComponent<T>(out type))
        {
            return type;
        }
        return default;
    }
    public async UniTask<T> LoadAssetAsyncGeneric<T>(string path) where T : class
    {
        T type = null;
        bool isProcessed = false;
        Addressables.LoadAssetAsync<T>(path).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 애셋 로드 성공
                type = handle.Result;
                loadedHandles[path] = handle.Result;
            }
            else
            {
                // 애셋 로드 실패
                Debug.LogError("Failed to load asset: ");
            }
            isProcessed = true;
        };


        await UniTask.WaitUntil(() => false != isProcessed);
        if (null == type)
            return default;


        return type;
    }
    public async UniTask<UnityEngine.GameObject> GameObjectInstantiate(string path, Transform parent, ResourceScope scope = ResourceScope.Global, Action<GameObject> onComplete = null)
    {
        GameObject assetInstance = null;
        bool isProcessed = false;

        if (true == string.IsNullOrEmpty(path))
            return null;

        Addressables.InstantiateAsync(path, parent).Completed += (AsyncOperationHandle<GameObject> handle) =>
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

        await UniTask.WaitUntil(() => false != isProcessed);

        if (onComplete != null)
        {
            onComplete(assetInstance);
        }
        if (assetInstance != null)
        {
            return assetInstance;
        }
        return null;
    }

    public async UniTask<T> Instantiate<T>(string path, Action<T> onComplete = null) where T : class
    {
        GameObject assetInstance = null;
        bool isProcessing = true;

        Addressables.InstantiateAsync(path).Completed += (AsyncOperationHandle<GameObject> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 애셋 인스턴스 생성 성공
                assetInstance = handle.Result;
                loadedHandles[path] = handle.Result;
            }
            else
            {
                // 애셋 인스턴스 생성 실패
                Debug.LogError("Failed to instantiate asset: " + handle.OperationException);
            }
            isProcessing = false;
        };

        await UniTask.WaitUntil(() => true != isProcessing);

        if (typeof(GameObject) == typeof(T))
        {
            if (onComplete != null)
            {
                onComplete(assetInstance as T);
            }
            return assetInstance as T;
        }

        if (assetInstance.TryGetComponent<T>(out T component))
        {
            if (onComplete != null)
            {
                onComplete(component);
            }
            return component;
        }
        return null;
    }

    public async UniTask<Sprite> GetSprite(string atlasPath, string sprieName)
    {
        SpriteAtlas atlas = null;
        if (false == loadedHandles.ContainsKey(atlasPath))
        {
            atlas = await LoadAssetAsyncGeneric<SpriteAtlas>(atlasPath);
        }
        else
        {
            atlas = loadedHandles[atlasPath] as SpriteAtlas;
        }

        if (null == atlas)
        {
            Debug.LogError($"NullException :: atlas is null... {atlasPath}");
            return null;
        }

        Sprite sprite = atlas.GetSprite(sprieName);
        if (null == sprite)
        {
            Debug.LogError($"Sprite does not exist in {atlas.name} atlas.");
            return null;
        }
        
        return sprite;
    }

    public async UniTask<SpriteAtlas> GetAtlas(string atlasPath)
    {
        SpriteAtlas atlas = null;
        if (false == loadedHandles.ContainsKey(atlasPath))
        {
            atlas = await LoadAssetAsyncGeneric<SpriteAtlas>(atlasPath);
        }
        else
        {
            atlas = loadedHandles[atlasPath] as SpriteAtlas;
        }

        if (null == atlas)
        {
            Debug.LogError($"NullException :: atlas is null... {atlasPath}");
            return null;
        }

        return atlas;
    }
}
