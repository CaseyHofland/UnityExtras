#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;

using Input = UnityExtras.InputSystem.Input;

namespace UnityExtras
{
    /// <summary>Caster to check for <see cref="PickUp"/>.</summary>
    [DisallowMultipleComponent]
    public class Picker : PickerBase<Picker, PickUp>
    {
        [field: Header("Cast Settings")]
        [field: SerializeField][field: Tooltip("The cast radius.")][field: Min(0f)] public float radius { get; set; } = 0.333333f;
        [field: SerializeField][field: Tooltip("The cast distance.")][field: Min(0f)] public float distance { get; set; } = 3f;
        [field: SerializeField][field: Tooltip("The cast layers.")] public LayerMask layers { get; set; } = Physics.AllLayers;
        [field: SerializeField][field: Tooltip("The cast trigger interaction.")] public QueryTriggerInteraction queryTriggerInteraction { get; set; }

        /// <summary>Perform a <see cref="Physics.SphereCast"/> querying for <see cref="PickUp"/>.</summary>
        /// <param name="pickUp">The <see cref="PickUp"/> being hit.</param>
        /// <returns>If the cast hit a <see cref="PickUp"/>.</returns>
        public override bool PickUpCast(out PickUp? pickUp)
        {
            pickUp = null;
            return Physics.SphereCast(transform.position, radius, transform.forward, out var hit, distance, layers, queryTriggerInteraction)
                && hit.rigidbody
                && hit.rigidbody.TryGetComponent(out pickUp);
        }

        [field: Header("Input (optional)")]
        [field: SerializeField][field: Tooltip("An optional input for triggering the Picker to cast for a PickUp, or drop the currently held PickUp.")] public Input holdInput { get; set; }

        #region Unity Methods
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

            this.Drop();
        }

        private void HoldPerformed(InputAction.CallbackContext context)
        {
            if (heldPickUp != null)
            {
                this.Drop();
            }
            else
            {
                this.TryHold(out _);
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
        #endregion
    }
}
