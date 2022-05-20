#nullable enable
using UnityEngine;

namespace UnityExtras
{
    public class ExtraObject
    {
        public static void DestroySafe(Object? @object)
        {
#if UNITY_EDITOR
            if (!Application.IsPlaying(@object))
            {
                if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    UnityEditor.EditorApplication.delayCall += () => Object.DestroyImmediate(@object);
                }
            }
            else
#endif
            {
                Object.Destroy(@object);
            }
        }
    }
}
