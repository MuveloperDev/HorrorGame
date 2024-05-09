#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEditor.SceneManagement;

public class BuildCustomWindow : EditorWindow
{
    // 창의 크기를 설정합니다.
    private static readonly Vector2 WindowSize = new Vector2(800, 600);

    // 버튼의 크기를 설정합니다.
    private static readonly GUILayoutOption ButtonWidth = GUILayout.Width(200);
    private static readonly GUILayoutOption ButtonHeight = GUILayout.Height(50);

    // 버튼의 여백을 설정합니다.
    private static readonly GUILayoutOption ButtonPadding = GUILayout.ExpandWidth(false);
    // 버튼의 크기와 위치를 설정합니다.
    private static readonly Rect ButtonRect = new Rect(100, 100, 200, 50);

    // 버튼의 스타일을 설정합니다.
    private static GUIStyle buttonStyle = null;
    // 현재 타겟 플랫폼을 저장할 변수입니다.
    private BuildTarget currentTarget;
    private Vector2 scrollPosition;

    [MenuItem("Custom/Build Window")]
    public static void ShowWindow()
    {
        // 새로운 에디터 창을 생성하고 보여줍니다.
        GetWindow<BuildCustomWindow>("Custom Window");


    }
    public enum EResourceType
    {
        Min = 0,
        Prefab,
        TMP_FontAsset,
        SpriteAtlas,
        Max
    }
    public static void AssignPrefabsToAddressables()
    {
        // AddressableAssetSettings를 로드합니다.
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        string typeStr = string.Empty;
        foreach (var type in System.Enum.GetValues(typeof(EResourceType)))
        {
            if ((EResourceType)type is EResourceType.Min or EResourceType.Max)
                continue;

            if ((int)type == (int)EResourceType.Max - 1)
                typeStr += $"t:{type}";
            else typeStr += $"t:{type} ";
        }
        // Resources 폴더에 있는 모든 프리팹을 찾습니다.
        string[] prefabGUIDs = AssetDatabase.FindAssets(typeStr, new[] { "Assets/Resources" });

        foreach (string guid in prefabGUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // 이미 Addressable로 등록된 에셋인지 확인합니다.
            if (settings.FindAssetEntry(guid) == null)
            {
                // Addressable에 등록되지 않은 에셋이라면, 새로 등록합니다.
                AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
                entry.address = assetPath;
            }
        }

        // 변경 사항을 저장합니다.
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true, true);
    }
    private static void BuildAddressables()
    {
        AddressableAssetSettings.BuildPlayerContent();
    }

    private void OnEnable()
    {
        // 현재 타겟 플랫폼을 가져옵니다.
        currentTarget = EditorUserBuildSettings.activeBuildTarget;
        
    }
    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("", GUI.skin.horizontalSlider);
        GUILayout.Space(10);

        CreateBuildSetting();

        GUILayout.Space(10);
        GUILayout.Label("", GUI.skin.horizontalSlider);
        GUILayout.Space(10);

        CreatePlatformSetting();

        GUILayout.Space(10);
        GUILayout.Label("", GUI.skin.horizontalSlider);
        GUILayout.Space(10);

        // 스타일을 적용한 레이블을 만듭니다.
        CreateAddressable();

        GUILayout.Space(10);
        GUILayout.Label("", GUI.skin.horizontalSlider);
    }
    private void CreateBuildSetting()
    {
        GUILayout.Label("Build Setting", FontStyle(), GUILayout.ExpandWidth(true));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(100));
        foreach (var scene in EditorBuildSettings.scenes)
        {
            GUILayout.BeginHorizontal();
            scene.enabled = EditorGUILayout.Toggle(scene.enabled, GUILayout.Width(20));
            EditorGUILayout.LabelField(scene.path, EditorStyles.label);
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                RemoveScene(scene.path);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.Space(10);

        if (GUILayout.Button("Add Open Scenes"))
        {
            AddOpenScenes();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Add Scene by Path"))
        {
            string path = EditorUtility.OpenFilePanel("Add Scene", "Assets", "unity");
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Replace(Application.dataPath, "Assets");
                AddScene(path);
            }
        }
    }
    private void CreatePlatformSetting()
    {
        GUILayout.Label("Platform Setting", FontStyle(), GUILayout.ExpandWidth(true));
        GUILayout.Space(10);
        // 타겟 플랫폼을 선택할 수 있는 드롭다운 메뉴를 만듭니다.
        currentTarget = (BuildTarget)EditorGUILayout.EnumPopup("Target Platform", currentTarget);
        GUILayout.Space(20);

        if (GUILayout.Button("Change Platform"))
        {
            // 선택한 플랫폼으로 변경합니다.
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(currentTarget), currentTarget);
        }
    }

    private void AddOpenScenes()
    {
        foreach (var scene in EditorSceneManager.GetSceneManagerSetup())
        {
            AddScene(scene.path);
        }
    }

    private void AddScene(string path)
    {
        var scenes = EditorBuildSettings.scenes.ToList();
        if (!scenes.Any(s => s.path == path))
        {
            scenes.Add(new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
    private void RemoveScene(string path)
    {
        var scenes = EditorBuildSettings.scenes.ToList();
        var scene = scenes.FirstOrDefault(s => s.path == path);
        if (scene != null)
        {
            scenes.Remove(scene);
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
    private void CreateAddressable()
    {
        GUILayout.Label("Addressable", FontStyle(), GUILayout.ExpandWidth(true));

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Clear And Load", ButtonWidth, ButtonHeight, ButtonPadding))
        {
            AssignPrefabsToAddressables();
            Debug.Log("Button clicked!");
        }
        GUILayout.Space(200);
        if (GUILayout.Button("Build", ButtonWidth, ButtonHeight, ButtonPadding))
        {
            BuildAddressables();
            Debug.Log("Button clicked!");
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private GUIStyle FontStyle()
    {
        GUISkin skin = GUI.skin;
        GUIStyle style = new GUIStyle(skin.label);
        style.normal.textColor = Color.gray;
        style.fontSize = 20;
        style.alignment = TextAnchor.MiddleLeft;
        return style;
    }
}
#endif
