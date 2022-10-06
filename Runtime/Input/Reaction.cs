#nullable enable
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityExtras.InputSystem
{
    public class Reaction
    {
        public bool isPerformed { get; set; }
        public event Action<InputAction.CallbackContext>? performed;
        public InputAction.CallbackContext callbackContext { get; private set; }
        public ReactionMask reactionMask { get; set; }

        public static implicit operator bool(Reaction reaction) => reaction.isPerformed;

        internal void Performed(InputAction.CallbackContext callbackContext)
        {
            this.callbackContext = callbackContext;

            switch (GetReactionMethod(callbackContext))
            {
                case ReactionMethod.Default:
                    performed?.Invoke(callbackContext);
                    break;
                case ReactionMethod.Toggle:
                    isPerformed = !isPerformed;
                    break;
                case ReactionMethod.Hold:
                    isPerformed = true;
                    break;
            }
        }

        internal void Canceled(InputAction.CallbackContext callbackContext)
        {
            if (GetReactionMethod(callbackContext) != ReactionMethod.Toggle)
            {
                isPerformed = false;
            }
        }

        private ReactionMethod GetReactionMethod(InputAction.CallbackContext callbackContext) => callbackContext.interaction == null
            ? reactionMask.defaultReaction
            : reactionMask.GetReaction(callbackContext.interaction.GetType());

        [AddComponentMenu("")]
        private class ReactionUpdater : MonoBehaviour
        {
            private void Update()
            {
                foreach (var reaction in InputActionExtensions._reactions.Values)
                {
                    if (reaction.isPerformed)
                    {
                        reaction.performed?.Invoke(reaction.callbackContext);
                    }
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad()
        {
            var go = new GameObject(nameof(ReactionUpdater), typeof(ReactionUpdater))
            {
                hideFlags = HideFlags.HideInHierarchy
            };
        }
    }
}
