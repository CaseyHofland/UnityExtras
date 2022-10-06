#nullable enable
using UnityEngine;

namespace UnityExtras
{
    /// <include file='./CharacterMoverBase.xml' path='docs/CharacterMoverBase/*'/>
    [DefaultExecutionOrder(10)]
    public abstract class CharacterMoverBase : MonoBehaviour
    {
        protected abstract Vector3 physicsGravity { get; }
        protected abstract bool characterIsGrounded { get; }
        protected abstract Vector3 characterVelocity { get; }
        protected abstract CollisionFlags CharacterMove(Vector3 motion);

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
        [field: SerializeField][field: Tooltip("Allow the character to jump perpetually")] public bool allowPerpetualJump { get; set; }

        public const float terminalVelocity = 53.0f;

        public Vector3 motion { get; set; }
        public Vector3 targetMotion { get; set; }

        private const float speedOffset = 0.1f;
        private const float noiseBuffer = 0.05f;

        private Vector3 _smoothGravity;
        private float _currentFastFallBuffer;
        private float _currentCoyoteTime;
        private float _currentJumpBuffer;
        private float _jumpBufferJumpHeight;
        private float _currentCanJumpBuffer;

        private float _jumpGravityScale => peakTime > 0f
            ? (2f * jumpHeight) / (peakTime * peakTime) / _gravityForce
            : gravityScale;

        private bool _fastFalling => _currentFastFallBuffer < 0f;

        private float _currentGravityScale => characterIsGrounded || _smoothGravity.normalized == _gravityDirection
            ? gravityScale
            : _jumpGravityScale * (_fastFalling ? fastFallRatio : 1f);
        private float _currentGravityForce => _currentGravityScale * _gravityForce;
        private Vector3 _gravity => _currentGravityScale * physicsGravity;

        #region Dirty
        private Vector3 _lastGravity;
        private bool _gravityChanged;
        private bool _gravityDirty
        {
            get => _gravityChanged
                || !_lastGravity.Equals(physicsGravity);
            set
            {
                if (_gravityChanged = value)
                {
                    return;
                }

                _lastGravity = physicsGravity;
            }
        }

        private float _gravityForce;
        private Vector3 _gravityDirection;
        private Vector3 _moveVelocityScale;

        private void PrepareGravity()
        {
            _gravityForce = physicsGravity.magnitude;
            _gravityDirection = physicsGravity.normalized;

            var dynamicGravityDirection = _gravityDirection;
            _moveVelocityScale = new Vector3
            (
                1f - Mathf.Abs(dynamicGravityDirection.x),
                1f - Mathf.Abs(dynamicGravityDirection.y),
                1f - Mathf.Abs(dynamicGravityDirection.z)
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

        #region Update
        private void OnEnable()
        {
            TryPrepareGravity();

            motion = Vector3.zero;
            targetMotion = Vector3.zero;
            _smoothGravity = _gravity * Time.deltaTime;
            CharacterMove(_smoothGravity * Time.deltaTime);

            _currentFastFallBuffer = 0f;
            _currentCoyoteTime = 0f;
            _currentJumpBuffer = 0f;
            _currentCanJumpBuffer = 0f;
        }

        // WARNING: Order dependent!
        private void Update()
        {
            TryPrepareGravity();

            CalculateMotion();
            CharacterMove((motion + _smoothGravity) * Time.deltaTime);
            targetMotion = Vector3.zero;

            CalculateGravity();
            UpdateJumpSettings();
        }

        private void CalculateMotion()
        {
            var targetSpeed = targetMotion.magnitude;

            // Accelerate or decelerate to target speed.
            var currentSpeed = Vector3.Scale(characterVelocity, _moveVelocityScale).magnitude;
            if (currentSpeed < targetSpeed - speedOffset
                || currentSpeed > targetSpeed + speedOffset)
            {
                // Creates curved result rather than a linear one giving a more organic speed change.
                // Note T in Lerp is clamped, so we don't need to clamp our speed.
                targetSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
            }

            motion = targetMotion.normalized * targetSpeed;
        }

        private void CalculateGravity()
        {
            if (characterIsGrounded)
            {
                _smoothGravity = _gravity * Time.deltaTime;
                return;
            }

            // Apply gravity over time if under terminal velocity (multiply by delta time twice to linearly speed up over time).
            var gravityDeltaSpeed = _currentGravityForce * Time.deltaTime;
            var factor = Mathf.Clamp(terminalVelocity - _smoothGravity.magnitude, -gravityDeltaSpeed, gravityDeltaSpeed);
            _smoothGravity += _gravityDirection * factor;
        }

        private void UpdateJumpSettings()
        {
            // Update coyote time, jump buffer and fast fall buffer.
            if (characterIsGrounded)
            {
                _currentCoyoteTime = coyoteTime;
                if (_currentJumpBuffer > 0f)
                {
                    ForcedJump(_jumpBufferJumpHeight);
                }
            }
            else
            {
                _currentCoyoteTime -= Time.deltaTime;
            }

            _currentJumpBuffer -= Time.deltaTime;
            _currentFastFallBuffer -= Time.deltaTime;
            _currentCanJumpBuffer += Time.deltaTime;
        }
        #endregion

        #region Movement
        /// <include file='./CharacterMoverBase.xml' path='docs/Move/*'/>
        protected void Move(Vector3 movement, bool sprint)
        {
            var targetSpeed = moveSpeed + (sprint ? sprintBoost : 0f);
            targetMotion += targetSpeed * movement;
        }

        /// <include file='./CharacterMoverBase.xml' path='docs/Jump/Base/*'/>
        public virtual void Jump() => Jump(jumpHeight);
        /// <include file='./CharacterMoverBase.xml' path='docs/Jump/Base/*'/>
        public virtual void Jump(float jumpHeight)
        {
            TryPrepareGravity();

            if (allowPerpetualJump || _currentCanJumpBuffer > 0f)
            {
                // Jump or activate the jump buffer.
                if (characterIsGrounded || _currentCoyoteTime > 0f)
                {
                    _smoothGravity = ExtraMath.JumpVelocity(jumpHeight, _gravityDirection, _jumpGravityScale * _gravityForce);
                    _currentFastFallBuffer = 0f;
                    _currentCoyoteTime = 0f;
                    _currentJumpBuffer = 0f;
                }
                else
                {
                    _currentJumpBuffer = jumpBuffer;
                    _jumpBufferJumpHeight = jumpHeight;
                }
            }

            // Reset the fast fall buffer and can jump buffer.
            if (!_fastFalling)
            {
                _currentFastFallBuffer = noiseBuffer + Time.deltaTime;
            }
            _currentCanJumpBuffer = -noiseBuffer - Time.deltaTime;
        }

        /// <include file='./CharacterMoverBase.xml' path='docs/Jump/*'/>
        public virtual void ForcedJump() => ForcedJump(jumpHeight);
        /// <include file='./CharacterMoverBase.xml' path='docs/Jump/*'/>
        public virtual void ForcedJump(float jumpHeight)
        {
            _currentCanJumpBuffer = 1f;
            _currentCoyoteTime = 1f;
            Jump(jumpHeight);

        }
        #endregion
    }
}
