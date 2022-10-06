#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;

using Input = UnityExtras.InputSystem.Input;

namespace UnityExtras
{
    /// <summary>Caster to check for <see cref="PickUp2D"/>.</summary>
    [DisallowMultipleComponent]
    public class Picker2D : PickerBase<Picker2D, PickUp2D>
    {
        [field: Header("Pick Up 2D Cast Settings")]
        [field: SerializeField][field: Tooltip("The cast radius.")][field: Min(0f)] public float radius { get; set; } = 0.333333f;
        [field: SerializeField][field: Tooltip("The cast distance.")][field: Min(0f)] public float distance { get; set; } = 3f;
        [field: SerializeField][field: Tooltip("The cast layers.")] public LayerMask layers { get; set; } = Physics2D.AllLayers;
        [field: SerializeField][field: Tooltip("The cast trigger interaction.")] public float minDepth { get; set; } = float.NegativeInfinity;
        [field: SerializeField][field: Tooltip("The cast trigger interaction.")] public float maxDepth { get; set; } = float.PositiveInfinity;

        /// <summary>Perform a <see cref="Physics2D.CircleCast(Vector2, float, Vector2, float, int, float, float)"/> querying for <see cref="PickUp2D"/>.</summary>
        /// <param name="pickUp2D">The <see cref="PickUp2D"/> being hit.</param>
        /// <returns>If the cast hit a <see cref="PickUp2D"/>.</returns>
        public override bool PickUpCast(out PickUp2D? pickUp2D)
        {
            pickUp2D = null;
            Vector2 direction = transform.right;
            var distance = this.distance * direction.magnitude;
            var hit = Physics2D.CircleCast(transform.position, radius, direction, distance, layers, minDepth, maxDepth);
            return hit.rigidbody
                && hit.rigidbody.TryGetComponent(out pickUp2D);
        }

        [field: Header("Input (optional)")]
        [field: SerializeField][field: Tooltip("An optional input for triggering the Picker2D to cast for a PickUp2D, or drop the currently held PickUp.")] public Input holdInput { get; set; }

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
#if UNITY_EDITOR
            if (PickUpCast(out _))
            {
                UnityEditor.Handles.color = Color.green;
            }

            var start = transform.position;
            Vector2 direction = transform.right;
            var end = start + (Vector3)(distance * direction);
            Vector3 up = radius * ExtraMath.Rotate2D(direction, 90f).normalized;

            UnityEditor.Handles.DrawWireArc(start, Vector3.forward, up, 180f, radius);
            UnityEditor.Handles.DrawAAPolyLine(start + up, end + up);
            UnityEditor.Handles.DrawAAPolyLine(start - up, end - up);
            UnityEditor.Handles.DrawWireArc(end, Vector3.forward, -up, 180f, radius);
#endif
        }
        #endregion
    }
}
