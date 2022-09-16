#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
    /// <summary>Wrapper for ensuring <typeparamref name="T"/> won't be reset through Unity serialization.</summary>
    [Serializable]
    public struct NonResetable<T> : ISerializationCallbackReceiver
    {
        public T value;
        private T _dump;

        [SerializeField] [HideInInspector] private bool valid;

        public static implicit operator T(NonResetable<T> nonResetable) => nonResetable.value;
        public static implicit operator NonResetable<T>(T value) => new() { value = value };

        public void OnBeforeSerialize()
        {
            _dump = value;
        }

        public void OnAfterDeserialize()
        {
            if (!valid)
            {
                value = _dump;
                valid = true;
            }
        }
    }
}
