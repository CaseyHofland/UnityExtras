#nullable enable
using UnityEngine;

namespace UnityExtras
{
    [DefaultExecutionOrder(1000)]
    [AddComponentMenu("Physics 2D/Rigidbody Simulator")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class RigidbodySimulator : MonoBehaviour
    {
        private Rigidbody2D? _rigidbody2D;
        public new Rigidbody2D rigidbody2D => _rigidbody2D ? _rigidbody2D! : (_rigidbody2D = GetComponent<Rigidbody2D>());

        [field: SerializeField] public bool simulateDrag { get; set; } = true;
        [field: SerializeField] public bool simulateOnJointBreakMessage { get; set; } = true;

        private void FixedUpdate()
        {
            SimulateDrag();
        }

        private void SimulateDrag()
        {
            if (!simulateDrag)
            {
                return;
            }

            // Zero checks allow the rigidbody to go to sleep.
            if (rigidbody2D.velocity != Vector2.zero)
            {
                rigidbody2D.velocity *= DragCompensation(rigidbody2D.drag);
            }
            if (rigidbody2D.angularVelocity >= Vector2.kEpsilon)
            {
                rigidbody2D.angularVelocity *= DragCompensation(rigidbody2D.angularDrag);
            }
        }

        private static float DragCompensation(float drag)
        {
            var fixedDrag = Time.fixedDeltaTime * drag;
            var dragFactor = 1f - Mathf.Clamp01(fixedDrag);
            var dragFactor2D = 1f + fixedDrag;

            return dragFactor * dragFactor2D;
        }

        private void OnJointBreak2D(Joint2D joint)
        {
            if (!simulateOnJointBreakMessage)
            {
                return;
            }

            var force = joint.reactionForce.magnitude;
            if (force >= joint.breakForce)
            {
                SendMessage("OnJointBreak", force, SendMessageOptions.DontRequireReceiver);
            }

            var torque = joint.reactionTorque;
            if (torque >= joint.breakTorque)
            {
                SendMessage("OnJointBreak", torque, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
