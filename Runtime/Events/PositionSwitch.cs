#nullable enable
using UnityEngine;

namespace UnityExtras.Events
{
    public class PositionSwitch : ConditionalSwitch
    {
        [field: SerializeField, Tooltip("Meets the condition when the position equals the target position.")] public Transform? target { get; set; }
        [field: SerializeField, Tooltip("An offset for meeting the condition.")] public Vector3 offset { get; set; }
        [field: SerializeField, Tooltip("A radius for meeting the condition.")][field: Min(0f)] public float radius { get; set; } = Vector3.kEpsilon;

        public Vector3 targetPosition => target != null ? target.position + offset : offset;
        public override bool condition => (transform.position - targetPosition).sqrMagnitude <= radius * radius;

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = isOn ? Color.green : Color.red;
            Gizmos.DrawWireSphere(targetPosition, radius);
        }
    }
}