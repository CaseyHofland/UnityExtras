#nullable enable
using UnityEditor;
using UnityEngine;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(Layer))]
    public class LayerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            SerializedProperty _value = property.FindPropertyRelative(nameof(_value));
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (_value != null)
            {
                _value.intValue = EditorGUI.LayerField(position, _value.intValue);
            }
            EditorGUI.EndProperty();
        }
    }
}
