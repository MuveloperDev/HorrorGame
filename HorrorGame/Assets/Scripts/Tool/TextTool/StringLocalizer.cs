using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StringLocalizer : MonoBehaviour
{
    [Header("[ DEPENDENCE ]")]
    [SerializeField] private TextMeshProUGUI tmp;
    [Header("[ INFORMATION ]")]
    [SerializeField] private int _currentId = -1;
    [SerializeField] private bool _isUpdateInit = false;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        if (null == tmp)
        {
            Debug.LogError($"{GetType()} tmp is null.");
            return;
        }
    }
    async void Start()
    {
        if (null == StringLocalizerManager.Instance.fontAsset)
            await UniTask.WaitUntil(() => null != StringLocalizerManager.Instance.fontAsset);

        tmp.font = StringLocalizerManager.Instance.fontAsset;
        StringLocalizerManager.Instance.onChangeLanguage += UpdateLanguage;
        if (true == _isUpdateInit)
        {
            UpdateString(_currentId);
        }
    }

    public void UpdateString(int id)
    {
        _currentId = id;
        switch (StringLocalizerManager.Instance.currentLanguage)
        {
            case ELanguage.En:
                {
                    tmp.text = StringData.table[id].En.Replace("\\n", "\n");
                }
                break;
            case ELanguage.Kr:
                {
                    tmp.text = StringData.table[id].Kr.Replace("\\n", "\n"); ;
                }
                break;
            default:
                Debug.LogError("Invaild language..");
                break;
        }
    }

    public void UpdateLanguage(ELanguage eLanguage)
    {
        switch (eLanguage)
        {
            case ELanguage.En:
                {
                    UpdateString(_currentId);
                }
                break;
            case ELanguage.Kr:
                {
                    UpdateString(_currentId);
                }
                break;
            default:
                Debug.LogError("Invaild language..");
                break;
        }
        tmp.font = StringLocalizerManager.Instance.fontAsset;
    }
}
