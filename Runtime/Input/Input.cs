#nullable enable
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityExtras.InputSystem
{
    [Serializable]
    public struct Input : ISerializationCallbackReceiver
    {
        public bool enableOnStart;
        public InputActionProperty inputActionProperty;
        public InputAction? action => inputActionProperty.action;

        private bool _started;

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (enableOnStart && !_started)
            {
                action?.Enable();
            }
            _started = true;
        }
    }
}
