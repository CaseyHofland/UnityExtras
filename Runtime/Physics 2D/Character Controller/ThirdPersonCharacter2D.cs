#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;
using UnityExtras.InputSystem;

namespace UnityExtras
{
    /// <summary>Handle third person character 2D movement through input.</summary>
    [AddComponentMenu("Physics 2D/Third Person Character 2D")]
    [RequireComponent(typeof(CharacterMover2D))]
    [DisallowMultipleComponent]
    public class ThirdPersonCharacter2D : CharacterInputBase<CharacterMover2D>
    {
        protected override void MovePerformed(InputAction.CallbackContext context)
        {
            var speed = context.ReadRevalue<float>();
            var direction = new Vector2(speed, 0f);
            characterMover.Move(direction, sprintReaction.reaction?.isPerformed ?? false);
            characterMover.Turn(speed > 0f);
        }
    }
}
