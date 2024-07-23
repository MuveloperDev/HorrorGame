using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum ELanguage
{
    En = 0,
    Kr
}
public class StringLocalizerManager : Singleton<StringLocalizerManager>
{
    public ELanguage currentLanguage { get; private set; }

    public Action<ELanguage> onChangeLanguage;

    public TMP_FontAsset fontAsset { get; private set; } = null;

    protected override void Initialize()
    {
        currentLanguage = ELanguage.En;
    }

    public void ChangeLanguage(ELanguage eLanguage)
    {
        currentLanguage = eLanguage;
        onChangeLanguage?.Invoke(currentLanguage);
    }


}
