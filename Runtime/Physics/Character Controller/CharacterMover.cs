#nullable enable
using UnityEngine;

using static UnityEngine.Mathf;
using static UnityExtras.ExtraMath;

namespace UnityExtras
{
    [AddComponentMenu("Physics/Character Mover")]
    [RequireComponent(typeof(CharacterController))]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(10)]
    public class CharacterMover : MonoBehaviour
    {
        private CharacterController? _characterController;
        public CharacterController characterController => _characterController ? _characterController! : (_characterController = GetComponent<CharacterController>());

        public static implicit operator CharacterController(CharacterMover characterControllerMove) => characterControllerMove.characterController;

        private const float speedOffset = 0.1f;

        [field: Header("Movement")]
        [field: SerializeField] [field: Tooltip("Move speed of the character in m/s")][field: Min(0f)] public float moveSpeed { get; set; } = 2.0f;
        [field: SerializeField] [field: Tooltip("Sprint speed of the character in m/s")][field: Min(0f)] public float sprintSpeed { get; set; } = 5.335f;
        [field: SerializeField] [field: Tooltip("Rotation speed of the character")][field: Min(0f)] public float rotationSpeed { get; set; } = 1.0f;
        [field: SerializeField] [field: Tooltip("Acceleration and deceleration")][field: Min(0.1f)] public float speedChangeRate { get; set; } = 10.0f;
        [field: Space(10)]
        [field: SerializeField] [field: Tooltip("The height the player can jump")][field: Min(0f)] public float jumpHeight { get; set; } = 1.2f;
        [field: SerializeField] [field: Tooltip("How much gravity affects this body")] public float gravityScale = 1f;

        private Vector3 _motion;
        private Vector3 _smoothGravity;
        private const float terminalVelocity = 53.0f;

        private Vector3 gravity => gravityScale * Physics.gravity;

        private void Start()
        {
            Debug.LogWarning("Character Mover needs to be optimized!");
        }

        private void OnEnable()
        {
            _smoothGravity = Vector3.zero;
            CalculateGravity();
        }

        private void Update()
        {
            characterController.Move((_motion + _smoothGravity) * Time.deltaTime);

            _motion = Vector3.zero;

            CalculateGravity();
        }

        private void CalculateGravity()
        {
            var gravityDelta = gravity * Time.deltaTime;
            if (characterController.isGrounded)
            {
                _smoothGravity = Vector3.down * 2f;
                return;
            }

            // Apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time).
            var gravityDeltaSpeed = gravityDelta.magnitude;
            var factor = Clamp(terminalVelocity - _smoothGravity.magnitude, -gravityDeltaSpeed, gravityDeltaSpeed);
            _smoothGravity += gravityDelta.normalized * factor;
        }

        public void Move(Vector3 direction) => Move(direction, 0f);
        public void Move(Vector3 direction, bool sprint) => Move(direction, sprint ? 1f : 0f);
        public void Move(Vector3 direction, float sprintFactor)
        {
            // Set target speed based on move speed, sprint speed and if sprint is pressed.
            var targetSpeed = moveSpeed + (sprintSpeed - moveSpeed) * sprintFactor;

            // Accelerate or decelerate to target speed.
            var moveVelocityScale = gravity.normalized;
            moveVelocityScale.x = 1f - Abs(moveVelocityScale.x);
            moveVelocityScale.y = 1f - Abs(moveVelocityScale.y);
            moveVelocityScale.z = 1f - Abs(moveVelocityScale.z);
            var currentSpeed = Vector3.Scale(characterController.velocity, moveVelocityScale).magnitude;
            if (currentSpeed < targetSpeed - speedOffset
                || currentSpeed > targetSpeed + speedOffset)
            {
                // Creates curved result rather than a linear one giving a more organic speed change.
                // Note T in Lerp is clamped, so we don't need to clamp our speed.
                targetSpeed = Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
            }

            direction = transform.rotation * direction;

            _motion += targetSpeed * direction;
        }

        public void Jump()
        {
            if (characterController.isGrounded)
            {
                _smoothGravity = JumpVelocity(jumpHeight, gravity);
            }
        }

        public void Turn(float turnFactor) => Turn(Vector3.up, turnFactor);
        public void Turn(Vector3 axis, float turnFactor)
        {
            // Rotate the player around the axis (left and right by default).
            transform.Rotate(axis, turnFactor * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
