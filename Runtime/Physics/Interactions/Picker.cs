#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityExtras
{
    [DisallowMultipleComponent]
    public class Picker : MonoBehaviour
    {
        [field: Header("Pick Up Cast Settings")]
        [field: SerializeField][field: Min(0f)] public float radius { get; set; } = 0.1f;
        [field: SerializeField][field: Min(0f)] public float distance { get; set; } = 8f;
        [field: SerializeField] public LayerMask layers { get; set; } = Physics.AllLayers;
        [field: SerializeField] public QueryTriggerInteraction queryTriggerInteraction { get; set; }

        [field: Header("Input")]
        [field: SerializeField] public InputActionProperty holdAction { get; set; }

        private bool _useInput;

        public PickUp? heldPickUp { get; private set; }

        private void Awake()
        {
            if (_useInput = (holdAction.action != null))
            {
                holdAction.action!.Enable();
                holdAction.action!.performed += HoldPerformed;
            }
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

        private void OnDestroy()
        {
            if (_useInput)
            {
                holdAction.action.performed -= HoldPerformed;
            }

            Drop();
        }

        public bool PickUpCast(out PickUp? pickUp)
        {
            pickUp = null;
            return Physics.SphereCast(transform.position, radius, transform.forward, out var hit, distance, layers, queryTriggerInteraction)
                && hit.rigidbody
                && hit.rigidbody.TryGetComponent(out pickUp);
        }

        public bool HeldOrTryHold(out PickUp? pickUp) => (pickUp = heldPickUp) != null || TryHold(out pickUp);

        public bool TryHold(out PickUp? pickUp)
        {
            bool result;
            if (result = PickUpCast(out pickUp))
            {
                Hold(pickUp!);
            }

            return result;
        }

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
