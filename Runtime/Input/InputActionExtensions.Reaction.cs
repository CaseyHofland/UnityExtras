#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityExtras.InputSystem
{
    public static partial class InputActionExtensions
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearReactions()
        {
            Application.quitting -= Quitting;
            Application.quitting += Quitting;

            static void Quitting()
            {
                var keys = new InputAction[_reactions.Count];
                _reactions.Keys.CopyTo(keys, 0);
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i].ClearReaction();
                }
            }
        }

        internal static Dictionary<InputAction, Reaction> _reactions = new();

        public static Reaction Reaction(this InputAction inputAction)
        {
            if (!_reactions.TryGetValue(inputAction, out var reaction))
            {
                _reactions[inputAction] = reaction = new();

                inputAction.performed += reaction.Performed;
                inputAction.canceled += reaction.Canceled;
            }

            return reaction;
        }

        public static Reaction? ClearReaction(this InputAction inputAction)
        {
            if (_reactions.Remove(inputAction, out var reaction))
            {
                inputAction.performed -= reaction.Performed;
                inputAction.canceled -= reaction.Canceled;
            }

            return reaction;
        }
    }
}
