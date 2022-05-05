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
        private const float fastFallBuffer = 0.01f;

        [field: Header("Move")]
        [field: SerializeField] [field: Tooltip("Move speed of the character in m/s")][field: Min(0f)] public float moveSpeed { get; set; } = 2.0f;
        [field: SerializeField] [field: Tooltip("Sprint boost of the character in m/s")][field: Min(0f)] public float sprintBoost { get; set; } = 3.335f;
        [field: SerializeField] [field: Tooltip("Rotation speed of the character")][field: Min(0f)] public float rotationSpeed { get; set; } = 1.0f;
        [field: SerializeField] [field: Tooltip("Acceleration and deceleration")][field: Min(0.1f)] public float speedChangeRate { get; set; } = 10.0f;

        [field: Header("Jump")]
        [field: SerializeField] [field: Tooltip("The height the character can jump in m")][field: Min(0f)] public float jumpHeight { get; set; } = 1.2f;
        [field: SerializeField] [field: Tooltip("Time to reach the peak of the jump: if 0, this value is ignored and gravity scale is used instead")] [field: Min(0f)] public float peakTime { get; set; } = 0f;
        [field: SerializeField] [field: Tooltip("How much this body is affected by gravity")] public float gravityScale { get; set; } = 1f;
        [field: SerializeField] [field: Tooltip("How much extra gravity is applied to end the character's jump early")] [field: Min(1f)] public float fastFallRatio { get; set; } = 3f;
        [field: SerializeField] [field: Tooltip("Delay to allow jumping after becoming ungrounded")][field: Min(0f)] public float coyoteTime { get; set; } = 0.15f;
        [field: SerializeField] [field: Tooltip("Buffer to allow jumping before becoming grounded")][field: Min(0f)] public float jumpBuffer { get; set; } = 0.2f;

        public Vector3 motion { get; set; }
        public Vector3 targetMotion { get; set; }

        private Vector3 _smoothGravity;
        private float _currentFastFallBuffer;
        private float _currentCoyoteTime;
        private float _currentJumpBuffer;
        public const float terminalVelocity = 53.0f;

        private float _jumpGravityScale => peakTime > 0f
            ? ((2f * jumpHeight) / (peakTime * peakTime)) / Physics.gravity.magnitude
            : gravityScale;
        private Vector3 _jumpGravity => _jumpGravityScale * Physics.gravity;

        private bool _fastFalling => _currentFastFallBuffer < 0f;

        private float _currentGravityScale => characterController.isGrounded || _smoothGravity.normalized == Physics.gravity.normalized
            ? gravityScale
            : _jumpGravityScale * (_fastFalling ? fastFallRatio : 1f);
        private Vector3 _gravity => _currentGravityScale * Physics.gravity;

        private void Start()
        {
            Debug.LogWarning("Character Mover needs to be optimized!");
            Debug.LogWarning("Character Mover rotation speed should be much higher! Work in tandem with InputSystem to get the right values.");
        }

        private void OnValidate()
        {
            if (characterController.minMoveDistance != 0f)
            {
                Debug.LogWarning($"It is recommended to set the {nameof(CharacterController)}'s {nameof(characterController.minMoveDistance)} to 0 to ensure all movement is correctly registered.", characterController);
            }
        }

        #region Update
        private void OnEnable()
        {
            motion = Vector3.zero;
            targetMotion = Vector3.zero;
            _smoothGravity = _gravity * Time.deltaTime;
            characterController.Move(_smoothGravity * Time.deltaTime);

            _currentFastFallBuffer = 0f;
            _currentCoyoteTime = 0f;
            _currentJumpBuffer = 0f;
        }

        // WARNING: Order dependent!
        private void Update()
        {
            CalculateMotion();
            characterController.Move((motion + _smoothGravity) * Time.deltaTime);
            targetMotion = Vector3.zero;

            CalculateGravity();
            UpdateJumpSettings();
        }

        private void CalculateMotion()
        {
            var targetSpeed = targetMotion.magnitude;

            // Accelerate or decelerate to target speed.
            var moveVelocityScale = Physics.gravity.normalized;
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

            motion = targetMotion.normalized * targetSpeed;
        }

        private void CalculateGravity()
        {
            var gravityDelta = _gravity * Time.deltaTime;
            if (characterController.isGrounded)
            {
                _smoothGravity = gravityDelta;
                return;
            }

            // Apply gravity over time if under terminal velocity (multiply by delta time twice to linearly speed up over time).
            var gravityDeltaSpeed = gravityDelta.magnitude;
            var factor = Clamp(terminalVelocity - _smoothGravity.magnitude, -gravityDeltaSpeed, gravityDeltaSpeed);
            _smoothGravity += gravityDelta.normalized * factor;
        }

        private void UpdateJumpSettings()
        {
            // Update coyote time, jump buffer and fast fall buffer.
            if (characterController.isGrounded)
            {
                _currentCoyoteTime = coyoteTime;
                if (_currentJumpBuffer > 0f)
                {
                    Jump();
                }
            }
            else
            {
                _currentCoyoteTime -= Time.deltaTime;
            }

            _currentJumpBuffer -= Time.deltaTime;
            _currentFastFallBuffer -= Time.deltaTime;
        }
        #endregion

        #region Movement
        public void Move(Vector3 movement) => Move(movement, default);
        public void Move(Vector3 movement, bool sprint)
        {
            // Set target speed based on move speed and
            var targetSpeed = moveSpeed + (sprint ? sprintBoost : 0f);
            targetMotion = targetSpeed * (transform.rotation * movement);
        }

        public void Jump()
        {
            // Jump or activate the jump buffer.
            if (characterController.isGrounded || _currentCoyoteTime > 0f)
            {
                if (_fastFalling)
                {
                    _smoothGravity = JumpVelocity(jumpHeight, _jumpGravity);
                    _currentFastFallBuffer = 0f;
                    _currentCoyoteTime = 0f;
                    _currentJumpBuffer = 0f;
                }
            }
            else
            {
                _currentJumpBuffer = jumpBuffer;
            }

            // Reset the fast fall buffer.
            if (!_fastFalling)
            {
                _currentFastFallBuffer = fastFallBuffer + Time.deltaTime;
            }
        }

        public void Turn(float turnFactor) => Turn(Vector3.up, turnFactor);
        public void Turn(Vector3 axis, float turnFactor)
        {
            // Rotate the player around the axis (left and right by default).
            transform.Rotate(axis, turnFactor * rotationSpeed * Time.deltaTime, Space.Self);
        }
        #endregion
    }
}
