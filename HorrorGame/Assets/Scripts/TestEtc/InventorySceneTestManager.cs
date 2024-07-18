using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySceneTestManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        JsonLoader loader = new JsonLoader();
        loader.Load();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
