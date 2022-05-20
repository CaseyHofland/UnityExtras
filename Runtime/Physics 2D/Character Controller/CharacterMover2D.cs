#nullable enable
using UnityEngine;

using static UnityEngine.Mathf;
using static UnityExtras.ExtraMath;

namespace UnityExtras
{
    [AddComponentMenu("Physics 2D/Character Mover 2D")]
    [RequireComponent(typeof(CharacterController2D))]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(10)]
    public class CharacterMover2D : MonoBehaviour
    {
        private CharacterController2D? _characterController2D;
        public CharacterController2D characterController2D => _characterController2D ? _characterController2D! : (_characterController2D = GetComponent<CharacterController2D>());

        public static implicit operator CharacterController2D(CharacterMover2D characterMover2D) => characterMover2D.characterController2D;

        private const float speedOffset = 0.1f;
        private const float fastFallBuffer = 0.05f;

        [field: Header("Move")]
        [field: SerializeField][field: Tooltip("Move speed of the character in m/s")][field: Min(0f)] public float moveSpeed { get; set; } = 2.0f;
        [field: SerializeField][field: Tooltip("Sprint boost of the character in m/s")][field: Min(0f)] public float sprintBoost { get; set; } = 3.335f;
        [field: SerializeField][field: Tooltip("Acceleration and deceleration")][field: Min(0.1f)] public float speedChangeRate { get; set; } = 10.0f;

        [field: Header("Jump")]
        [field: SerializeField][field: Tooltip("The height the character can jump in m")][field: Min(0f)] public float jumpHeight { get; set; } = 1.2f;
        [field: SerializeField][field: Tooltip("Time to reach the peak of the jump: if 0, this value is ignored and gravity scale is used instead")][field: Min(0f)] public float peakTime { get; set; } = 0f;
        [field: SerializeField][field: Tooltip("How much this body is affected by gravity")] public float gravityScale { get; set; } = 1f;
        [field: SerializeField][field: Tooltip("How much extra gravity is applied to end the character's jump early")][field: Min(1f)] public float fastFallRatio { get; set; } = 3f;
        [field: SerializeField][field: Tooltip("Delay to allow jumping after becoming ungrounded")][field: Min(0f)] public float coyoteTime { get; set; } = 0.15f;
        [field: SerializeField][field: Tooltip("Buffer to allow jumping before becoming grounded")][field: Min(0f)] public float jumpBuffer { get; set; } = 0.2f;

        public Vector2 motion { get; set; }
        public Vector2 targetMotion { get; set; }

        private Vector2 _smoothGravity;
        private float _currentFastFallBuffer;
        private float _currentCoyoteTime;
        private float _currentJumpBuffer;
        public const float terminalVelocity = 53.0f;

        private float _jumpGravityScale => peakTime > 0f
            ? (2f * jumpHeight) / (peakTime * peakTime) / _gravityForce
            : gravityScale;
        private Vector2 _jumpGravity => _jumpGravityScale * Physics2D.gravity;

        private bool _fastFalling => _currentFastFallBuffer < 0f;

        private float _currentGravityScale => characterController2D.isGrounded || _smoothGravity.normalized == _gravityDirection
            ? gravityScale
            : _jumpGravityScale * (_fastFalling ? fastFallRatio : 1f);
        private Vector2 _gravity => _currentGravityScale * Physics2D.gravity;

        #region Dirty
        private Vector2 _lastGravity;
        private bool _gravityDirty
        {
            get => !_lastGravity.Equals(Physics2D.gravity);
            set
            {
                if (value)
                {
                    return;
                }

                _lastGravity = Physics2D.gravity;
            }
        }

        private float _gravityForce;
        private float _currentGravityForce => _currentGravityScale * _gravityForce;
        private Vector2 _gravityDirection;
        private Vector2 _moveVelocityScale;

        private void PrepareGravity()
        {
            _gravityForce = Physics2D.gravity.magnitude;
            _gravityDirection = Physics2D.gravity.normalized;

            _moveVelocityScale = new Vector2
            (
                1f - Abs(_gravityDirection.x),
                1f - Abs(_gravityDirection.y)
            );

            _gravityDirty = false;
        }

        private bool TryPrepareGravity()
        {
            if (_gravityDirty)
            {
                PrepareGravity();
                return true;
            }

            return false;
        }
        #endregion

        private void OnValidate()
        {
            if (characterController2D.minMoveDistance != 0f)
            {
                Debug.LogWarning($"It is recommended to set the {nameof(CharacterController2D)}'s {nameof(characterController2D.minMoveDistance)} to 0 to ensure all movement is correctly registered.", characterController2D);
            }
        }

        #region Update
        private void OnEnable()
        {
            TryPrepareGravity();

            motion = Vector3.zero;
            targetMotion = Vector3.zero;
            _smoothGravity = _gravity * Time.deltaTime;
            characterController2D.Move(_smoothGravity * Time.deltaTime);

            _currentFastFallBuffer = 0f;
            _currentCoyoteTime = 0f;
            _currentJumpBuffer = 0f;
        }

        // WARNING: Order dependent!
        private void Update()
        {
            TryPrepareGravity();

            CalculateMotion();
            characterController2D.Move((motion + _smoothGravity) * Time.deltaTime);
            targetMotion = Vector3.zero;

            CalculateGravity();
            UpdateJumpSettings();
        }

        private void CalculateMotion()
        {
            var targetSpeed = targetMotion.magnitude;

            // Accelerate or decelerate to target speed.
            var currentSpeed = Vector2.Scale(characterController2D.velocity, _moveVelocityScale).magnitude;
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
            if (characterController2D.isGrounded)
            {
                _smoothGravity = _gravity * Time.deltaTime;
                return;
            }

            // Apply gravity over time if under terminal velocity (multiply by delta time twice to linearly speed up over time).
            var gravityDeltaSpeed = _currentGravityForce * Time.deltaTime;
            var factor = Clamp(terminalVelocity - _smoothGravity.magnitude, -gravityDeltaSpeed, gravityDeltaSpeed);
            _smoothGravity += _gravityDirection * factor;
        }

        private void UpdateJumpSettings()
        {
            // Update coyote time, jump buffer and fast fall buffer.
            if (characterController2D.isGrounded)
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
        public void Move(Vector2 movement) => Move(movement, default);
        public void Move(Vector2 movement, bool sprint)
        {
            // Set target speed based on move speed and
            var targetSpeed = moveSpeed + (sprint ? sprintBoost : 0f);
            targetMotion += targetSpeed * movement;
        }

        public void MoveRelative(Vector2 movement) => MoveRelative(movement, default);
        public void MoveRelative(Vector2 movement, bool sprint) => Move(Quaternion.Euler(0f, 0f, transform.eulerAngles.z) * movement, sprint);

        public void Jump()
        {
            TryPrepareGravity();

            // Jump or activate the jump buffer.
            if (characterController2D.isGrounded || _currentCoyoteTime > 0f)
            {
                _smoothGravity = JumpVelocity(jumpHeight, _gravityDirection, _jumpGravityScale * _gravityForce);
                _currentFastFallBuffer = 0f;
                _currentCoyoteTime = 0f;
                _currentJumpBuffer = 0f;
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

        public void Turn(bool faceRight)
        {
            var angle = -transform.eulerAngles.y + (faceRight ? 0f : 180f);
            transform.rotation *= Quaternion.AngleAxis(angle, Vector3.up);
        }
        #endregion
    }
}
