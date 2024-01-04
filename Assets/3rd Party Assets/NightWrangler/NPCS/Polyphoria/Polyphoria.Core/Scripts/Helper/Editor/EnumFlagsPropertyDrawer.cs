using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        EditorGUI.showMixedValue = _property.hasMultipleDifferentValues;

        EditorGUI.BeginChangeCheck();
        int newValue = EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
        if (EditorGUI.EndChangeCheck())
        {
            _property.intValue = newValue;
        }
        
    }
}