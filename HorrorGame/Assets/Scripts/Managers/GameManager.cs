using Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private EScenes _currentScene;
    public EScenes currentScene { get { return _currentScene; }}

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
        Debug.Log("�� ���Ӿ� �ε� ��");
    }

    private void LoadedScene(Scene scene, LoadSceneMode mode)
    {
       switch (scene.name)
       {
            case nameof(EScenes.TitleScene):
                {
                    _currentScene = EScenes.TitleScene;
                    TitleSequence();
                }
                break;
            case nameof(EScenes.InGameScene):
                {
                    Debug.Log("�ΰ��Ӿ� �ε� ��");
                    _currentScene = EScenes.InGameScene;
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
