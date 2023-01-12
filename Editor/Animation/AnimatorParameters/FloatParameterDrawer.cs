#nullable enable
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(FloatParameter))]
    public class FloatParameterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = ExtraEditorGUIUtility.LoadUnityExtrasUxml(nameof(FloatParameter)).CloneTree();

            var valueRoot = tree.Q(nameof(FloatParameter.value));
            var floatField = valueRoot.Q<FloatField>();
            var slider = valueRoot.Q<Slider>();

            var foldout = tree.Q<Foldout>();
            var propertyField = foldout.Q<PropertyField>();
            propertyField.RegisterValueChangeCallback(e =>
            {
                var property = e.changedProperty.Copy();
                property.NextVisible(true);
                floatField.label = slider.label = string.IsNullOrEmpty(property.stringValue) ? " " : property.stringValue;
            });

            var hasMinMaxToggle = foldout.Q<PropertyField>(nameof(FloatParameter.hasMinMax));
            hasMinMaxToggle.RegisterValueChangeCallback(e => OnHasMinMaxChanged(e.changedProperty.boolValue));
            OnHasMinMaxChanged(property.FindPropertyRelative($"_{nameof(FloatParameter.hasMinMax)}").boolValue);

            var min = foldout.Q<PropertyField>(nameof(FloatParameter.min));
            min.RegisterValueChangeCallback(e => OnMinChanged(e.changedProperty.floatValue));
            OnMinChanged(property.FindPropertyRelative($"_{nameof(FloatParameter.min)}").floatValue);

            var max = foldout.Q<PropertyField>(nameof(FloatParameter.max));
            max.RegisterValueChangeCallback(e => OnMaxChanged(e.changedProperty.floatValue));
            OnMaxChanged(property.FindPropertyRelative($"_{nameof(FloatParameter.max)}").floatValue);

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

            void OnHasMinMaxChanged(bool newValue)
            {
                if (newValue)
                {
                    floatField.style.display = DisplayStyle.None;
                    slider.style.display = DisplayStyle.Flex;
                    slider.value = floatField.value;
                }
                else
                {
                    floatField.style.display = DisplayStyle.Flex;
                    slider.style.display = DisplayStyle.None;
                    floatField.value = slider.value;
                }
            }

            void OnMinChanged(float newValue) => slider.highValue = Mathf.Max(slider.lowValue = newValue, slider.highValue);
            void OnMaxChanged(float newValue) => slider.highValue = Mathf.Max(slider.lowValue, newValue);
        }
    }
}