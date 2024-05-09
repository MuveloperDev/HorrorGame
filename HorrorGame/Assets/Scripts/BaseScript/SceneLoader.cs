using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    public Action<float> onProgress;
    public Action onCompleteLoad;
    public Action onLoadBefore;

    public void LoadSceneAsync(string sceneName)
    {
        _= LoadAsyncScene(sceneName);
    }

    private async UniTask LoadAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        onLoadBefore?.Invoke();
        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log("Loading progress: " + (asyncLoad.progress * 100) + "%");
            onProgress?.Invoke(asyncLoad.progress);
            await UniTask.Yield();
        }

        onCompleteLoad?.Invoke();
        asyncLoad.allowSceneActivation = true;
        while (false == asyncLoad.isDone)
        {
            onProgress?.Invoke(asyncLoad.progress);
            await UniTask.Yield();
        }
        //SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
