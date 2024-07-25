using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
public enum ETMPColorType
{
    PRIMARY,
    SECONDARY,
    PRIMARY_REVERSED,
    NEGATIVE,
    BACKGROUND,
    BACKGROUND_ALT,
    CUSTOM
}

public enum ETMPFontType
{
    LIGHT,
    MEDIUM,
    BOLD,
    ALTERNATIVE_1,
    //ALTERNATIVE_2
}

public enum ETextCapitalize
{
    None,
    UPPER,
    LOWER
}

public class StringLocalizer : MonoBehaviour
{

    [Header("[ RESOURCES ]")]
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TMP_FontAsset _font;

    [Header("[ INFORMATION ]")]
    [SerializeField] private bool _isUpdateInit = false;
    [SerializeField] private Color _primaryColor = Color.white;
    [SerializeField] private Color _primaryReversedColor = new Color(0.3215686f, 0.5372549f, 0.7490196f, 1);
    [SerializeField] private Color _secondaryColor = Color.white;
    [SerializeField] private Color _negativeColor = new Color(0.7490196f, 0.07450981f, 0.07450981f, 1);
    [SerializeField] private Color _backgroundColor = new Color(0.4352941f, 0.5137255f, 0.7490196f, 1);
    [SerializeField] private Color _backgroundAltColor = Color.black;
    [SerializeField] private Color _customColor = Color.black;

    [Header("[ OPTIONS ]")]
    [SerializeField] private int _stringId = -1;
    [SerializeField] private bool keepAlphaValue = false;
    [SerializeField] private ETMPColorType colorType;
    [SerializeField] private ETMPFontType _fontType;
    [SerializeField] private ETextCapitalize _capitalize;
    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        if (null == _text)
        {
            Debug.LogError($"{GetType()} tmp is null.");
            return;
        }
        _primaryColor = Color.white;
        _primaryReversedColor = new Color(0.3215686f, 0.5372549f, 0.7490196f, 1);
        _secondaryColor = Color.white;
        _negativeColor = new Color(0.7490196f, 0.07450981f, 0.07450981f, 1);
        _backgroundColor = new Color(0.4352941f, 0.5137255f, 0.7490196f, 1);
        _backgroundAltColor = Color.black;
    }
    async void Start()
    {
        if (null == StringLocalizerManager.Instance.fontAsset)
            await UniTask.WaitUntil(() => null != StringLocalizerManager.Instance.fontAsset);

        StringLocalizerManager.Instance.onChangeLanguage += UpdateLanguage;
        if (true == _isUpdateInit)
        {
            UpdateString(_stringId);
        }
        await UpdateOptions();
    }

    public void UpdateString(int id)
    {
        _stringId = id;
        string text = string.Empty;
        switch (StringLocalizerManager.Instance.currentLanguage)
        {
            case ELanguage.En:
                {
                    text = StringData.table[id].En.Replace("\\n", "\n");
                }
                break;
            case ELanguage.Kr:
                {
                    text = StringData.table[id].Kr.Replace("\\n", "\n"); ;
                }
                break;
            default:
                Debug.LogError("Invaild language..");
                break;
        }
        switch (_capitalize)
        {
            case ETextCapitalize.None:
                _text.text = text;
                break;
            case ETextCapitalize.UPPER:
                _text.text = text.ToUpper();
                break;
            case ETextCapitalize.LOWER:
                _text.text = text.ToLower();
                break;
        }
    }



    public async void UpdateLanguage(ELanguage eLanguage)
    {
        switch (eLanguage)
        {
            case ELanguage.En:
                {
                    UpdateString(_stringId);
                }
                break;
            case ELanguage.Kr:
                {
                    UpdateString(_stringId);
                }
                break;
            default:
                Debug.LogError("Invaild language..");
                break;
        }
        await UpdateOptions();
        //_text.font = StringLocalizerManager.Instance.fontAsset;
    }

    async UniTask UpdateOptions()
    {
        UpdateColor();

        _text.font = await ResourcesManager.Instance.GetTMPFont(_fontType);
        _font = _text.font;
    }

    private void UpdateColor()
    {
        if (keepAlphaValue == false)
        {
            switch (colorType)
            {
                case ETMPColorType.PRIMARY:
                    _text.color = _primaryColor;
                    break;
                case ETMPColorType.SECONDARY:
                    _text.color = _secondaryColor;
                    break;
                case ETMPColorType.PRIMARY_REVERSED:
                    _text.color = _primaryReversedColor;
                    break;
                case ETMPColorType.NEGATIVE:
                    _text.color = _negativeColor;
                    break;
                case ETMPColorType.BACKGROUND:
                    _text.color = _backgroundColor;
                    break;
                case ETMPColorType.BACKGROUND_ALT:
                    _text.color = _backgroundAltColor;
                    break;
                case ETMPColorType.CUSTOM:
                    _text.color = _customColor;
                    break;
            }
        }
        else
        {
            switch (colorType)
            {
                case ETMPColorType.PRIMARY:
                    _text.color = new Color(_primaryColor.r, _primaryColor.g, _primaryColor.b, _text.color.a);
                    break;
                case ETMPColorType.SECONDARY:
                    _text.color = new Color(_secondaryColor.r, _secondaryColor.g, _secondaryColor.b, _text.color.a);
                    break;
                case ETMPColorType.PRIMARY_REVERSED:
                    _text.color = new Color(_primaryReversedColor.r, _primaryReversedColor.g, _primaryReversedColor.b, _text.color.a);
                    break;
                case ETMPColorType.NEGATIVE:
                    _text.color = new Color(_negativeColor.r, _negativeColor.g, _negativeColor.b, _text.color.a);
                    break;
                case ETMPColorType.BACKGROUND:
                    _text.color = new Color(_backgroundColor.r, _backgroundColor.g, _backgroundColor.b, _text.color.a);
                    break;
                case ETMPColorType.BACKGROUND_ALT:
                    _text.color = new Color(_backgroundAltColor.r, _backgroundAltColor.g, _backgroundAltColor.b, _text.color.a);
                    break;
                case ETMPColorType.CUSTOM:
                    _text.color = new Color(_customColor.r, _customColor.g, _customColor.b, _text.color.a); ;
                    break;
            }
        }
    }

#if UNITY_EDITOR

    private ELanguage languageForTest = ELanguage.En;
    public int GetStringID() => _stringId;
    public void SetLanguageForTest(ELanguage value) => languageForTest = value;
    public ELanguage GetLanguageForTest() => languageForTest;

    public async void UpdateStringToInspector(int id)
    {
        JsonLoader loader = new JsonLoader();
        loader.Load();

        _stringId = id;
        string text = string.Empty;
        switch (languageForTest)
        {
            case ELanguage.En:
                {
                    text = StringData.table[id].En.Replace("\\n", "\n");
                }
                break;
            case ELanguage.Kr:
                {
                    text = StringData.table[id].Kr.Replace("\\n", "\n"); ;
                }
                break;
            default:
                Debug.LogError("Invaild language..");
                break;
        }
        switch (_capitalize)
        {
            case ETextCapitalize.None:
                _text.text = text;
                break;
            case ETextCapitalize.UPPER:
                _text.text = text.ToUpper();
                break;
            case ETextCapitalize.LOWER:
                _text.text = text.ToLower();
                break;
        }

        UpdateColor();
        _text.font =  await ResourcesManager.GetTMPFontEditor(_fontType, languageForTest);
        _font = _text.font;
        Debug.Log("[StringLocalizer] - Complete update text!");
    }


#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(StringLocalizer))]
public class StringLocalizerEditor : Editor
{
    StringLocalizer stringLocalizer = null;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        stringLocalizer = (StringLocalizer)target;
        DrawUpdateText();

    }

    private void DrawUpdateText()
    {
        GUILayout.Space(20);
        DrawLabel("[ APPLY ]");
        GUILayout.Space(10);
        stringLocalizer.SetLanguageForTest((ELanguage)EditorGUILayout.EnumPopup("Language For Test", stringLocalizer.GetLanguageForTest()));
        if (GUILayout.Button("[ UPDATE TEXT ]"))
        {
            stringLocalizer.UpdateStringToInspector(stringLocalizer.GetStringID());
        }
    }

    private void DrawLabel(string text, int fontSize = 14, FontStyle fontStyle = FontStyle.Bold, Color fontColor = default, TextAnchor textAnchor = TextAnchor.MiddleCenter)
    {
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = fontSize; // 폰트 크기
        labelStyle.fontStyle = fontStyle; // 폰트 스타일
        labelStyle.normal.textColor = fontColor != default ? fontColor : Color.white;
        labelStyle.hover.textColor = fontColor != default ? fontColor : Color.white;
        labelStyle.active.textColor = fontColor != default ? fontColor : Color.white;
        labelStyle.focused.textColor = fontColor != default ? fontColor : Color.white;
        labelStyle.alignment = textAnchor; // 텍스트 정렬
        GUILayout.Label(text, labelStyle);
    }
}
#endif