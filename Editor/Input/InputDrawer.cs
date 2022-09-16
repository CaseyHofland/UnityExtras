#nullable enable
using UnityEditor;
using UnityEngine;

namespace UnityExtras.InputSystem.Editor
{
    [CustomPropertyDrawer(typeof(Input))]
    public class InputDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position.y = position.yMin;
            position.height = EditorGUIUtility.singleLineHeight;

            // Collect properties.
            var enableOnStartProperty = property.FindPropertyRelative(nameof(Input.enableOnStart));

            var inputActionPropertyProperty = property.FindPropertyRelative(nameof(Input.inputActionProperty));
            SerializedProperty m_UseReference = inputActionPropertyProperty.FindPropertyRelative(nameof(m_UseReference));
            SerializedProperty m_Action = inputActionPropertyProperty.FindPropertyRelative(nameof(m_Action));
            SerializedProperty m_Reference = inputActionPropertyProperty.FindPropertyRelative(nameof(m_Reference));

            // Draw the enable on construction toggle.
            var enableOnStartPosition = new Rect(position);
            enableOnStartPosition.x += EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 18f;
            enableOnStartPosition.width = 96f + EditorGUI.indentLevel * 18f;
            enableOnStartPosition.height = EditorGUIUtility.singleLineHeight;

            var defaultEnabledLabel = new GUIContent(enableOnStartProperty.displayName, "Should the action be enabled on start?");
            EditorGUI.LabelField(enableOnStartPosition, defaultEnabledLabel);

            enableOnStartPosition.x += 96f;
            enableOnStartPosition.width = position.width - EditorGUIUtility.labelWidth + EditorGUI.indentLevel * 18f - 96f;
            EditorGUI.PropertyField(enableOnStartPosition, enableOnStartProperty, GUIContent.none);

            // Draw the use reference popup.
            var useReferencePosition = new Rect(enableOnStartPosition);
            useReferencePosition.x += 18f;
            useReferencePosition.width -= 18f;
            var selectedIndex = m_UseReference.boolValue ? 1 : 0;
            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUI.Popup(useReferencePosition, selectedIndex, new[] { "Use Action", "Use Reference" });
            if (EditorGUI.EndChangeCheck())
            {
                m_UseReference.boolValue = selectedIndex == 1;
            }

            // Draw the label.
            var foldoutPosition = new Rect(position);
            foldoutPosition.width = EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 18f;
            property.isExpanded = EditorGUI.Foldout(foldoutPosition, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                position.y += EditorGUIUtility.singleLineHeight + 2f;
                EditorGUI.indentLevel++;

                if (m_UseReference.boolValue)
                {
                    position.height = EditorGUI.GetPropertyHeight(m_Reference, true);
                    EditorGUI.PropertyField(position, m_Reference, true);
                }
                else
                {
                    position.height = EditorGUI.GetPropertyHeight(m_Action, true);
                    EditorGUI.PropertyField(position, m_Action, true);
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = base.GetPropertyHeight(property, label);

            if (property.isExpanded)
            {
                var inputActionPropertyProperty = property.FindPropertyRelative(nameof(Input.inputActionProperty));
                SerializedProperty m_UseReference = inputActionPropertyProperty.FindPropertyRelative(nameof(m_UseReference));
                SerializedProperty m_Action = inputActionPropertyProperty.FindPropertyRelative(nameof(m_Action));
                SerializedProperty m_Reference = inputActionPropertyProperty.FindPropertyRelative(nameof(m_Reference));

                height += m_UseReference.boolValue ? EditorGUI.GetPropertyHeight(m_Reference, true) : EditorGUI.GetPropertyHeight(m_Action, true);
                height += 2f;
            }

            return height;
        }
    }
}
