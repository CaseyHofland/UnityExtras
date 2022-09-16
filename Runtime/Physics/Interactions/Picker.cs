#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;

using Input = UnityExtras.InputSystem.Input;

namespace UnityExtras
{
    /// <include file='./Picker.xml' path='docs/Picker/*'/>
    [DisallowMultipleComponent]
    public class Picker : MonoBehaviour
    {
        [field: Header("Pick Up Cast Settings")]
        [field: SerializeField][field: Tooltip("The cast radius.")][field: Min(0f)] public float radius { get; set; } = 0.1f;
        [field: SerializeField][field: Tooltip("The cast distance.")][field: Min(0f)] public float distance { get; set; } = 8f;
        [field: SerializeField][field: Tooltip("The cast layers.")] public LayerMask layers { get; set; } = Physics.AllLayers;
        [field: SerializeField][field: Tooltip("The cast trigger interaction.")] public QueryTriggerInteraction queryTriggerInteraction { get; set; }

        [field: Header("Input (optional)")]
        [field: SerializeField][field: Tooltip("An optional input for triggering the Picker to cast for a PickUp, or drop the currently held PickUp.")] public Input holdInput { get; set; }

        /// <summary>The <see cref="PickUp"/> that is currently held, or <see langword="null"/> otherwise.</summary>
        public PickUp? heldPickUp { get; private set; }

        private void Awake()
        {
            if (holdInput.action != null)
            {
                holdInput.action.performed += HoldPerformed;
            }
        }

        private void OnDestroy()
        {
            if (holdInput.action != null)
            {
                holdInput.action.performed -= HoldPerformed;
            }

            Drop();
        }

        private void HoldPerformed(InputAction.CallbackContext context)
        {
            if (heldPickUp != null)
            {
                Drop();
            }
            else
            {
                TryHold(out _);
            }
        }

        /// <include file='./Picker.xml' path='docs/PickUpCast/*'/>
        public bool PickUpCast(out PickUp? pickUp)
        {
            pickUp = null;
            return Physics.SphereCast(transform.position, radius, transform.forward, out var hit, distance, layers, queryTriggerInteraction)
                && hit.rigidbody
                && hit.rigidbody.TryGetComponent(out pickUp);
        }

        /// <include file='./Picker.xml' path='docs/HeldOrTryHold/*'/>
        public bool HeldOrTryHold(out PickUp? pickUp) => (pickUp = heldPickUp) != null || TryHold(out pickUp);

        /// <include file='./Picker.xml' path='docs/TryHold/*'/>
        public bool TryHold(out PickUp? pickUp)
        {
            bool result;
            if (result = PickUpCast(out pickUp))
            {
                Hold(pickUp!);
            }

            return result;
        }

        /// <include file='./Picker.xml' path='docs/Hold/*'/>
        public void Hold(PickUp pickUp)
        {
            if (heldPickUp == pickUp)
            {
                return;
            }

            Drop();

            heldPickUp = pickUp;
            pickUp.Hold(this);
        }

        /// <include file='./Picker.xml' path='docs/Drop/*'/>
        public void Drop()
        {
            var tmp = heldPickUp;
            heldPickUp = null;

            if (tmp != null)
            {
                tmp.Drop();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (PickUpCast(out _))
            {
                Gizmos.color = Color.green;
            }

            ExtraGizmos.DrawWireCapsule(transform.position, transform.position + transform.forward * distance, radius);
        }
    }
}
