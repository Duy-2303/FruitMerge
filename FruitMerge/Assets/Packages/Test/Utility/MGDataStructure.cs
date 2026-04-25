using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public struct MGBossData
{
    public string id;
    public float totalHP;
    public MGModuleData[] moduleDatas;
}

public struct MGModuleData
{
    public string id;
    public float hp;
}

[Serializable]
public class MGArrayGameObject
{
    public GameObject[] gameObjects;
}

[Serializable]
public class MGArrayTransform
{
    public Transform[] transforms;
}

[Serializable]
public class MGListInt
{
    public List<int> array = new List<int>();
}

[Serializable]
public class MGArrayInt
{
    public int[] array;
}

[Serializable]
public class MGArrayFloat
{
    public float[] array;

    public MGArrayFloat(params float[] array)
    {
        this.array = array;
    }
}

[Serializable]
public class MGArrayBool
{
    public bool[] array;
}

[Serializable]
public class MGPair_Transform_Float
{
    public Transform transform;
    public float number;
}

[System.Serializable]
public class MGPair_GameObject_Float
{
    public GameObject gameObject;
    public float number;
}

[Serializable]
public class MGArrayObject
{
    public object[] array;
}

[Serializable]
public class MGArrayString
{
    public string[] array;
}

[Serializable]
public class MGArraySprite
{
    public Sprite[] array;
}

[Serializable]
public class MGPair_Vector3_AudioClip
{
    public Vector3 position;
    public AudioClip audio;

    public MGPair_Vector3_AudioClip(Vector3 position, AudioClip audio = null)
    {
        this.position = position;
        this.audio = audio;
    }
}

[Serializable]
public struct FloatRange
{
    public float min;
    public float max;

    public FloatRange(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public float GetRandomWithin()
    {
        return UnityEngine.Random.Range(min, max);
    }

    public bool isDefault { get => Equals(default(FloatRange)); }
    public float RandomValue 
    {
        get
        {
            return UnityEngine.Random.Range(min, max);
        }
    }

    public bool IsInRange(float value)
    {
        return value >= min && value <= max;
    }

    //public static FloatRange zero = new FloatRange(0, 0);

    //public static bool operator ==(FloatRange a, FloatRange b)
    //{
    //    return a.min == b.min && a.max == b.max;
    //}

    //public static bool operator !=(FloatRange a, FloatRange b)
    //{
    //    return !(a == b);
    //}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(FloatRange))]
public class FloatRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var minRect = new Rect(position.x, position.y, position.width / 2 - 2, position.height);
        var maxRect = new Rect(position.x + position.width / 2 + 2, position.y, position.width / 2 - 2, position.height);

        // Draw fields - pass GUIContent.none to each so they don't draw labels
        EditorGUI.PropertyField(minRect, property.FindPropertyRelative("min"), GUIContent.none);
        EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("max"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
#endif