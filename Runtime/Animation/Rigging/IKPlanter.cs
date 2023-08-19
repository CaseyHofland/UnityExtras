#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace UnityExtras.Rigging
{
    [RequireComponent(typeof(TwoBoneIKConstraint))]
    public class IKPlanter : MonoBehaviour
    {
        [field: SerializeField, HideInInspector] public TwoBoneIKConstraint twoBoneIKConstraint { get; private set; }

        private void Reset()
        {
            twoBoneIKConstraint = GetComponent<TwoBoneIKConstraint>();
        }

        private void Awake()
        {
            twoBoneIKConstraint = GetComponent<TwoBoneIKConstraint>();
            CreateRigCollisionModel();
        }

        public Transform tip => twoBoneIKConstraint.data.tip;
        public Transform target => twoBoneIKConstraint.data.target;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [field: SerializeField] public Transform constraint { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [field: SerializeField] public RelativeDirection solverDirection { get; set; } = new();
        [field: SerializeField] public float rayStep { get; set; } = 0.5f;
        [field: SerializeField] public float rayDip { get; set; } = 0.1f;
        [field: SerializeField] public LayerMask layerMask { get; set; } = Physics.AllLayers;
        [field: SerializeField] public float targetOffset { get; set; }

        private List<Rigidbody> rigidbodies = new();
        private List<Collider> colliders = new();

        private void LateUpdate()
        {
            rigidbodies.ForEach(DisableRigidbody);
            colliders.ForEach(DisableCollider);

            var rayDistance = rayStep + rayDip;
            if (Physics.Raycast(constraint.position - rayStep * (Direction)solverDirection, rayDistance * solverDirection, out var hit, rayDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                twoBoneIKConstraint.weight = 1f;
                target.position = hit.point - targetOffset * solverDirection;
                target.rotation = Quaternion.FromToRotation(-solverDirection, hit.normal) * constraint.rotation;
            }
            else
            {
                twoBoneIKConstraint.weight = 0f;
            }

            colliders.ForEach(EnableCollider);
            rigidbodies.ForEach(EnableRigidbody);
        }

        public void CreateRigCollisionModel()
        {
            var rigBuilder = GetComponentInParent<RigBuilder>();
            if (rigBuilder == null)
            {
                return;
            }

            rigBuilder.GetComponentsInChildren(true, rigidbodies);
            if (rigidbodies.Count == 0)
            {
                rigBuilder.GetComponentsInChildren(true, colliders);
            }
            else
            {
                colliders.Clear();
            }
        }

        private void EnableRigidbody(Rigidbody rigidbody) => rigidbody.detectCollisions = true;
        private void DisableRigidbody(Rigidbody rigidbody) => rigidbody.detectCollisions = false;
        private void EnableCollider (Collider Collider) => Collider.enabled = true;
        private void DisableCollider (Collider Collider) => Collider.enabled = false;

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
            Gizmos.color = Color.green;
            Gizmos.DrawRay(tip.position, solverDirection * -rayStep);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(tip.position, solverDirection * rayDip);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(tip.position, solverDirection * targetOffset);
        }
    }
}