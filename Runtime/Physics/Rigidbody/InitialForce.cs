#nullable enable
using UnityEngine;

namespace UnityExtras
{
    [AddComponentMenu("Physics/Initial Force")]
    [RequireComponent(typeof(Rigidbody))]
    public class InitialForce : MonoBehaviour
    {
        private Rigidbody? _rigidbody;
        public new Rigidbody rigidbody => _rigidbody ? _rigidbody! : (_rigidbody = GetComponent<Rigidbody>());

        [field: SerializeField] public Vector3 force { get; set; }
        [field: SerializeField] public Vector3 relativeForce { get; set; }
        [field: SerializeField] public Vector3 torque { get; set; }
        [field: SerializeField] public Vector3 relativeTorque { get; set; }

        private void Start()
        {
            rigidbody.AddForce(force, ForceMode.Impulse);
            rigidbody.AddRelativeForce(relativeForce, ForceMode.Impulse);
            rigidbody.AddTorque(torque, ForceMode.Impulse);
            rigidbody.AddRelativeTorque(relativeTorque, ForceMode.Impulse);
            Destroy(this);
        }
    }
}
