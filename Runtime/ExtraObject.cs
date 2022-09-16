#nullable enable
using UnityEngine;

namespace UnityExtras
{
    /// <summary>Extra helper methods for <see cref="Object"/>.</summary>
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

        private static bool DestroyViaEditor(Object? obj)
        {
#if UNITY_EDITOR
            if (obj != null && (!Application.IsPlaying(obj) || isQuitting))
            {
                if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    UnityEditor.EditorApplication.delayCall += () => Object.DestroyImmediate(obj);
                }

                return true;
            }
#endif

            return false;
        }

        /// <summary>Removes a <see cref="GameObject"/>, <see cref="Component"/> or Asset safely via the editor if not in playmode. Incurs no extra performance cost at runtime.</summary>
        /// <param name="obj">The <see cref="Object"/> to destroy.</param>
        public static void DestroySafe(Object? obj)
        {
#if UNITY_EDITOR
            if (!DestroyViaEditor(obj))
#endif
            {
                Object.Destroy(obj);
            }
        }

        /// <summary>Destroys the <see cref="Object"/> <paramref name="obj"/> immediately safely via the editor if not in playmode. Incurs no extra performance cost at runtime. You are strongly recommended to use <see cref="DestroySafe"/> instead.</summary>
        /// <param name="obj"><see cref="Object"/> to destroy.</param>
        public static void DestroyImmediateSafe(Object? obj)
        {
#if UNITY_EDITOR
            if (!DestroyViaEditor(obj))
#endif
            {
                Object.DestroyImmediate(obj);
            }
        }
    }
}
