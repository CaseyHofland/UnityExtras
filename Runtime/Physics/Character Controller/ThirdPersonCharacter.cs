#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;
using UnityExtras.InputSystem;

namespace UnityExtras
{
    /// <summary>Handle third person character movement through input.</summary>
    [AddComponentMenu("Physics/Third Person Character")]
    [RequireComponent(typeof(CharacterMover))]
    [DisallowMultipleComponent]
    public class ThirdPersonCharacter : CharacterInputBase<CharacterMover>
    {
        protected override void Reset()
        {
            base.Reset();
            orientationPoint = Camera.main.transform;
        }

        protected override void MovePerformed(InputAction.CallbackContext context)
        {
            var direction2D = context.ReadRevalue<Vector2>();
            var direction = new Vector3(direction2D.x, 0f, direction2D.y);
            direction = Orientate(direction);

            characterMover.Move(direction, sprintReaction.reaction ?? false);
            characterMover.TurnTowards(direction);
        }

        #region Orientation
        [field: Header("Orientation")]
        [field: SerializeField] public Transform? orientationPoint { get; set; }

        public Vector3 Orientate(Vector3 direction)
        {
            if (orientationPoint == null)
            {
                return direction;
            }

            var planarDirection = Vector3.ProjectOnPlane(orientationPoint.forward, transform.up);
            var planarRotation = Quaternion.LookRotation(planarDirection != Vector3.zero ? planarDirection : Vector3.forward, transform.up);
            return planarRotation * direction;
        }
        #endregion
    }
}
