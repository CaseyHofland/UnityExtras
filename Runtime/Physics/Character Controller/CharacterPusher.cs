#nullable enable
using UnityEngine;

namespace UnityExtras
{
    [AddComponentMenu("Physics/Character Pusher")]
    [DisallowMultipleComponent]
	public class CharacterPusher : MonoBehaviour
	{
		private const float downPushThreshold = -0.3f;

		[field: SerializeField] public LayerMask pushLayers { get; set; } = Physics.AllLayers;
		[field: SerializeField] [field: Min(0f)] public float pushStrength { get; set; } = 1.1f;

		private void Start() { } // Included to make enabled toggle show up in inspector.

        private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			// https://docs.unity3d.com/ScriptReference/CharacterController.OnControllerColliderHit.html

			// Check the script is enabled.
			if (!isActiveAndEnabled)
            {
				return;
            }

			// Make sure we hit a non kinematic rigidbody.
			Rigidbody body = hit.rigidbody;
			if (body == null || body.isKinematic)
			{
				return;
			}

			// Make sure we only push desired layer(s).
			var bodyLayerMask = 1 << body.gameObject.layer;
			if ((bodyLayerMask & pushLayers.value) == 0)
			{
				return;
			}

			// We don't want to push objects below us.
			if (hit.moveDirection.y < downPushThreshold)
			{
				return;
			}

			// Calculate the push direction from the move direction, horizontal motion only.
			Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

			// Apply the push taking strength into account.
			body.AddForce(pushDirection * pushStrength, ForceMode.Impulse);
		}
	}
}
