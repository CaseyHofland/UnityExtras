#nullable enable
using UnityEditor;

namespace UnityExtras.Editor
{
    public static class SerializedObjectExtensions
    {
        public static SerializedProperty FindAutoProperty(this SerializedObject serializedObject, string propertyPath) => serializedObject.FindProperty($"<{propertyPath}>k__BackingField");
    }
}
