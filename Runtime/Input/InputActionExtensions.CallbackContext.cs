#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityExtras.InputSystem
{
    public static partial class InputActionExtensions
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearContextData()
        {
            Application.quitting -= Quitting;
            Application.quitting += Quitting;

            static void Quitting()
            {
                lastValues.Clear();
                lastControls.Clear();
            }
        }

        private static Dictionary<InputAction.CallbackContext, object> lastValues = new();
        private static Dictionary<InputAction.CallbackContext, InputControl> lastControls = new();

        public static TValue ReadValueOrLast<TValue>(this InputAction.CallbackContext context)
            where TValue : struct
        {
            if (context.phase.IsInProgress() || !lastValues.TryGetValue(context, out var value))
            {
                lastValues[context] = value = context.ReadValue<TValue>();
            }

            return (TValue)value;
        }

        public static InputControl? ControlOrLast(this InputAction.CallbackContext context)
        {
            try
            {
                return lastControls[context] = context.control;
            }
            catch (IndexOutOfRangeException)
            {
                return lastControls.TryGetValue(context, out var control) ? control : null;
            }
        }
    }
}
