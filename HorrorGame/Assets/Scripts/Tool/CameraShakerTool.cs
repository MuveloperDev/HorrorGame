#if UNITY_EDITOR
using FirstGearGames.SmoothCameraShaker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
[InitializeOnLoad]
public class SceneOpenedCallback
{
    private const string TargetScenePath = "Assets/Scenes/CameraShake.unity";

    static SceneOpenedCallback()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        if (scene.path == TargetScenePath)
        {
            // 특정 씬이 열렸을 때 수행할 작업을 여기에 작성합니다.
            Debug.Log("Target scene opened: " + scene.name);
            SetCameraShaker();
        }
    }
    private static void SetCameraShaker()
    {
        var cameraShaker = Camera.main.GetComponent<CameraShaker>();
        if (null == cameraShaker)
        {
            Camera.main.AddComponent<CameraShaker>();
        }
        var container = GameObject.Find("CameraShakerSimulator");
        if (null == container)
        {
            CameraShakerTool go = new GameObject("CameraShakerSimulator").AddComponent<CameraShakerTool>();
        }
    }
}
public class CameraShakerTool : MonoBehaviour
{

    [SerializeField] private List<ShakeData> _shakeDatalist = new();

    public int _tableId;
    public bool _useTable;
    public bool _scaledTime = true;
    public bool _shakeCameras = true;
    public bool _shakeCanvases = true;
    public bool _shakeObjects = true;
    public bool _unlimitedDuration = false;
    public float _totalDuration = 1f;
    public float _fadeInDuration = 0.5f;
    public float _fadeOutDuration = 0.5f;
    public float _magnitude;
    public float _magnitudeNoise;
    public AnimationCurve _magnitudeCurve = new();
    public float _roughness;
    public float _roughnessNoise;
    public AnimationCurve _roughnessCurve = new();
    public Vector3 _positionalInfluence = new Vector3(0.1f, 0.1f, 0.1f);
    public InvertibleAxes _positionalInverts;
    public Vector3 _rotationalInfluence = new Vector3(0.35f, 0.35f, 0.35f);
    public InvertibleAxes _rotationalInverts;
    public bool _randomSeed;

    private void Awake()
    {
        var loader = new JsonLoader();
        loader.Load();
    }
    //private void OnValidate()
    //{
    //	StopShake();
    //}
    void Start()
    {
        foreach (var data in CameraShakeData.table.Values)
        {
            ShakeData shakeData = new ShakeData();
            AnimationCurve maginutdeCurve = EditorCurveSerializer.DeserializeCurve(EditorCurveSerializer.stringToByteArray(data.MagnitudeCurve));
            AnimationCurve roughnessCurve = EditorCurveSerializer.DeserializeCurve(EditorCurveSerializer.stringToByteArray(data.RoughnessCurve));

            shakeData.SetInstancedWithProperties(
                data.ScaledTime,
                data.Camera,
                data.Canvases,
                data.Objects,
                data.UnlimitedDuration,
                data.Magnitude,
                data.MagnitudeNoise,
                maginutdeCurve,
                data.Roughness,
                data.RoughnessNoise,
                roughnessCurve,
                data.TotalDuration,
                data.FadeInDuration,
                data.FadeOutDuration,
                new Vector3(data.PositionalInfluence[0], data.PositionalInfluence[1], data.PositionalInfluence[2]),
                GenerateInvertibleAxes(data.PositionalInverts),
                new Vector3(data.RotationalInfluence[0], data.RotationalInfluence[1], data.RotationalInfluence[2]),
                GenerateInvertibleAxes(data.RotationalInverts),
                data.RandomSeed
                );
            _shakeDatalist.Add(shakeData);
        }
    }

    #region [Custom Editor && DataIntegrity Functions]
    static public void CameraEditorShake(bool useTable, int idx = 0, ShakeData argData = null)
    {
        CameraShakerHandler.Stop();
        if (true == useTable)
        {
            var loader = new JsonLoader();
            loader.Load();
            var data = CameraShakeData.table[idx];
            ShakeData shakeData = new ShakeData();
            AnimationCurve maginutdeCurve = EditorCurveSerializer.DeserializeCurve(EditorCurveSerializer.stringToByteArray(data.MagnitudeCurve));
            AnimationCurve roughnessCurve = EditorCurveSerializer.DeserializeCurve(EditorCurveSerializer.stringToByteArray(data.RoughnessCurve));

            shakeData.SetInstancedWithProperties(
                data.ScaledTime,
                data.Camera,
                data.Canvases,
                data.Objects,
                data.UnlimitedDuration,
                data.Magnitude,
                data.MagnitudeNoise,
                maginutdeCurve,
                data.Roughness,
                data.RoughnessNoise,
                roughnessCurve,
                data.TotalDuration,
                data.FadeInDuration,
                data.FadeOutDuration,
                new Vector3(data.PositionalInfluence[0], data.PositionalInfluence[1], data.PositionalInfluence[2]),
                GenerateInvertibleAxes(data.PositionalInverts),
                new Vector3(data.RotationalInfluence[0], data.RotationalInfluence[1], data.RotationalInfluence[2]),
                GenerateInvertibleAxes(data.RotationalInverts),
                data.RandomSeed
                );
            CameraShakerHandler.Shake(shakeData);
            return;
        }

        CameraShakerHandler.Shake(argData);
    }

    static public void StopShake()
    {
        CameraShakerHandler.SetPaused(true);
        CameraShakerHandler.StopAll();
    }
    public static InvertibleAxes GenerateInvertibleAxes(int[] array)
    {
        InvertibleAxes currentInvert = 0;
        if (array[0] == 0 && array[1] == 0 && array[2] == 0)
        {
            currentInvert = 0; // Nothing
        }
        else if (array[0] == 1 && array[1] == 1 && array[2] == 1)
        {
            currentInvert = InvertibleAxes.X | InvertibleAxes.Y | InvertibleAxes.Z; // Everything
        }
        else
        {
            if (array[0] == 1)
            {
                currentInvert = InvertibleAxes.X;
            }
            else if (array[1] == 1)
            {
                currentInvert = InvertibleAxes.Y;
            }
            else if (array[2] == 1)
            {
                currentInvert = InvertibleAxes.Z;
            }
        }
        return currentInvert;
    }
    public static string InvertibleAxesToString(InvertibleAxes data)
    {
        if (data == 0)
        {
            return "[0,0,0]";
        }
        else if (data == (InvertibleAxes.X | InvertibleAxes.Y | InvertibleAxes.Z) || -1 == (int)data)
        {
            return "[1,1,1]";
        }
        else
        {
            if (data == InvertibleAxes.X)
            {
                return "[1,0,0]";
            }
            else if (data == InvertibleAxes.Y)
            {
                return "[0,2,0]";
            }
            else if (data == InvertibleAxes.Z)
            {
                return "[0,0,4]";
            }
        }
        Debug.LogError($"{data} is invailid....");
        return string.Empty;
    }


    public static string AddDecimalPoint(float value)
    {
        string strValue = value.ToString();
        string[] tokens = strValue.Split('.');
        if (1 >= tokens.Length)
        {
            return $"{strValue}.0";
        }
        else
        {
            return strValue;
        }
    }

    public static string NonZeroInterpolate(float value)
    {
        if (value == 0)
        {
            return "0.0001";
        }
        return value.ToString();
    }
    #endregion
}

public static class EditorCurveSerializer
{
    public static byte[] SerializeCurve(AnimationCurve curve)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();

        formatter.Serialize(stream, new SerializableCurve(curve));

        return stream.ToArray();
    }

    public static AnimationCurve DeserializeCurve(byte[] data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream(data);

        SerializableCurve serializableCurve = (SerializableCurve)formatter.Deserialize(stream);

        return serializableCurve.ToCurve();
    }

    public static byte[] stringToByteArray(string data)
    {
        List<byte> bytes = new();
        string[] tokens = data.Split(',');
        foreach (var token in tokens)
        {
            byte byteValue;
            if (byte.TryParse(token, out byteValue))
            {
                bytes.Add(byteValue);
            }
            else
            {
                Console.WriteLine($"'{token}' is out of range for a Byte.");
            }
        }
        return bytes.ToArray();
    }

    public static string byteArrayToString(byte[] data)
    {
        if (0 != data.Length)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                if (i == data.Length - 1)
                {
                    sb.Append($"{data[i]}");
                    continue;
                }
                sb.Append($"{data[i]},");
            }
            return sb.ToString();
        }
        else
        {
            Debug.LogError("Invalid data : data length is 0...");
            return string.Empty;
        }
    }
}


[CustomEditor(typeof(CameraShakerTool))]
public class CameraShakerContainerCustomEditor : Editor
{
    ShakeData shakeData = new ShakeData();
    SerializedProperty _shakerDataList;

    SerializedProperty _tableId;
    SerializedProperty _useTable;
    SerializedProperty _scaledTime;
    SerializedProperty _shakeCameras;
    SerializedProperty _shakeCanvases;
    SerializedProperty _shakeObjects;
    SerializedProperty _unlimitedDuration;
    SerializedProperty _totalDuration;
    SerializedProperty _fadeInDuration;
    SerializedProperty _fadeOutDuration;
    SerializedProperty _magnitude;
    SerializedProperty _magnitudeNoise;
    SerializedProperty _magnitudeCurve;
    SerializedProperty _roughness;
    SerializedProperty _roughnessNoise;
    SerializedProperty _roughnessCurve;
    SerializedProperty _positionalInfluence;
    SerializedProperty _positionalInverts;
    SerializedProperty _rotationalInfluence;
    SerializedProperty _rotationalInverts;
    SerializedProperty _randomSeed;

    void OnEnable()
    {
        shakeData = new ShakeData();
        _shakerDataList = serializedObject.FindProperty("_shakeDatalist");
        _tableId = serializedObject.FindProperty("_tableId");
        _useTable = serializedObject.FindProperty("_useTable");
        _scaledTime = serializedObject.FindProperty("_scaledTime");
        _shakeCameras = serializedObject.FindProperty("_shakeCameras");
        _shakeCanvases = serializedObject.FindProperty("_shakeCanvases");
        _shakeObjects = serializedObject.FindProperty("_shakeObjects");
        _unlimitedDuration = serializedObject.FindProperty("_unlimitedDuration");
        _totalDuration = serializedObject.FindProperty("_totalDuration");
        _fadeInDuration = serializedObject.FindProperty("_fadeInDuration");
        _fadeOutDuration = serializedObject.FindProperty("_fadeOutDuration");
        _magnitude = serializedObject.FindProperty("_magnitude");
        _magnitudeNoise = serializedObject.FindProperty("_magnitudeNoise");
        _magnitudeCurve = serializedObject.FindProperty("_magnitudeCurve");
        _roughness = serializedObject.FindProperty("_roughness");
        _roughnessNoise = serializedObject.FindProperty("_roughnessNoise");
        _roughnessCurve = serializedObject.FindProperty("_roughnessCurve");
        _positionalInfluence = serializedObject.FindProperty("_positionalInfluence");
        _positionalInverts = serializedObject.FindProperty("_positionalInverts");
        _rotationalInfluence = serializedObject.FindProperty("_rotationalInfluence");
        _rotationalInverts = serializedObject.FindProperty("_rotationalInverts");
        _randomSeed = serializedObject.FindProperty("_randomSeed");
    }
    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        serializedObject.Update();
        EditorGUILayout.PropertyField(_shakerDataList, new GUIContent("SHAKE TABLE DATA LIST"));
        GUILayout.Space(10);
        Rect rect = EditorGUILayout.GetControlRect(false, 2);
        rect.height = 2;
        EditorGUI.DrawRect(rect, Color.cyan);
        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.normal.textColor = Color.green;
        labelStyle.fontSize = 15;
        GUILayout.Label("CAMERA SHAKE SIMULATOR", labelStyle);
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(_tableId, new GUIContent("TABLE ID"));
        EditorGUILayout.PropertyField(_useTable, new GUIContent("USE TABLE"));
        if (false == _useTable.boolValue)
        {
            EditorGUILayout.PropertyField(_scaledTime, new GUIContent("SCALE TIME"));
            EditorGUILayout.PropertyField(_shakeCameras, new GUIContent("CAMERA"));
            EditorGUILayout.PropertyField(_shakeCanvases, new GUIContent("CANVASES"));
            EditorGUILayout.PropertyField(_shakeObjects, new GUIContent("OBJECTS"));
            EditorGUILayout.PropertyField(_unlimitedDuration, new GUIContent("UNLIMITDURATION"));
            if (false == _unlimitedDuration.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(15);
                    EditorGUILayout.PropertyField(_totalDuration, new GUIContent("TOTAL DURATION"));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.PropertyField(_fadeInDuration, new GUIContent("FADE IN DURATION"));
            EditorGUILayout.PropertyField(_fadeOutDuration, new GUIContent("FADE OUT DURATION"));
            EditorGUILayout.PropertyField(_magnitude, new GUIContent("MAGNITUDE"));
            EditorGUILayout.PropertyField(_magnitudeNoise, new GUIContent("MAGNITUDE NOISE"));
            EditorGUILayout.PropertyField(_magnitudeCurve, new GUIContent("MAGNITUDE CURVE"));
            EditorGUILayout.PropertyField(_roughness, new GUIContent("ROUGHNESS"));
            EditorGUILayout.PropertyField(_roughnessNoise, new GUIContent("ROUGHNESS NOISE"));
            EditorGUILayout.PropertyField(_roughnessCurve, new GUIContent("ROUGHNESS CURVE"));
            EditorGUILayout.PropertyField(_positionalInfluence, new GUIContent("POSITIONAL INFLUENCE"));
            EditorGUILayout.PropertyField(_positionalInverts, new GUIContent("POSITIONAL INVERTS"));
            EditorGUILayout.PropertyField(_rotationalInfluence, new GUIContent("ROTATIONAL INFLUENCE"));
            EditorGUILayout.PropertyField(_rotationalInverts, new GUIContent("ROTATIONAL INVERTS"));
            EditorGUILayout.PropertyField(_randomSeed, new GUIContent("RANDOM SEED"));
            InvertibleAxes position = (InvertibleAxes)_positionalInverts.intValue;
            InvertibleAxes rotation = (InvertibleAxes)_rotationalInverts.intValue;
            if (-1 == _positionalInverts.intValue)
            {
                position = InvertibleAxes.X | InvertibleAxes.Y | InvertibleAxes.Z;
            }
            if (-1 == _rotationalInverts.intValue)
            {
                rotation = InvertibleAxes.X | InvertibleAxes.Y | InvertibleAxes.Z;
            }

            shakeData.SetInstancedWithProperties(
                _scaledTime.boolValue,
                _shakeCameras.boolValue,
                _shakeCanvases.boolValue,
                _shakeObjects.boolValue,
                _unlimitedDuration.boolValue,
                _magnitude.floatValue,
                _magnitudeNoise.floatValue,
                _magnitudeCurve.animationCurveValue,
                _roughness.floatValue,
                _roughnessNoise.floatValue,
                _roughnessCurve.animationCurveValue,
                _totalDuration.floatValue,
                _fadeInDuration.floatValue,
                _fadeOutDuration.floatValue,
                _positionalInfluence.vector3Value,
                position,
                _rotationalInfluence.vector3Value,
                rotation,
                _randomSeed.boolValue
                );
        }

        if (GUILayout.Button("PLAY"))
        {
            if (true == _useTable.boolValue && 0 == _tableId.intValue)
            {
                Debug.LogError("Table id is 0. please check table id...");
                return;
            }
            CameraShakerTool.CameraEditorShake(_useTable.boolValue, _tableId.intValue, shakeData);
        }
        if (GUILayout.Button("STOP"))
        {
            CameraShakerTool.StopShake();
        }
        GUILayout.Space(10);
        Rect rect2 = EditorGUILayout.GetControlRect(false, 2);
        rect2.height = 2;
        EditorGUI.DrawRect(rect2, Color.cyan);
        labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.normal.textColor = Color.green;
        labelStyle.fontSize = 15;
        GUILayout.Label("LOAD & PRINT DATA BASED ON TABLE", labelStyle);
        GUILayout.Space(10);

        if (GUILayout.Button("LOAD TABLE DATA"))
        {
            var loader = new JsonLoader();
            loader.Load();

            var data = CameraShakeData.table[_tableId.intValue];
            if (null == data)
            {
                Debug.LogError("Invalid Table id..... ");
                return;
            }
            InvertibleAxes position = (InvertibleAxes)EditorGUILayout.EnumFlagsField("POSITIONAL AXES", (InvertibleAxes)_positionalInverts.intValue);
            InvertibleAxes rotation = (InvertibleAxes)EditorGUILayout.EnumFlagsField("ROTATIONAL AXES", (InvertibleAxes)_rotationalInverts.intValue);
            AnimationCurve maginutdeCurve = EditorCurveSerializer.DeserializeCurve(EditorCurveSerializer.stringToByteArray(data.MagnitudeCurve));
            AnimationCurve roughnessCurve = EditorCurveSerializer.DeserializeCurve(EditorCurveSerializer.stringToByteArray(data.RoughnessCurve));

            _scaledTime.boolValue = data.ScaledTime;
            _shakeCameras.boolValue = data.Camera;
            _shakeCanvases.boolValue = data.Canvases;
            _shakeObjects.boolValue = data.Objects;
            _unlimitedDuration.boolValue = data.UnlimitedDuration;
            _totalDuration.floatValue = data.TotalDuration;
            _fadeInDuration.floatValue = data.FadeInDuration;
            _fadeOutDuration.floatValue = data.FadeOutDuration;
            _magnitude.floatValue = data.Magnitude;
            _magnitudeNoise.floatValue = data.MagnitudeNoise;
            _magnitudeCurve.animationCurveValue = maginutdeCurve;
            _roughness.floatValue = data.Roughness;
            _roughnessNoise.floatValue = data.RoughnessNoise;
            _roughnessCurve.animationCurveValue = roughnessCurve;
            _positionalInfluence.vector3Value = new Vector3(data.PositionalInfluence[0], data.PositionalInfluence[1], data.PositionalInfluence[2]);
            _positionalInverts.intValue = (int)CameraShakerTool.GenerateInvertibleAxes(data.PositionalInverts);
            _rotationalInfluence.vector3Value = new Vector3(data.RotationalInfluence[0], data.RotationalInfluence[1], data.RotationalInfluence[2]);
            _rotationalInverts.intValue = (int)CameraShakerTool.GenerateInvertibleAxes(data.RotationalInverts);
            _randomSeed.boolValue = data.RandomSeed;
        }

        if (GUILayout.Button("PRINT SHAKE DATA"))
        {
            string magintudeCurve = EditorCurveSerializer.byteArrayToString(EditorCurveSerializer.SerializeCurve(_magnitudeCurve.animationCurveValue));
            string roughnessCurve = EditorCurveSerializer.byteArrayToString(EditorCurveSerializer.SerializeCurve(_roughnessCurve.animationCurveValue));



            Debug.Log($"Scale Time	'{_scaledTime.boolValue.ToString().ToLower()} \n" +
                $"_shakeCameras	'{_shakeCameras.boolValue.ToString().ToLower()} \n" +
                $"_shakeCanvases	'{_shakeCanvases.boolValue.ToString().ToLower()} \n" +
                $"_shakeObjects	'{_shakeObjects.boolValue.ToString().ToLower()} \n" +
                $"_unlimitedDuration	'{_unlimitedDuration.boolValue.ToString().ToLower()} \n" +
                $"_totalDuration	{CameraShakerTool.AddDecimalPoint(_totalDuration.floatValue)} \n" +
                $"_fadeInDuration	{CameraShakerTool.NonZeroInterpolate(_fadeInDuration.floatValue)} \n" +
                $"_fadeOutDuration	{CameraShakerTool.NonZeroInterpolate(_fadeOutDuration.floatValue)} \n" +
                $"_magnitude	{CameraShakerTool.AddDecimalPoint(_magnitude.floatValue)} \n" +
                $"_magnitudeNoise	{CameraShakerTool.AddDecimalPoint(_magnitudeNoise.floatValue)} \n" +
                $"_magintudeCurve	{magintudeCurve} \n" +
                $"_roughness	{CameraShakerTool.AddDecimalPoint(_roughness.floatValue)} \n" +
                $"_roughnessNoise	{CameraShakerTool.AddDecimalPoint(_roughnessNoise.floatValue)} \n" +
                $"_roughnessCurve	{roughnessCurve} \n" +
                $"_positionalInfluence	[{CameraShakerTool.AddDecimalPoint(_positionalInfluence.vector3Value.x)},{CameraShakerTool.AddDecimalPoint(_positionalInfluence.vector3Value.y)},{CameraShakerTool.AddDecimalPoint(_positionalInfluence.vector3Value.z)}]\n" +
                $"_positionalInverts	{CameraShakerTool.InvertibleAxesToString((InvertibleAxes)_positionalInverts.intValue)}\n" +
                $"_rotationalInfluence	[{CameraShakerTool.AddDecimalPoint(_rotationalInfluence.vector3Value.x)},{CameraShakerTool.AddDecimalPoint(_rotationalInfluence.vector3Value.y)},{CameraShakerTool.AddDecimalPoint(_rotationalInfluence.vector3Value.z)}]\n" +
                $"_rotationalInverts	{CameraShakerTool.InvertibleAxesToString((InvertibleAxes)_rotationalInverts.intValue)}\n" +
                $"_randomSeed	'{_randomSeed.boolValue.ToString().ToLower()}");
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

