#nullable enable
using UnityEditor;
using UnityEngine;

namespace UnityExtras.Editor
{
    public sealed class ExtraEditorGUILayout
    {
        public static Object ScriptField(Object target)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                var monoScript = target is MonoBehaviour monoBehaviour
                    ? MonoScript.FromMonoBehaviour(monoBehaviour)
                    : target is ScriptableObject scriptableObject
                    ? MonoScript.FromScriptableObject(scriptableObject)
                    : throw new System.ArgumentException($"{target} needs to be either a {nameof(MonoBehaviour)} or {nameof(ScriptableObject)}.");
                return EditorGUILayout.ObjectField("Script", monoScript, target.GetType(), false);
            }
        }

        public static SerializedProperty DrawInspectorGUI(SerializedProperty from) => DrawInspectorGUI(from, from);
        public static SerializedProperty DrawInspectorGUI(SerializedProperty from, SerializedProperty until)
        {
            from = from.Copy();
            while (from.NextVisible(false) && !Same(from, until))
            {
                EditorGUILayout.PropertyField(from, from.hasVisibleChildren);
            }

            return from;

            static bool Same(SerializedProperty lhs, SerializedProperty rhs) => lhs.propertyPath == rhs.propertyPath
                && lhs.propertyType == rhs.propertyType
                && lhs.serializedObject == rhs.serializedObject;
                //&& lhs.name == rhs.name
                //&& lhs.type == rhs.type
        }
    }
}
