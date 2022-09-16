#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;
using UnityExtras.InputSystem;

namespace UnityExtras
{
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

        [field: SerializeField][field: Min(0f)] public float jumpHoldTime { get; set; }
        private float _jumpHoldTime;

        private void OnEnable()
        {
            if (moveReaction.reaction != null)
            {
                moveReaction.reaction.performed += MovePerformed;
            }
            if (jumpReaction.reaction != null)
            {
                jumpReaction.reaction.performed += JumpPerformed;
            }
        }

        private void OnDisable()
        {
            if (moveReaction.reaction != null)
            {
                moveReaction.reaction.performed -= MovePerformed;
            }
            if (jumpReaction.reaction != null)
            {
                jumpReaction.reaction.performed -= JumpPerformed;
            }
        }

        private void Update()
        {
            _jumpHoldTime -= Time.deltaTime;
            if (_jumpHoldTime <= 0f && jumpReaction.reaction != null)
            {
                jumpReaction.reaction.isPerformed = false;
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
            if (_jumpHoldTime <= 0f)
            {
                _jumpHoldTime = jumpHoldTime;
            }
            characterMover2D.Jump();
        }
        #endregion
    }
}
