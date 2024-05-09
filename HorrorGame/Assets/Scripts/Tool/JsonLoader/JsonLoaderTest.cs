using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonLoaderTest : MonoBehaviour
{

    async void Start()
    {
        JsonLoader loader = new JsonLoader();
        loader.Load();

        foreach (var data in TestData.table.Values)
        {
            Debug.Log($"data : {data.Id} / {data.Name} / {data.Description}");
        }

        //var a = await ResourcesManager.Instance.LoadAssetAsyncGo<PagingSystem>("Assets/Resources/Prefabs/DialogBox.prefab");
        //if (a != null)
        //{
        //    Debug.Log("AAA");
        //    a.Test();
        //}
        //else
        //{
        //    Debug.Log("BBBB");
        //}
    }
}
