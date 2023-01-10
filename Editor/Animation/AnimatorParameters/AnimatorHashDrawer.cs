#nullable enable
using UnityEditor;
using UnityEngine.UIElements;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorHash))]
    public class AnimatorHashDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = ExtraEditorGUIUtility.LoadUnityExtrasUxml(nameof(AnimatorHash)).CloneTree();

            var textField = tree.Q<TextField>();
            textField.label = property.displayName;

            return tree;
        }
    }
}
