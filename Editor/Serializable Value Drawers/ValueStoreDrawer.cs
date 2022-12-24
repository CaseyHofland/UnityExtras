#nullable enable
using UnityEditor;
using UnityEngine;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(ValueStore<>), true)]
    public class ValueStoreDrawer : PropertyDrawer
    {
        const float storeMethodWidth = 80f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var value = property.FindAutoPropertyRelative(nameof(ValueStore<object>.value));
            var storeMethod = property.FindAutoPropertyRelative(nameof(ValueStore<object>.storeMethod));

            position.width -= storeMethodWidth - 1f;
            EditorGUI.PropertyField(position, value, label, true);

            var storeMethodPosition = new Rect(position)
            {
                x = position.x + position.width + 2f - EditorGUI.indentLevel * 16f,
                width = storeMethodWidth + EditorGUI.indentLevel * 16f,
            };
            EditorGUI.PropertyField(storeMethodPosition, storeMethod, GUIContent.none, true);

            EditorGUI.EndProperty();
        }
    }
}
