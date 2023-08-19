#nullable enable
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(RelativeDirection))]
    public class RelativeDirectionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var pivotField = new PropertyField(property.FindPropertyRelative(nameof(RelativeDirection.pivot)));
            var directionField = new PropertyField(property.FindPropertyRelative(nameof(RelativeDirection.direction)))
            {
                label = property.displayName,
                tooltip = property.tooltip,
            };

            directionField.RegisterCallback<GeometryChangedEvent>(_ => directionField.Q<Label>().text = property.displayName);

            directionField.style.position = Position.Absolute;
            directionField.style.width = new Length(100f, LengthUnit.Percent);
            directionField.style.left = 18f;
            directionField.style.paddingRight = 18f;

            var foldout = new Foldout();
            foldout.contentContainer.Add(pivotField);

            var root = new VisualElement();
            root.Add(foldout);
            root.Add(directionField);
            return root;
        }
    }
}