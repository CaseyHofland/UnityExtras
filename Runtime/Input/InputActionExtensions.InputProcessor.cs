#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityExtras.InputSystem
{
    public static partial class InputActionExtensions
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearReprocessors()
        {
            Application.quitting -= Quitting;
            Application.quitting += Quitting;

            static void Quitting()
            {
                actionReprocessors.Clear();
                bindingReprocessors.Clear();
            }
        }

        private static Dictionary<InputAction, List<InputProcessor>> actionReprocessors = new();
        private static Dictionary<InputBinding, List<InputProcessor>> bindingReprocessors = new();

        public static List<InputProcessor> Reprocessors(this InputAction inputAction)
        {
            actionReprocessors.TryAdd(inputAction, new());
            return actionReprocessors[inputAction];
        }

        public static List<InputProcessor> Reprocessors(this InputBinding inputBinding)
        {
            bindingReprocessors.TryAdd(inputBinding, new());
            return bindingReprocessors[inputBinding];
        }

        public static TValue ReadRevalue<TValue>(this InputAction.CallbackContext context)
            where TValue : struct
        {
            var value = context.ReadValueOrLast<TValue>();

            // Process the actions reprocessors.
            var control = context.ControlOrLast();
            if (control == null)
            {
                return value;
            }

            if (actionReprocessors.TryGetValue(context.action, out var inputProcessors))
            {
                for (int i = 0; i < inputProcessors.Count; i++)
                {
                    var inputProcessor = (InputProcessor<TValue>)inputProcessors[i];
                    value = inputProcessor.Process(value, control);
                }
            }

            // Process the bindings reprocessors.
            var binding = context.action.GetBindingForControl(control);
            if (binding == null)
            {
                return value;
            }
            
            if (bindingReprocessors.TryGetValue(binding.Value, out inputProcessors))
            {
                for (int i = 0; i < inputProcessors.Count; i++)
                {
                    var inputProcessor = (InputProcessor<TValue>)inputProcessors[i];
                    value = inputProcessor.Process(value, control);
                }
            }

            return value;
        }
    }
}
