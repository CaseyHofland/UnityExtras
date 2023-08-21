#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtras
{
    public abstract class Instance<T> : MonoBehaviour where T : Instance<T>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SubsystemRegistration()
        {
            instances.Clear();
        }

        private static List<T> instances = new();

        /// <summary>
        /// Return the current instance.
        /// </summary>
        public static T? current => instances.Count > 0 ? instances[instances.Count - 1] : null;

        protected virtual void OnEnable()
        {
            instances.Add((T)this);
            SetState();
        }

        protected virtual void OnDisable()
        {
            var wasCurrent = current == this;
            instances.Remove((T)this);
            if (wasCurrent && current != null)
            {
                current.SetState();
            }
        }

        /// <summary>
        /// Sets the instance state.
        /// </summary>
        public abstract void SetState();
    }
}
