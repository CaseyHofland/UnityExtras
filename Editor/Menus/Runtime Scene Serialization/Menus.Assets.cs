using UnityEditor;
using UnityEngine;
using Unity.RuntimeSceneSerialization;

namespace UnityExtras.Editor
{
    public static partial class Menus
    {
        [MenuItem("CONTEXT/Object/Serialize", priority = 200)]
        private static void Serialize(MenuCommand command)
        {
            var content = (string)SceneSerialization.ToJson((dynamic)command.context); // We need to cast to dynamic since SceneSerialization doesn't support polymorphism.
            ProjectWindowUtil.CreateAssetWithContent($"New{command.context.GetType().Name}State.json", content);
        }
    }
}
