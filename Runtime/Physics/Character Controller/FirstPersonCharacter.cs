#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;
using UnityExtras.InputSystem;

using Input = UnityExtras.InputSystem.Input;

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
        [field: SerializeField] public Input moveInput { get; set; }
        [field: SerializeField] public Input lookInput { get; set; }
        [field: SerializeField] public Input sprintInput { get; set; }
        [field: SerializeField] public Input jumpInput { get; set; }

        private void OnEnable()
        {
            moveInput.action?.AddContinuousActions(MovePerformed);
            lookInput.action?.AddContinuousActions(LookPerformed);
            jumpInput.action?.AddContinuousActions(JumpPerformed);
            sprintInput.action?.AddContinuousActions();
        }

        private void OnDisable()
        {
            moveInput.action?.RemoveContinuousActions(MovePerformed);
            lookInput.action?.RemoveContinuousActions(LookPerformed);
            jumpInput.action?.RemoveContinuousActions(JumpPerformed);
            sprintInput.action?.RemoveContinuousActions();
        }

        private void MovePerformed(InputAction.CallbackContext context)
        {
            var direction2D = context.ReadValue<Vector2>();
            var direction = new Vector3(direction2D.x, 0f, direction2D.y);
            characterMover.MoveRelative(direction, sprintInput.action?.IsContinuousPerformed() ?? false);
        }

        private void LookPerformed(InputAction.CallbackContext context)
        {
            Look(context.ReadValue<Vector2>());
        }

        private void JumpPerformed(InputAction.CallbackContext context)
        {
            characterMover.Jump();
        }
        #endregion
    }
}
