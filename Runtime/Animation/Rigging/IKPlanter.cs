#nullable enable
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace UnityExtras.Rigging
{
    [RequireComponent(typeof(TwoBoneIKConstraint))]
    public class IKPlanter : MonoBehaviour
    {
        private TwoBoneIKConstraint? _twoBoneIKConstraint;
        public TwoBoneIKConstraint twoBoneIKConstraint => _twoBoneIKConstraint != null ? _twoBoneIKConstraint : (_twoBoneIKConstraint = GetComponent<TwoBoneIKConstraint>());

        public Transform tip => twoBoneIKConstraint.data.tip;
        public Transform target => twoBoneIKConstraint.data.target;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [field: SerializeField] public Transform constraint { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [field: SerializeField] public Direction solverDirection { get; set; } = new();
        [field: SerializeField] public float rayStep { get; set; } = 0.5f;
        [field: SerializeField] public float rayDip { get; set; } = 0.1f;
        [field: SerializeField] public LayerMask layerMask { get; set; } = Physics.AllLayers;
        [field: SerializeField] public float targetOffset { get; set; }

        private void LateUpdate()
        {
            var direction = (Direction)(constraint.rotation * solverDirection);

            var rayDistance = rayStep + rayDip;
            if (Physics.Raycast(constraint.position - direction * rayStep, direction * rayDistance, out var hit, rayDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                twoBoneIKConstraint.weight = 1f;
                target.position = hit.point - direction * targetOffset;
                target.rotation = Quaternion.FromToRotation(-direction, hit.normal) * constraint.rotation;
            }
            else
            {
                twoBoneIKConstraint.weight = 0f;
            }
        }

        [ContextMenu(nameof(UseLeftFeetBottomOffset))] public void UseLeftFeetBottomOffset() => targetOffset = GetComponentInParent<Animator>(true).leftFeetBottomHeight;
        [ContextMenu(nameof(UseRightFeetBottomOffset))] public void UseRightFeetBottomOffset() => targetOffset = GetComponentInParent<Animator>(true).rightFeetBottomHeight;
        [ContextMenu(nameof(CreateMultiParentConstraint))]
        public void CreateMultiParentConstraint()
        {
            if (constraint == null)
            {
                constraint = new GameObject($"{tip.name} Constraint").transform;
                constraint.SetParent(transform.parent);
                constraint.SetSiblingIndex(transform.GetSiblingIndex());
            }

            if (!constraint.TryGetComponent(out MultiParentConstraint multiParentConstraint))
            {
                multiParentConstraint = constraint.gameObject.AddComponent<MultiParentConstraint>();
            }

            multiParentConstraint.data.constrainedObject = multiParentConstraint.transform;
            multiParentConstraint.data.sourceObjects = new WeightedTransformArray()
            {
                new WeightedTransform(tip, 1f)
            };
            multiParentConstraint.data.constrainedPositionXAxis
                = multiParentConstraint.data.constrainedPositionYAxis
                = multiParentConstraint.data.constrainedPositionZAxis
                = multiParentConstraint.data.constrainedRotationXAxis
                = multiParentConstraint.data.constrainedRotationYAxis
                = multiParentConstraint.data.constrainedRotationZAxis
                = true;
        }

        private void OnDrawGizmosSelected()
        {
            var direction = (Direction)(tip.transform.rotation * solverDirection);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(tip.position, direction * -rayStep);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(tip.position, direction * rayDip);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(tip.position, direction * targetOffset);
        }
    }
}