#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;
using UnityExtras.InputSystem;

using Input = UnityExtras.InputSystem.Input;

namespace UnityExtras
{
    [AddComponentMenu("Physics 2D/Third Person Character 2D")]
    [RequireComponent(typeof(CharacterMover2D))]
    [DisallowMultipleComponent]
    public class ThirdPersonCharacter2D : MonoBehaviour
    {
        private CharacterMover2D? _characterMover2D;
        public CharacterMover2D characterMover2D => _characterMover2D ? _characterMover2D! : (_characterMover2D = GetComponent<CharacterMover2D>());

        public static implicit operator CharacterMover2D(ThirdPersonCharacter2D thirdPersonCharacter2D) => thirdPersonCharacter2D.characterMover2D;

        #region Input
        [field: Header("Input")]
        [field: SerializeField] public Input moveInput { get; set; }
        [field: SerializeField] public Input sprintInput { get; set; }
        [field: SerializeField] public Input jumpInput { get; set; }

        private void OnEnable()
        {
            moveInput.action?.AddContinuousActions(MovePerformed);
            jumpInput.action?.AddContinuousActions(JumpPerformed);
            sprintInput.action?.AddContinuousActions();
        }

        private void OnDisable()
        {
            moveInput.action?.RemoveContinuousActions(MovePerformed);
            jumpInput.action?.RemoveContinuousActions(JumpPerformed);
            sprintInput.action?.RemoveContinuousActions();
        }

        private void MovePerformed(InputAction.CallbackContext context)
        {
            var speed = context.ReadValue<float>();
            var direction = new Vector2(speed, 0f);
            characterMover2D.Move(direction, sprintInput.action?.IsContinuousPerformed() ?? false);
            characterMover2D.Turn(speed > 0f);
        }

        private void JumpPerformed(InputAction.CallbackContext context)
        {
            characterMover2D.Jump();
        }
        #endregion
    }
}
