#nullable enable
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(Direction))]
    public class DirectionDrawer : PropertyDrawer
    {
        private bool changed;
        private Vector3 unscaledValue;

        private const float buttonWidth = 80f;
        private static readonly GUIContent buttonContent = new("Normalize", "Press the button (or Enter) to normalize and save the new direction.");

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty _value = property.FindPropertyRelative(nameof(_value));

            var valueField = new Vector3Field(property.displayName)
            {
                tooltip = property.tooltip,
                value = _value.vector3Value,
            };
            valueField.style.flexGrow = 1f;
            valueField.AddToClassList(BaseField<object>.alignedFieldUssClassName);

            var inputFields = valueField.Query(className: FloatField.inputUssClassName);
            var crimson = new Color(87f / 255f, 15f / 255f, 20f / 255f);
            var originalColor = inputFields.First().style.backgroundColor;

            valueField.RegisterValueChangedCallback(_ => inputFields.ForEach(input => input.style.backgroundColor = crimson));

            var button = new Button(() =>
            {
                _value.vector3Value = valueField.value.magnitude > Vector3.kEpsilon ? valueField.value.normalized : Vector3.forward;
                property.serializedObject.ApplyModifiedProperties();

                valueField.SetValueWithoutNotify(_value.vector3Value);
                inputFields.ForEach(input => input.style.backgroundColor = originalColor);
            })
            {
                text = buttonContent.text,
                tooltip = buttonContent.tooltip,
            };
            button.style.width = buttonWidth;

            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.Add(valueField);
            root.Add(button);
            return root;
        }

        //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        //{
        //    EditorGUI.BeginProperty(position, label, property);

        //    SerializedProperty _value = property.FindPropertyRelative(nameof(_value));

        //    //if (property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true))
        //    //{
        //    //    var rotation = Quaternion.LookRotation(_value.vector3Value);
        //    //    using (var eulerCheck = new EditorGUI.ChangeCheckScope())
        //    //    {
        //    //        rotation.eulerAngles = EditorGUI.Vector3Field(position, label, rotation.eulerAngles);

        //    //        if (eulerCheck.changed)
        //    //        {
        //    //            _value.vector3Value = rotation * Vector3.forward;
        //    //            _value.serializedObject.ApplyModifiedProperties();
        //    //        }
        //    //    }
        //    //}
        //    //else
        //    {
        //        var tmp = GUI.color;
        //        if (changed)
        //        {
        //            GUI.color = Color.red;
        //            Debug.Log("Cooolz");
        //        }
        //        else
        //        {
        //            unscaledValue = _value.vector3Value;
        //        }

        //        position.width -= (buttonWidth + 1f);
        //        using (var unscaledValueCheck = new EditorGUI.ChangeCheckScope())
        //        {
        //            unscaledValue = EditorGUI.Vector3Field(position, label, unscaledValue);
        //            changed |= unscaledValueCheck.changed;
        //        }

        //        GUI.color = tmp;

        //        position.x += position.width - EditorGUI.indentLevel * 16f + 2f;
        //        position.width = buttonWidth + EditorGUI.indentLevel * 16f;

        //        if (GUI.Button(position, buttonContent) || Event.current.keyCode == KeyCode.Return)
        //        {
        //            _value.vector3Value = unscaledValue.magnitude > Vector3.kEpsilon ? unscaledValue.normalized : Vector3.forward;
        //            property.serializedObject.ApplyModifiedProperties();
        //            changed = false;
        //            Debug.Log("False");
        //        }
        //    }

        //    EditorGUI.EndProperty();
        //}
    }
}