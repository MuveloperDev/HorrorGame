using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

[Serializable]
public class SerializeDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();
    [SerializeField]
    private List<TValue> values = new List<TValue>();

    private Dictionary<TKey, TValue> dictionary = new();

    public TValue this[TKey key]
    {
        get => dictionary[key];
        set => dictionary[key] = value;
    }

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        dictionary = new Dictionary<TKey, TValue>();

        for (int i = 0; i != Math.Min(keys.Count, values.Count); i++)
        {
            if (!dictionary.ContainsKey(keys[i]))
            {
                dictionary.Add(keys[i], values[i]);
            }
        }
    }

    public void Add(TKey key, TValue value)
    {
        dictionary[key] = value;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return dictionary.TryGetValue(key, out value);
    }

    public TValue TryGetValue(TKey key)
    {
        if (dictionary.TryGetValue(key, out TValue value))
        {
            return value;
        }
        else
        {
            Debug.LogError("Invalid key in dictionary.");
            return default;
        }
    }

    public Dictionary<TKey, TValue> ToDictionary()
    {
        return dictionary;
    }

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }
}
[CustomPropertyDrawer(typeof(SerializeDictionary<,>), true)]
public class SerializeDictionaryDrawer : PropertyDrawer
{
    private bool foldout;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var keysProperty = property.FindPropertyRelative("keys");
        return foldout ? (keysProperty.arraySize + 3) * EditorGUIUtility.singleLineHeight : EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        foldout = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), foldout, label, true);
        if (!foldout) return;

        EditorGUI.indentLevel++;
        position.y += EditorGUIUtility.singleLineHeight;

        var keysProperty = property.FindPropertyRelative("keys");
        var valuesProperty = property.FindPropertyRelative("values");

        for (int i = 0; i < keysProperty.arraySize; i++)
        {
            var keyProperty = keysProperty.GetArrayElementAtIndex(i);
            var valueProperty = valuesProperty.GetArrayElementAtIndex(i);

            var keyRect = new Rect(position.x, position.y, position.width * 0.4f, EditorGUIUtility.singleLineHeight);
            var valueRect = new Rect(position.x + position.width * 0.45f, position.y, position.width * 0.5f, EditorGUIUtility.singleLineHeight);

            if (keyProperty.propertyType == SerializedPropertyType.Enum)
            {
                EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);
            }
            else if (keyProperty.propertyType == SerializedPropertyType.Vector3Int)
            {
                keyProperty.vector3IntValue = EditorGUI.Vector3IntField(keyRect, GUIContent.none, keyProperty.vector3IntValue);
            }
            else
            {
                EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);
            }

            if (valueProperty.propertyType == SerializedPropertyType.Enum)
            {
                EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);
            }
            else if (valueProperty.propertyType == SerializedPropertyType.Vector3Int)
            {
                valueProperty.vector3IntValue = EditorGUI.Vector3IntField(valueRect, GUIContent.none, valueProperty.vector3IntValue);
            }
            else
            {
                EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);
            }

            position.y += EditorGUIUtility.singleLineHeight;
        }

        if (GUI.Button(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), "Add Entry"))
        {
            AddDefaultEntry(keysProperty, valuesProperty);
            foldout = true;  // Add this line to ensure the foldout remains open
        }

        EditorGUI.indentLevel--;
    }

    private void AddDefaultEntry(SerializedProperty keysProperty, SerializedProperty valuesProperty)
    {
        keysProperty.arraySize++;
        valuesProperty.arraySize++;

        var keyProperty = keysProperty.GetArrayElementAtIndex(keysProperty.arraySize - 1);
        var valueProperty = valuesProperty.GetArrayElementAtIndex(valuesProperty.arraySize - 1);

        if (keyProperty.propertyType == SerializedPropertyType.String)
        {
            keyProperty.stringValue = GetUniqueStringKey(keysProperty);
        }
        else if (keyProperty.propertyType == SerializedPropertyType.Integer)
        {
            keyProperty.intValue = GetUniqueIntKey(keysProperty);
        }
        else if (keyProperty.propertyType == SerializedPropertyType.Enum)
        {
            keyProperty.enumValueIndex = GetUniqueEnumKey(keysProperty);
        }
        else if (keyProperty.propertyType == SerializedPropertyType.Vector3Int)
        {
            keyProperty.vector3IntValue = GetUniqueVector3IntKey(keysProperty);
        }
        // Add other types as necessary

        // Set default value for the value property
        if (valueProperty.propertyType == SerializedPropertyType.ObjectReference)
        {
            valueProperty.objectReferenceValue = null;
        }
        else if (valueProperty.propertyType == SerializedPropertyType.Enum)
        {
            valueProperty.enumValueIndex = 0;
        }
        else
        {
            // Set default value for other types if needed
        }
    }

    private string GetUniqueStringKey(SerializedProperty keysProperty)
    {
        int index = 1;
        string newKey = "New Key " + index;
        while (KeyExists(keysProperty, newKey))
        {
            index++;
            newKey = "New Key " + index;
        }
        return newKey;
    }

    private int GetUniqueIntKey(SerializedProperty keysProperty)
    {
        int newKey = 1;
        while (KeyExists(keysProperty, newKey))
        {
            newKey++;
        }
        return newKey;
    }

    private int GetUniqueEnumKey(SerializedProperty keysProperty)
    {
        int newKey = 0;
        while (EnumKeyExists(keysProperty, newKey))
        {
            newKey++;
        }
        return newKey;
    }

    private Vector3Int GetUniqueVector3IntKey(SerializedProperty keysProperty)
    {
        Vector3Int newKey = Vector3Int.zero;
        while (KeyExists(keysProperty, newKey))
        {
            newKey += Vector3Int.one;
        }
        return newKey;
    }

    private bool KeyExists(SerializedProperty keysProperty, string key)
    {
        for (int i = 0; i < keysProperty.arraySize; i++)
        {
            if (keysProperty.GetArrayElementAtIndex(i).stringValue == key)
            {
                return true;
            }
        }
        return false;
    }

    private bool KeyExists(SerializedProperty keysProperty, int key)
    {
        for (int i = 0; i < keysProperty.arraySize; i++)
        {
            if (keysProperty.GetArrayElementAtIndex(i).intValue == key)
            {
                return true;
            }
        }
        return false;
    }

    private bool KeyExists(SerializedProperty keysProperty, Vector3Int key)
    {
        for (int i = 0; i < keysProperty.arraySize; i++)
        {
            if (keysProperty.GetArrayElementAtIndex(i).vector3IntValue == key)
            {
                return true;
            }
        }
        return false;
    }

    private bool EnumKeyExists(SerializedProperty keysProperty, int enumValueIndex)
    {
        for (int i = 0; i < keysProperty.arraySize; i++)
        {
            if (keysProperty.GetArrayElementAtIndex(i).enumValueIndex == enumValueIndex)
            {
                return true;
            }
        }
        return false;
    }
}