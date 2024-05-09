using Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private Scenes _currentScene;
    public Scenes currentScene { get { return _currentScene; }}

    [SerializeField] private JsonLoader _jsonLoader = null;
    protected override void Awake()
    {
        base.Awake();
        CreateObjects();
        SceneManager.sceneLoaded += LoadedScene;
        SceneLoader.Instance.onCompleteLoad += LoadComplete;
    }

    private void CreateObjects()
    {
        _jsonLoader = new JsonLoader();
        _jsonLoader.Load();
        ResourcesManager.Instance.CreateObject();
        StringLocalizerManager.Instance.CreateObject();

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {

            SceneLoader.Instance.LoadSceneAsync("InGameScene");
        }
    }

    private void LoadComplete()
    {
        Debug.Log("인 게임씬 로드 전");
    }

    private void LoadedScene(Scene scene, LoadSceneMode mode)
    {
       switch (scene.name)
       {
            case nameof(Scenes.TitleScene):
                {
                    _currentScene = Scenes.TitleScene;
                    TitleSequence();
                }
                break;
            case nameof(Scenes.InGameScene):
                {
                    Debug.Log("인게임씬 로드 완");
                    _currentScene = Scenes.InGameScene;
                    InGameSequence();
                }
                break;
       }
    }

    private void TitleSequence()
    { 
        
    }
    private void InGameSequence()
    { 
        
    }
}
