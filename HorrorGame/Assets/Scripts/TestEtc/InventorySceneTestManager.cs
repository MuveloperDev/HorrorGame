using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySceneTestManager : MonoBehaviour
{
    // Start is called before the first frame update
    async void Awake()
    {
        JsonLoader loader = new JsonLoader();
        loader.Load();

        await UniTask.WaitUntil(()=> true == IngameUIManager.Instance.isInitialized);
        await ResourcesManager.Instance.LoadResources();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
