#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
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
