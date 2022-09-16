#nullable enable
using UnityEngine;

namespace UnityExtras
{
    [AddComponentMenu("Physics 2D/Initial Force 2D")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class InitialForce2D : MonoBehaviour
    {
        private Rigidbody2D? _rigidbody2D;
        public new Rigidbody2D rigidbody2D => _rigidbody2D ? _rigidbody2D! : (_rigidbody2D = GetComponent<Rigidbody2D>());

        [field: SerializeField][field: Tooltip("The force to apply globally.")] public Vector2 force { get; set; }
        [field: SerializeField][field: Tooltip("The force to apply locally.")] public Vector2 relativeForce { get; set; }
        [field: SerializeField][field: Tooltip("The torque to apply.")] public float torque { get; set; }

        private void Start()
        {
            rigidbody2D.AddForce(force, ForceMode2D.Impulse);
            rigidbody2D.AddRelativeForce(relativeForce, ForceMode2D.Impulse);
            rigidbody2D.AddTorque(torque, ForceMode2D.Impulse);
            Destroy(this);
        }
    }
}
