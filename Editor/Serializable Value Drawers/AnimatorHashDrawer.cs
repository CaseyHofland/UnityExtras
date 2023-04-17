#nullable enable
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorHash))]
    public class AnimatorHashDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var textField = new TextField(property.displayName);
            textField.BindProperty(property.FindPropertyRelative($"_{nameof(AnimatorHash.name)}"));
            textField.AddToClassList(TextField.alignedFieldUssClassName);
            return textField;
        }
    }
}
