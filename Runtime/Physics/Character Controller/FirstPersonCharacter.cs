#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;
using UnityExtras.InputSystem;

namespace UnityExtras
{
    [AddComponentMenu("Physics/First Person Character")]
    [RequireComponent(typeof(CharacterMover))]
    [DisallowMultipleComponent]
    public class FirstPersonCharacter : MonoBehaviour
    {
        private CharacterMover? _characterMover;
        public CharacterMover characterMover => _characterMover ? _characterMover! : (_characterMover = GetComponent<CharacterMover>());

        public static implicit operator CharacterMover(FirstPersonCharacter firstPersonCharacter) => firstPersonCharacter.characterMover;

        [field: Header("Look")]
        [field: SerializeField] public Transform? lookTransform { get; set; }
        [field: SerializeField][field: Tooltip("How far in degrees can you move the camera up")][field: Min(0f)] public float topClamp { get; set; } = 89.9f;
        [field: SerializeField][field: Tooltip("How far in degrees can you move the camera down")][field: Min(0f)] public float bottomClamp { get; set; } = 89.9f;

        private float _lookPitch;

        public void Look(Vector2 movement)
        {
            var turnFactor = movement.x;
            characterMover.Turn(turnFactor);

            var lookFactor = movement.y;
            if (lookTransform != null)
            {
                _lookPitch += lookFactor * characterMover.rotationSpeed * Time.deltaTime;
                _lookPitch = Mathf.Clamp(_lookPitch, -bottomClamp, topClamp);
                lookTransform.rotation = transform.rotation * Quaternion.Euler(_lookPitch, 0f, 0f);
            }
        }

        #region Input
        [field: Header("Input")]
        [field: SerializeField] public InputReaction moveReaction { get; set; }
        [field: SerializeField] public InputReaction lookReaction { get; set; }
        [field: SerializeField] public InputReaction sprintReaction { get; set; }
        [field: SerializeField] public InputReaction jumpReaction { get; set; }

        [field: SerializeField] [field: Min(0f)] public float jumpHoldTime { get; set; }
        private float _jumpHoldTime;

        private void OnEnable()
        {
            if (moveReaction.reaction != null)
            {
                moveReaction.reaction.performed += MovePerformed;
            }
            if (lookReaction.reaction != null)
            {
                lookReaction.reaction.performed += LookPerformed;
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
            if (lookReaction.reaction != null)
            {
                lookReaction.reaction.performed -= LookPerformed;
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
            var direction2D = context.ReadRevalue<Vector2>();
            var direction = new Vector3(direction2D.x, 0f, direction2D.y);
            characterMover.MoveRelative(direction, sprintReaction.reaction ?? false);
        }

        private void LookPerformed(InputAction.CallbackContext context)
        {
            Look(context.ReadRevalue<Vector2>());
        }

        private void JumpPerformed(InputAction.CallbackContext context)
        {
            if (_jumpHoldTime <= 0f)
            {
                _jumpHoldTime = jumpHoldTime;
            }
            characterMover.Jump();
        }
        #endregion
    }
}
