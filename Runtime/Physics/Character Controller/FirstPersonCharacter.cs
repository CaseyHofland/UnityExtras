#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;
using UnityExtras.InputSystem;

namespace UnityExtras
{
    /// <summary>Handle first person character movement through input.</summary>
    [AddComponentMenu("Physics/First Person Character")]
    [RequireComponent(typeof(CharacterMover))]
    [DisallowMultipleComponent]
    public class FirstPersonCharacter : CharacterInputBase<CharacterMover>
    {
        #region Input
        [field: SerializeField] public InputReaction lookReaction { get; set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (lookReaction.reaction != null)
            {
                lookReaction.reaction.performed += LookPerformed;
            }
        }

        protected override void OnDisable()
        {
            if (lookReaction.reaction != null)
            {
                lookReaction.reaction.performed -= LookPerformed;
            }
        }

        protected override void MovePerformed(InputAction.CallbackContext context)
        {
            var direction2D = context.ReadRevalue<Vector2>();
            var direction = new Vector3(direction2D.x, 0f, direction2D.y);
            characterMover.MoveRelative(direction, sprintReaction.reaction ?? false);
        }

        private void LookPerformed(InputAction.CallbackContext context)
        {
            Look(context.ReadRevalue<Vector2>());
        }
        #endregion

        #region Look
        [field: Header("Look")]
        [field: SerializeField] public Transform? lookTransform { get; set; }
        [field: SerializeField][field: Tooltip("How far in degrees the look transform may look up")][field: Min(0f)] public float topClamp { get; set; } = 90f;
        [field: SerializeField][field: Tooltip("How far in degrees the look transform may look down")][field: Min(0f)] public float bottomClamp { get; set; } = 90f;

        private float _lookPitch;

        /// <summary>Turn the characters looking direction by a given movement.</summary>
        /// <param name="movement">The movement to apply to the looking direction. <see cref="Vector2.x">x</see> is used to turn the whole character, <see cref="Vector2.y">y</see> is only applied to the characters <see cref="lookTransform">look transform</see>.</param>
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
        #endregion
    }
}
