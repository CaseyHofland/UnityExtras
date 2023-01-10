#nullable enable
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(TriggerParameter))]
    public class TriggerParameterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = ExtraEditorGUIUtility.LoadUnityExtrasUxml(nameof(TriggerParameter)).CloneTree();

            var valueRoot = tree.Q(nameof(TriggerParameter.hideSettings));
            var valueLabel = valueRoot.Q<Label>();

            var setTriggerButton = valueRoot.Q<Button>(nameof(Animator.SetTrigger));
            setTriggerButton.clicked += () =>
            {
                var target = (Component)property.serializedObject.targetObject;
                var animator = target.GetComponent<Animator>();
                animator.SetTrigger((TriggerParameter)property.boxedValue);
            };

            var resetTriggerButton = valueRoot.Q<Button>(nameof(Animator.ResetTrigger));
            resetTriggerButton.clicked += () =>
            {
                var target = (Component)property.serializedObject.targetObject;
                var animator = target.GetComponent<Animator>();
                animator.ResetTrigger((TriggerParameter)property.boxedValue);
            };

            var foldout = tree.Q<Foldout>();
            var propertyField = foldout.Q<PropertyField>();
            propertyField.RegisterValueChangeCallback(e =>
            {
                var property = e.changedProperty.Copy();
                property.NextVisible(true);
                valueLabel.text = property.stringValue;
            });

            var hideSettings = property.FindAutoPropertyRelative(nameof(FloatParameter.hideSettings)).boolValue;
            if (hideSettings)
            {
                foldout.style.display = DisplayStyle.None;
                valueRoot.style.position = Position.Relative;
            }
            else
            {
                foldout.style.display = DisplayStyle.Flex;
                valueRoot.style.position = Position.Absolute;
            }

            return tree;
        }
    }
}