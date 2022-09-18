#nullable enable
using UnityEngine;

namespace UnityExtras
{
    /// <include file='./CharacterMover2D.xml' path='docs/CharacterMover2D/*'/>
    [AddComponentMenu("Physics 2D/Character Mover 2D")]
    [RequireComponent(typeof(CharacterController2D))]
    [DisallowMultipleComponent]
    public class CharacterMover2D : CharacterMoverBase
    {
        private CharacterController2D? _characterController2D;
        public CharacterController2D characterController2D => _characterController2D ? _characterController2D! : (_characterController2D = GetComponent<CharacterController2D>());

        public static implicit operator CharacterController2D(CharacterMover2D characterMover2D) => characterMover2D.characterController2D;

        protected override Vector3 physicsGravity => Physics2D.gravity;
        protected override bool characterIsGrounded => characterController2D.isGrounded;
        protected override Vector3 characterVelocity => characterController2D.velocity;
        protected override CollisionFlags CharacterMove(Vector3 motion) => characterController2D.Move(motion);

        private void OnValidate()
        {
            if (characterController2D.minMoveDistance != 0f)
            {
                Debug.LogWarning($"It is recommended to set the {nameof(CharacterController2D)}'s {nameof(characterController2D.minMoveDistance)} to 0 to ensure all movement is correctly registered.", characterController2D);
            }
        }

        /// <inheritdoc cref="CharacterMoverBase.Move"/>
        public void Move(Vector2 movement) => Move(movement, default);
        /// <inheritdoc cref="CharacterMoverBase.Move"/>
        public void Move(Vector2 movement, bool sprint) => base.Move(movement, sprint);
        /// <inheritdoc cref="CharacterMoverBase.Move"/>
        public void MoveRelative(Vector2 movement) => MoveRelative(movement, default);
        /// <inheritdoc cref="CharacterMoverBase.Move"/>
        public void MoveRelative(Vector2 movement, bool sprint) => Move(Quaternion.Euler(0f, 0f, transform.eulerAngles.z) * movement, sprint);

        /// <include file='./CharacterMover2D.xml' path='docs/Turn/*'/>
        public void Turn(bool faceRight)
        {
            var angle = -transform.eulerAngles.y + (faceRight ? 0f : 180f);
            transform.rotation *= Quaternion.AngleAxis(angle, Vector3.up);
        }
    }
}
