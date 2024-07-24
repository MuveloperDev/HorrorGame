using Cysharp.Threading.Tasks;
using Enum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ResourcesManager
{
    public async UniTask LoadResources()
    {
        await LoadAduioClips();
    }

    public async UniTask LoadAduioClips()
    {
        await LoadAudioClips_Button();
    }

    public async UniTask LoadAudioClips_Button()
    {
        //foreach (var value in System.Enum.GetValues(typeof(ESoundsType_Button)))
        //{
        //    var type = (ESoundsType_Button)value;
        //    if (type is ESoundsType_Button.None)
        //        continue;
        //}

        _audioClipsDic[ESoundTypes.Button][ESoundsType_Button.Click]
        = await LoadAssetAsyncGeneric<AudioClip>($"Assets/Resources/Sounds/FX_Sounds/Click.wav");
        _audioClipsDic[ESoundTypes.Button][ESoundsType_Button.Hover]
        = await LoadAssetAsyncGeneric<AudioClip>($"Assets/Resources/Sounds/FX_Sounds/Hover.wav");
    }
}
