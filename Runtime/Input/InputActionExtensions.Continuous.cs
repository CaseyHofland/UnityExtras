#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityExtras.InputSystem
{
    public static partial class InputActionExtensions
    {
        private struct ContinuousInputAction
        {
            public bool isPerformed;
            public event Action<InputAction.CallbackContext>? performed;
            public InputAction.CallbackContext callbackContext;

            public void Invoke()
            {
                performed?.Invoke(callbackContext);
            }

            public bool Empty() => performed == null;
        }

        [AddComponentMenu("")]
        private class ContinuousInputActionUpdater : MonoBehaviour
        {
            private void Update()
            {
                foreach (var continuousAction in continuousInputActions.Values)
                {
                    if (continuousAction.isPerformed)
                    {
                        continuousAction.Invoke();
                    }
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad()
        {
            var go = new GameObject(nameof(ContinuousInputActionUpdater), typeof(ContinuousInputActionUpdater));
            go.hideFlags = HideFlags.HideAndDontSave;
        }

        private static Dictionary<InputAction, ContinuousInputAction> continuousInputActions = new();

        private static void Perform(InputAction.CallbackContext callbackContext)
        {
            if (!continuousInputActions.TryGetValue(callbackContext.action, out var continuousAction))
            {
                return;
            }

            continuousAction.isPerformed = !continuousAction.isPerformed;
            continuousAction.callbackContext = callbackContext;

            continuousInputActions[callbackContext.action] = continuousAction;
        }

        public static void AddContinuousActions(this InputAction inputAction, params Action<InputAction.CallbackContext>[] actions) => inputAction.AddContinuousActions(true, actions);
        public static void AddContinuousActions(this InputAction inputAction, bool addEmpty, params Action<InputAction.CallbackContext>[] actions)
        {
            if (!addEmpty && actions.Length == 0)
            {
                return;
            }

            if (!continuousInputActions.TryGetValue(inputAction, out var continuousAction))
            {
                // With this set-up, you can use:
                // - Tap for a single-fire action that cancels after tap-duration.
                // - Slow Tap for a continuous action that cancels on release.
                // - Press: Release Only for a continuous action that cancels on press.
                inputAction.started += Perform;
                if (inputAction.type == InputActionType.Button)
                {
                    inputAction.performed += Perform;
                }
                inputAction.canceled += Perform;
            }

            for (int i = 0; i < actions.Length; i++)
            {
                continuousAction.performed += actions[i];
            }

            continuousInputActions[inputAction] = continuousAction;
        }

        public static void RemoveContinuousActions(this InputAction inputAction, params Action<InputAction.CallbackContext>[] actions) => inputAction.RemoveContinuousActions(true, actions);
        public static void RemoveContinuousActions(this InputAction inputAction, bool removeEmpty, params Action<InputAction.CallbackContext>[] actions)
        {
            if (!continuousInputActions.TryGetValue(inputAction, out var continuousAction))
            {
                return;
            }

            for (int i = 0; i < actions.Length; i++)
            {
                continuousAction.performed -= actions[i];
            }

            if (removeEmpty && continuousAction.Empty())
            {
                inputAction.started -= Perform;
                inputAction.performed -= Perform;
                inputAction.canceled -= Perform;

                continuousInputActions.Remove(inputAction);
            }
            else
            {
                continuousInputActions[inputAction] = continuousAction;
            }
        }

        public static bool IsContinuousPerformed(this InputAction inputAction)
        {
            return continuousInputActions.TryGetValue(inputAction, out var continuousAction) && continuousAction.isPerformed;
        }
    }
}
