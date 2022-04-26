#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class PickUp : MonoBehaviour
    {
        private Rigidbody? _rigidbody;
        public new Rigidbody rigidbody => _rigidbody ? _rigidbody! : (_rigidbody = GetComponent<Rigidbody>());

        [Serializable]
        public class TargetSettings
        {
            [field: SerializeField] public bool enabled { get; set; } = true;
            [field: SerializeField][field: Min(0f)] public float maxForce { get; set; } = float.PositiveInfinity;
            [field: SerializeField][field: Range(0f, 1f)] public float dampingRatio { get; set; } = 1f;
            [field: SerializeField][field: Min(0f)] public float frequency { get; set; } = 5f;
            [field: SerializeField][field: Min(0f)] public float breakForce { get; set; } = float.PositiveInfinity;
        }

        [field: SerializeField] public Vector3 followCenter { get; set; } = Vector3.forward * 5f;
        [field: SerializeField] public Quaternion followRotation { get; set; } = Quaternion.identity;
        [field: SerializeField] public bool usePickUpRotation { get; set; }
        [field: SerializeField][field: Range(0f, 1f)] public float followUpwards { get; set; } = 0f;
        [field: SerializeField] public TargetSettings targetSettings { get; set; } = new TargetSettings();
        [field: SerializeField] public TargetSettings gyroSettings { get; set; } = new TargetSettings();
        [field: SerializeField] public float breakDistance { get; set; } = 8f;
        [field: SerializeField] public LayerMask breakLayers { get; set; }

        public Picker? holdingPicker { get; private set; }
        private TargetJoint? targetJoint;
        private GyroJoint? gyroJoint;

        //private static PhysicMaterial? zeroFriction;

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        //private static void SubsystemRegistration()
        //{
        //    zeroFriction = new PhysicMaterial()
        //    {
        //        dynamicFriction = 0f,
        //        staticFriction = 0f,
        //        bounciness = 0f,
        //        frictionCombine = PhysicMaterialCombine.Minimum,
        //        bounceCombine = PhysicMaterialCombine.Minimum,
        //    };
        //}

        private void LateUpdate()
        {
            Follow();
        }

        private void OnDisable()
        {
            Drop();
        }

        private void Follow()
        {
            if (holdingPicker == null)
            {
                return;
            }

            var tmp = rigidbody.detectCollisions;
            rigidbody.detectCollisions = false;

            // Drop the pick up if the connection broke.
            if (targetJoint == null
                || gyroJoint == null
                || (transform.position - holdingPicker.transform.position).sqrMagnitude >= breakDistance * breakDistance
                || Physics.Linecast(holdingPicker.transform.position, rigidbody.position, breakLayers, QueryTriggerInteraction.Ignore))
            {
                Drop();
            }
            else
            {
                // Set target settings.
                targetJoint.enabled = targetSettings.enabled;
                targetJoint.maxForce = targetSettings.maxForce;
                targetJoint.dampingRatio = targetSettings.dampingRatio;
                targetJoint.frequency = targetSettings.frequency;

                gyroJoint.enabled = gyroSettings.enabled;
                gyroJoint.maxTorque = gyroSettings.maxForce;
                gyroJoint.dampingRatio = gyroSettings.dampingRatio;
                gyroJoint.frequency = gyroSettings.frequency;

                // Set targets.
                if (Physics.Raycast(holdingPicker.transform.position, holdingPicker.transform.forward, out var hit, followCenter.magnitude, ExtraPhysics.GetLayerCollisionMask(gameObject.layer), QueryTriggerInteraction.Ignore))
                {
                    targetJoint.target = hit.point;
                }
                else
                {
                    targetJoint.target = holdingPicker.transform.position + holdingPicker.transform.rotation * followCenter;
                }

                var forward = holdingPicker.transform.forward;
                forward.y *= followUpwards;
                forward.Normalize();
                if (forward == Vector3.zero)
                {
                    forward = Vector3.forward;
                }
                gyroJoint.target = Quaternion.LookRotation(forward) * followRotation;
            }

            rigidbody.detectCollisions = tmp;
        }

        public void Hold(Picker picker)
        {
            if (holdingPicker == picker || !enabled)
            {
                return;
            }

            Drop();

            targetJoint = gameObject.AddComponent<TargetJoint>();
            gyroJoint = gameObject.AddComponent<GyroJoint>();
            targetJoint.hideFlags = gyroJoint.hideFlags = HideFlags.HideInInspector;
            targetJoint.autoConfigureTarget = gyroJoint.autoConfigureTarget = false;

            if (usePickUpRotation)
            {
                followRotation = Quaternion.Inverse(Quaternion.LookRotation(picker.transform.forward));
            }

            holdingPicker = picker;
            picker.Hold(this);
        }

        public void Drop()
        {
            var tmp = holdingPicker;
            holdingPicker = null;

            if (tmp != null)
            {
                Destroy(targetJoint);
                Destroy(gyroJoint);
                tmp.Drop();
            }
        }
    }
}
