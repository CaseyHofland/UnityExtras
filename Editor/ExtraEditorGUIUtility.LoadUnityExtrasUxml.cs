#nullable enable
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityExtras.Editor
{
    public static partial class ExtraEditorGUIUtility
    {
        private static readonly string projectPath = Application.dataPath[..^"/Assets".Length];

        public static VisualTreeAsset LoadUnityExtrasUxml(string uxmlName)
        {
            const string rootPath = "Packages/com.caseydecoder.unityextras/Editor";
            uxmlName += ".uxml";
            uxmlName = Directory.EnumerateFiles($"{projectPath}/{rootPath}", uxmlName, SearchOption.AllDirectories).First()[$"{projectPath}/".Length..];
            return (VisualTreeAsset)EditorGUIUtility.Load(uxmlName);
        }
    }
}
