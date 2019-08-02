using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EnumSelectorAttribute))]
public class EnumSelectorPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Prevent values being overwritten during multiple-selection
        UnityEditor.EditorGUI.BeginChangeCheck();
        int values = UnityEditor.EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        if (UnityEditor.EditorGUI.EndChangeCheck())
        {
            property.intValue = values;
        }
    }
}
