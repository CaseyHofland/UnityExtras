#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;
using UnityExtras.InputSystem;

namespace UnityExtras
{
    /// <summary>Handle third person character 2D movement through input.</summary>
    [AddComponentMenu("Physics 2D/Third Person Character 2D")]
    [RequireComponent(typeof(CharacterMover2D))]
    [DisallowMultipleComponent]
    public class ThirdPersonCharacter2D : MonoBehaviour
    {
        private CharacterMover2D? _characterMover2D;
        public CharacterMover2D characterMover2D => _characterMover2D ? _characterMover2D! : (_characterMover2D = GetComponent<CharacterMover2D>());

        public static implicit operator CharacterMover2D(ThirdPersonCharacter2D thirdPersonCharacter2D) => thirdPersonCharacter2D.characterMover2D;

        #region Input
        [field: Header("Input")]
        [field: SerializeField] public InputReaction moveReaction { get; set; }
        [field: SerializeField] public InputReaction sprintReaction { get; set; }
        [field: SerializeField] public InputReaction jumpReaction { get; set; }

        private const float jumpHoldTime = 0.8f;
        private float _currentJumpHoldTime = jumpHoldTime;

        private void OnEnable()
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

        private void OnDisable()
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

        private void MovePerformed(InputAction.CallbackContext context)
        {
            var speed = context.ReadRevalue<float>();
            var direction = new Vector2(speed, 0f);
            characterMover2D.Move(direction, sprintReaction.reaction?.isPerformed ?? false);
            characterMover2D.Turn(speed > 0f);
        }

        private void JumpPerformed(InputAction.CallbackContext context)
        {
            characterMover2D.Jump();
            if (!characterMover2D.allowPerpetualJump)
            {
                _currentJumpHoldTime -= Time.deltaTime;
                if (_currentJumpHoldTime <= 0f)
                {
                    jumpReaction.reaction!.isPerformed = false;
                    _currentJumpHoldTime = jumpHoldTime + characterMover2D.jumpBuffer;
                }
            }
        }

        private void JumpCanceled(InputAction.CallbackContext context)
        {
            if (!jumpReaction.reaction!.isPerformed)
            {
                _currentJumpHoldTime = jumpHoldTime + characterMover2D.jumpBuffer;
            }
        }
        #endregion
    }
}
