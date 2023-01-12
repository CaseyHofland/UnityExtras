#nullable enable
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(BoolParameter))]
    public class BoolParameterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = ExtraEditorGUIUtility.LoadUnityExtrasUxml(nameof(BoolParameter)).CloneTree();

            var valueRoot = tree.Q(nameof(TriggerParameter.hideSettings));
            var valueLabel = valueRoot.Q<Label>();

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
