#nullable enable
using UnityEngine;

namespace UnityExtras
{
    /// <include file='./CharacterMover.xml' path='docs/CharacterMover/*'/>
    [AddComponentMenu("Physics/Character Mover")]
    [RequireComponent(typeof(CharacterController))]
    [DisallowMultipleComponent]
    public class CharacterMover : CharacterMoverBase
    {
        private CharacterController? _characterController;
        public CharacterController characterController => _characterController ? _characterController! : (_characterController = GetComponent<CharacterController>());

        public static implicit operator CharacterController(CharacterMover characterMover) => characterMover.characterController;

        protected override Vector3 physicsGravity => Physics.gravity;
        protected override bool characterIsGrounded => characterController.isGrounded;
        protected override Vector3 characterVelocity => characterController.velocity;
        protected override CollisionFlags CharacterMove(Vector3 motion) => characterController.Move(motion);

        [field: Header("Turn")]
        [field: SerializeField][field: Tooltip("Rotation speed of the character in d/s")][field: Min(0f)] public float rotationSpeed { get; set; } = 360f;

        /// <inheritdoc cref="CharacterMoverBase.Move"/>
        public void Move(Vector3 movement) => Move(movement, default);
        /// <inheritdoc cref="CharacterMoverBase.Move"/>
        public new void Move(Vector3 movement, bool sprint) => base.Move(movement, sprint);
        /// <inheritdoc cref="CharacterMoverBase.Move"/>
        public void MoveRelative(Vector3 movement) => MoveRelative(movement, default);
        /// <inheritdoc cref="CharacterMoverBase.Move"/>
        public void MoveRelative(Vector3 movement, bool sprint) => Move(transform.rotation * movement, sprint);

        /// <include file='./CharacterMover.xml' path='docs/Turn/*'/>
        public void Turn(float turnFactor = 1f) => Turn(Vector3.up, turnFactor);
        /// <include file='./CharacterMover.xml' path='docs/Turn/*'/>
        public void Turn(Vector3 axis, float turnFactor = 1f) => transform.Rotate(axis, turnFactor * rotationSpeed * Time.deltaTime, Space.Self);

        public void TurnTowards(Vector3 direction, float turnFactor = 1f) => TurnTowards(direction, Vector3.up, turnFactor);
        public void TurnTowards(Vector3 direction, Vector3 axis, float turnFactor = 1f)
        {
            var signedAngle = Vector3.SignedAngle(transform.forward, direction, transform.up);
            var sign = Mathf.Sign(signedAngle);
            var angle = Mathf.Abs(signedAngle);
            angle = Mathf.Min(angle, rotationSpeed * Time.deltaTime);
            transform.rotation *= Quaternion.AngleAxis(angle * sign * turnFactor, axis);
        }
    }
}
