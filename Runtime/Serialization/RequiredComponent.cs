#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
    /// <summary>Wrapper for ensuring <typeparamref name="T"/> will be added to a <see cref="GameObject"/> as a dependency. May be used for <see cref="Component"/> of which multiple may be present on the same <see cref="GameObject"/>.</summary>
    [Serializable]
    public struct RequiredComponent<T> 
        where T : Component
    {
        [SerializeField][HideInInspector] private NonResetable<T?> _nonResetable;
        public T? component;

        public static implicit operator T?(RequiredComponent<T> requiredComponent) => requiredComponent.component;

        public T GetComponent(GameObject gameObject) => GetComponent(gameObject, default);
        public T GetComponent(GameObject gameObject, HideFlags hideFlags)
        {
            if (component == null)
            {
                component = _nonResetable.value != null ? _nonResetable.value : gameObject.AddComponent<T>();
                component.hideFlags = hideFlags;
            }

            return _nonResetable.value = component;
        }
    }
}
