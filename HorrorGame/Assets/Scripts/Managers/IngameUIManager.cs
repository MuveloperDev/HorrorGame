using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameUIManager : UIManager<IngameUIManager>
{
    public UIProjectBase mainPanel;
    private Dictionary<Type, object> _UIDictionary;


    private async UniTask<bool> Initalize()
    {
        _UIDictionary = new();

        CreateCanvas();

        return true;
    }

}
