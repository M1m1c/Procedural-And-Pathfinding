using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorAttributeUtils : MonoBehaviour
{
   
}

//Generates a bit mask that can have its individual bits set in editor
public sealed class EnumFlagsAttribute : PropertyAttribute
{
    public EnumFlagsAttribute() { }

    //Gets the indexes of the dropdown selections where T is the enum class
    public static List<int> GetSelectedIndexes<T>(T val) where T : IConvertible
    {
        List<int> selectedIndexes = new List<int>();
        for (int i = 0; i < System.Enum.GetValues(typeof(T)).Length; i++)
        {
            int layer = 1 << i;
            if ((Convert.ToInt32(val) & layer) != 0)
            {
                selectedIndexes.Add(i);
            }
        }
        return selectedIndexes;
    }

    //Gets the names of the dropdown selections where T is the enum class
    public static List<string> GetSelectedStrings<T>(T val) where T : IConvertible
    {
        List<string> selectedStrings = new List<string>();
        for (int i = 0; i < Enum.GetValues(typeof(T)).Length; i++)
        {
            int layer = 1 << i;
            if ((Convert.ToInt32(val) & layer) != 0)
            {
                selectedStrings.Add(Enum.GetValues(typeof(T)).GetValue(i).ToString());
            }
        }
        return selectedStrings;
    }
}

//Shows the EnumFlagsAttribute in the editor as a dropdown list of enum values.
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
    }
}
#endif
