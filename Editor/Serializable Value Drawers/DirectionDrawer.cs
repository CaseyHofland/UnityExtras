#nullable enable
using UnityEditor;
using UnityEngine;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(Direction))]
    public class DirectionDrawer : PropertyDrawer
    {
        private bool changed;
        private Vector3 unscaledValue;

        private const float buttonWidth = 80f;
        private static readonly GUIContent buttonContent = new("Normalize", "Press the button (or Enter) to normalize and save the new direction.");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty _value = property.FindPropertyRelative(nameof(_value));

            //if (property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true))
            //{
            //    var rotation = Quaternion.LookRotation(_value.vector3Value);
            //    using (var eulerCheck = new EditorGUI.ChangeCheckScope())
            //    {
            //        rotation.eulerAngles = EditorGUI.Vector3Field(position, label, rotation.eulerAngles);

            //        if (eulerCheck.changed)
            //        {
            //            _value.vector3Value = rotation * Vector3.forward;
            //            _value.serializedObject.ApplyModifiedProperties();
            //        }
            //    }
            //}
            //else
            {
                var tmp = GUI.color;
                if (changed)
                {
                    GUI.color = Color.red;
                }
                else
                {
                    unscaledValue = _value.vector3Value;
                }

                position.width -= (buttonWidth + 1f);
                using (var unscaledValueCheck = new EditorGUI.ChangeCheckScope())
                {
                    unscaledValue = EditorGUI.Vector3Field(position, label, unscaledValue);
                    changed |= unscaledValueCheck.changed;
                }

                GUI.color = tmp;

                position.x += position.width - EditorGUI.indentLevel * 16f + 2f;
                position.width = buttonWidth + EditorGUI.indentLevel * 16f;

                if (GUI.Button(position, buttonContent) || Event.current.keyCode == KeyCode.Return)
                {
                    _value.vector3Value = unscaledValue.magnitude > Vector3.kEpsilon ? unscaledValue.normalized : Vector3.forward;
                    property.serializedObject.ApplyModifiedProperties();
                    changed = false;
                }
            }

            EditorGUI.EndProperty();
        }
    }
}