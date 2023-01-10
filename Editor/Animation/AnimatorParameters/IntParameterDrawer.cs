#nullable enable
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(IntParameter))]
    public class IntParameterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = ExtraEditorGUIUtility.LoadUnityExtrasUxml(nameof(IntParameter)).CloneTree();

            var valueRoot = tree.Q(nameof(FloatParameter.value));
            var intField = valueRoot.Q<IntegerField>();
            var sliderInt = valueRoot.Q<SliderInt>();

            var foldout = tree.Q<Foldout>();
            var propertyField = foldout.Q<PropertyField>();
            propertyField.RegisterValueChangeCallback(e =>
            {
                var property = e.changedProperty.Copy();
                property.NextVisible(true);
                intField.label = sliderInt.label = string.IsNullOrEmpty(property.stringValue) ? " " : property.stringValue;
            });

            var hasMinMaxToggle = foldout.Q<PropertyField>(nameof(FloatParameter.hasMinMax));
            hasMinMaxToggle.RegisterValueChangeCallback(e => OnHasMinMaxChanged(e.changedProperty.boolValue));
            OnHasMinMaxChanged(property.FindPropertyRelative($"_{nameof(FloatParameter.hasMinMax)}").boolValue);

            var min = foldout.Q<PropertyField>(nameof(FloatParameter.min));
            min.RegisterValueChangeCallback(e => OnMinChanged(e.changedProperty.intValue));
            OnMinChanged(property.FindPropertyRelative($"_{nameof(FloatParameter.min)}").intValue);

            var max = foldout.Q<PropertyField>(nameof(FloatParameter.max));
            max.RegisterValueChangeCallback(e => OnMaxChanged(e.changedProperty.intValue));
            OnMaxChanged(property.FindPropertyRelative($"_{nameof(FloatParameter.max)}").intValue);

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
                    intField.style.display = DisplayStyle.None;
                    sliderInt.style.display = DisplayStyle.Flex;
                    sliderInt.value = intField.value;
                }
                else
                {
                    intField.style.display = DisplayStyle.Flex;
                    sliderInt.style.display = DisplayStyle.None;
                    intField.value = sliderInt.value;
                }
            }

            void OnMinChanged(int newValue) => sliderInt.highValue = Mathf.Max(sliderInt.lowValue = newValue, sliderInt.highValue);
            void OnMaxChanged(int newValue) => sliderInt.highValue = Mathf.Max(sliderInt.lowValue, newValue);
        }
    }
}
