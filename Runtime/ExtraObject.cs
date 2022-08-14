#nullable enable
using UnityEngine;

namespace UnityExtras
{
    public class ExtraObject
    {
        private static bool isQuitting;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad()
        {
            isQuitting = false;
            Application.quitting -= Quitting;
            Application.quitting += Quitting;
        }

        private static void Quitting()
        {
            Application.quitting -= Quitting;
            isQuitting = true;
        }

        private static bool DestroyViaEditor(Object? @object)
        {
#if UNITY_EDITOR
            if (@object != null && (!Application.IsPlaying(@object) || isQuitting))
            {
                if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    UnityEditor.EditorApplication.delayCall += () => Object.DestroyImmediate(@object);
                }

                return true;
            }
#endif

            return false;
        }

        public static void DestroySafe(Object? @object)
        {
            if (!DestroyViaEditor(@object))
            {
                Object.Destroy(@object);
            }
        }

        public static void DestroyImmediateSafe(Object? @object)
        {
            if (!DestroyViaEditor(@object))
            {
                Object.DestroyImmediate(@object);
            }
        }
    }
}
