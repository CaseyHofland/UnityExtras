#nullable enable
using UnityEditor;
using UnityEngine;

namespace UnityExtras.Editor
{
    public static partial class Menus
    {
        [MenuItem("CONTEXT/" + nameof(CharacterController) + "/Set Recommended Rigidbody")]
        private static void SetRecommendedRigidbody(MenuCommand command)
        {
            var characterController = (CharacterController)command.context;
            if (!characterController.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody = Undo.AddComponent<Rigidbody>(characterController.gameObject);
            }

            rigidbody.isKinematic = true;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }
}
