using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using static Unity.PlasticSCM.Editor.WebApi.CredentialsResponse;
using Newtonsoft.Json;
using System;
using System.Collections;

public class JsonToCSharp : EditorWindow
{
    [MenuItem("Tools/Generate C# Classes from JSON Files")]
    public static void ShowWindow()
    {
        GenerateClassesFromJson();
        //EditorWindow.GetWindow(typeof(JsonToCSharp));
    }
    private void OnGUI()
    {
        GUILayout.Label("Generate C# Classes from JSON Files", EditorStyles.boldLabel);
        if (GUILayout.Button("Generate"))
        {
            GenerateClassesFromJson();
        }
    }

    private static void GenerateClassesFromJson()
    {
        string resourcesPath = Path.Combine(Application.dataPath, "Resources", "ResourcesTable");
        DirectoryInfo directoryInfo = new DirectoryInfo(resourcesPath);

        foreach (FileInfo file in directoryInfo.GetFiles("*.json"))
        {
            string className = Path.GetFileNameWithoutExtension(file.Name);

            JArray jsonArray;
            try
            {
                jsonArray = JArray.Parse(File.ReadAllText(file.FullName));
            }
            catch (JsonReaderException)
            {
                Debug.LogError($"Invalid JSON file: {file.FullName}");
                continue;
            }

            string classContent = GenerateClass(className, jsonArray);

            string csharpFilePath = Path.Combine("Assets/Scripts/Tables", className + ".cs");
            Debug.Log($"Generate Successed {csharpFilePath}");
            File.WriteAllText(csharpFilePath, classContent);
        }

        AssetDatabase.Refresh();
    }

    private static string GenerateClass(string className, JArray jsonArray)
    {
        JObject firstObject = (JObject)jsonArray.FirstOrDefault();
        if (firstObject == null)
        {
            Debug.LogError($"Empty JSON array for class {className}");
            return string.Empty;
        }

        List<string> properties = new List<string>();
        foreach (var property in firstObject.Properties())
        {
            string type = "object";

            if (property.Value.Type == JTokenType.String)
            {
                type = "string";
            }
            else if (property.Value.Type == JTokenType.Integer)
            {
                type = "int";
            }
            else if (property.Value.Type == JTokenType.Float)
            {
                type = "float";
            }
            else if (property.Value.Type == JTokenType.Boolean)
            {
                type = "bool";
            }
            else if (property.Value.Type == JTokenType.Array)
            {
                type = GetArrayType((JArray)property.Value);
            }

            properties.Add($"    public {type} {property.Name};");
        }

        string classTemplate = $@"using System.Collections.Generic;
using Enum;
[System.Serializable]
public class {className}
{{
{string.Join("\n", properties)}
    public static Dictionary<int, {className}> table = new Dictionary<int, {className}> ();   
}}";

        return classTemplate;
    }

    private static string GetArrayType(JArray array)
    {
        JTokenType arrayType = array.FirstOrDefault()?.Type ?? JTokenType.Null;
        if (arrayType == JTokenType.Array)
        {
            // 이 배열은 다차원 배열입니다.
            return $"{GetArrayType((JArray)array.First())}[]";
        }
        else if (arrayType == JTokenType.Integer)
        {
            return "int[]";
        }
        else if (arrayType == JTokenType.Float)
        {
            return "float[]";
        }
        else if (arrayType == JTokenType.String)
        {
            return "string[]";
        }
        else if (arrayType == JTokenType.Boolean)
        {
            return "bool[]";
        }
        else
        {
            throw new NotSupportedException($"지원되지 않는 배열 요소 형식: {arrayType}");
        }
    }
}