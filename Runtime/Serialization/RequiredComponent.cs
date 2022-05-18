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

        public T GetComponent(GameObject gameObject)
        {
            if (component == null)
            {
                component = _nonResetable.value != null ? _nonResetable.value : gameObject.AddComponent<T>();
            }

            return _nonResetable.value = component;
        }
    }
}
