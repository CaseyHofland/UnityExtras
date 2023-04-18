#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;
using UnityExtras.InputSystem;

namespace UnityExtras
{
    public abstract class CharacterInputBase<T> : MonoBehaviour
        where T : CharacterMoverBase
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [field: SerializeField, HideInInspector] public T characterMover { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public static implicit operator T(CharacterInputBase<T> characterInput) => characterInput.characterMover;

        protected virtual void InitializeComponents()
        {
            characterMover = GetComponent<T>();
        }

        protected virtual void Reset()
        {
            InitializeComponents();
        }

        protected virtual void Awake()
        {
            InitializeComponents();
        }

        #region Input
        [field: Header("Input")]
        [field: SerializeField] public InputReaction moveReaction { get; set; }
        [field: SerializeField] public InputReaction sprintReaction { get; set; }
        [field: SerializeField] public InputReaction jumpReaction { get; set; }

        private const float jumpHoldTime = 0.8f;
        private float _currentJumpHoldTime = jumpHoldTime;

        protected virtual void OnEnable()
        {
            if (moveReaction.reaction != null)
            {
                moveReaction.reaction.performed += MovePerformed;
            }
            if (jumpReaction.reaction != null && jumpReaction.input.action != null)
            {
                jumpReaction.reaction.performed += JumpPerformed;
                jumpReaction.input.action.canceled += JumpCanceled;
            }
        }

        protected virtual void OnDisable()
        {
            if (moveReaction.reaction != null)
            {
                moveReaction.reaction.performed -= MovePerformed;
            }
            if (jumpReaction.reaction != null && jumpReaction.input.action != null)
            {
                jumpReaction.reaction.performed -= JumpPerformed;
                jumpReaction.input.action.canceled -= JumpCanceled;
            }
        }

        protected abstract void MovePerformed(InputAction.CallbackContext context);

        protected virtual void JumpPerformed(InputAction.CallbackContext context)
        {
            characterMover.Jump();
            if (!characterMover.allowPerpetualJump)
            {
                _currentJumpHoldTime -= Time.deltaTime;
                if (_currentJumpHoldTime <= 0f)
                {
                    jumpReaction.reaction!.isPerformed = false;
                    _currentJumpHoldTime = jumpHoldTime + characterMover.jumpBuffer;
                }
            }
        }

        protected virtual void JumpCanceled(InputAction.CallbackContext context)
        {
            if (!jumpReaction.reaction!.isPerformed)
            {
                _currentJumpHoldTime = jumpHoldTime + characterMover.jumpBuffer;
            }
        }
        #endregion
    }
}
