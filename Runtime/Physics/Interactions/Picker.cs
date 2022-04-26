#nullable enable
using UnityEngine;

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

        public PickUp? heldPickUp { get; private set; }

        private void OnDestroy()
        {
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
    }
}
