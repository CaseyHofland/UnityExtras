#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
    /// <summary>Follow a <see cref="Picker"/> by follow settings.</summary>
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class PickUp : PickUpBase<Picker, PickUp>
    {
        private Rigidbody? _rigidbody;
        public new Rigidbody rigidbody => _rigidbody ? _rigidbody! : (_rigidbody = GetComponent<Rigidbody>());

        [field: Header("Follow Settings")]
        [field: SerializeField][field: Tooltip("The position to follow relative to the Picker holding this PickUp.")] public Vector3 followCenter { get; set; } = Vector3.forward * 3f;
        [field: SerializeField][field: Tooltip("The rotation to follow when held.")] public Quaternion followRotation { get; set; } = Quaternion.identity;
        [field: SerializeField][field: Tooltip("If the follow rotation should be set based on the PickUp rotation when first held.")] public bool usePickUpRotation { get; set; }
        [field: SerializeField][field: Tooltip("If the follow rotation should factor in the Pickers pitch (x rotation). 0 will always keep the PickUp level, 1 will always point towards the Picker exactly the same no matter of pitch.")][field: Range(0f, 1f)] public float followUpwards { get; set; } = 0f;

        [field: Header("Release Settings")]
        [field: SerializeField][field: Tooltip("Maximum force the PickUp may have when released.")][field: Min(0f)] public float maxReleaseForce { get; set; } = float.PositiveInfinity;
        [field: SerializeField][field: Tooltip("Maximum torque the PickUp may have when released.")][field: Min(0f)] public float maxReleaseTorque { get; set; } = float.PositiveInfinity;

        [field: Header("Physics Settings")]
        [field: SerializeField][field: Tooltip("Settings for the underlying TargetJoint.")] public TargetSettings targetSettings { get; set; } = new();
        [field: SerializeField][field: Tooltip("Settings for the underlying GyroJoint.")] public TargetSettings gyroSettings { get; set; } = new();
        [field: SerializeField][field: Tooltip("Settings for how to manipulate the Rigidbody on held.")] public RigidbodySettings rigidbodySettings { get; set; } = new();

        [field: Header("Break Settings")]
        [field: SerializeField][field: Tooltip("Maximum distance the PickUp may have before breaking.")][field: Min(0f)] public float breakDistance { get; set; } = 8f;
        [field: SerializeField][field: Tooltip("Obstruction layers that break the PickUp when obstructing the PickUp and Picker.")] public LayerMask breakLayers { get; set; }

        #region Wrappers
        [Serializable]
        public class TargetSettings
        {
            [field: SerializeField] public bool enabled { get; set; } = true;
            [field: SerializeField][field: Min(0f)] public float maxForce { get; set; } = float.PositiveInfinity;
            [field: SerializeField][field: Range(0f, 1f)] public float dampingRatio { get; set; } = 1f;
            [field: SerializeField][field: Min(0f)] public float frequency { get; set; } = 5f;
            [field: SerializeField][field: Tooltip("Maximum force the joint can withstand before breaking. Infinity means unbreakable. [0.001, infinity].")][field: Min(0f)] public float breakForce { get; set; } = float.PositiveInfinity;
        }

        [Serializable]
        public class RigidbodySettings
        {
            [field: SerializeField] public ValueStore<float> mass { get; set; } = new(1f);
            [field: SerializeField] public ValueStore<float> drag { get; set; } = new(0f, StoreMethod.Store);
            [field: SerializeField] public ValueStore<float> angularDrag { get; set; } = new(0f, StoreMethod.Store);
            [field: SerializeField] public ValueStore<bool> useGravity { get; set; } = new(false, StoreMethod.Store);
            [field: SerializeField] public ValueStore<bool> isKinematic { get; set; } = new(false, StoreMethod.Override);
            [field: SerializeField] public ValueStore<RigidbodyInterpolation> interpolation { get; set; } = new(RigidbodyInterpolation.Interpolate, StoreMethod.Store);
            [field: SerializeField] public ValueStore<CollisionDetectionMode> collisionDetectionMode { get; set; } = new();
            [field: SerializeField] public ValueStore<RigidbodyConstraints> constraints { get; set; } = new();

            public void Store(Rigidbody rigidbody)
            {
                rigidbody.mass = mass.StoreValue(rigidbody.mass);
                rigidbody.drag = drag.StoreValue(rigidbody.drag);
                rigidbody.angularDrag = angularDrag.StoreValue(rigidbody.angularDrag);
                rigidbody.useGravity = useGravity.StoreValue(rigidbody.useGravity);
                rigidbody.isKinematic = isKinematic.StoreValue(rigidbody.isKinematic);
                rigidbody.interpolation = interpolation.StoreValue(rigidbody.interpolation);
                rigidbody.collisionDetectionMode = collisionDetectionMode.StoreValue(rigidbody.collisionDetectionMode);
                rigidbody.constraints = constraints.StoreValue(rigidbody.constraints);
            }

            public void Restore(Rigidbody rigidbody)
            {
                rigidbody.mass = mass.storedValue;
                rigidbody.drag = drag.storedValue;
                rigidbody.angularDrag = angularDrag.storedValue;
                rigidbody.useGravity = useGravity.storedValue;
                rigidbody.isKinematic = isKinematic.storedValue;
                rigidbody.interpolation = interpolation.storedValue;
                rigidbody.collisionDetectionMode = collisionDetectionMode.storedValue;
                rigidbody.constraints = constraints.storedValue;
            }
        }
        #endregion

        private TargetJoint? targetJoint;
        private GyroJoint? gyroJoint;

        private void LateUpdate()
        {
            TryBreak();
            Follow();
        }

        protected virtual void Follow()
        {
            if (holdingPicker == null)
            {
                return;
            }

            if (targetJoint == null
                || gyroJoint == null)
            {
                this.Drop();
                return;
            }

            // Set target settings.
            targetJoint.enabled = targetSettings.enabled;
            targetJoint.maxForce = targetSettings.maxForce;
            targetJoint.dampingRatio = targetSettings.dampingRatio;
            targetJoint.frequency = targetSettings.frequency;
            targetJoint.breakForce = targetSettings.breakForce;

            gyroJoint.enabled = gyroSettings.enabled;
            gyroJoint.maxTorque = gyroSettings.maxForce;
            gyroJoint.dampingRatio = gyroSettings.dampingRatio;
            gyroJoint.frequency = gyroSettings.frequency;
            gyroJoint.breakTorque = gyroSettings.breakForce;

            // Set position target.
            var tmp = rigidbody.detectCollisions;
            rigidbody.detectCollisions = false;
            if (followCenter != Vector3.zero
                && Physics.Linecast(holdingPicker.transform.position, holdingPicker.transform.position + followCenter, out var hit, ExtraPhysics.GetLayerCollisionMask(gameObject.layer), QueryTriggerInteraction.Ignore))
            {
                targetJoint.target = hit.point;
            }
            else
            {
                targetJoint.target = holdingPicker.transform.position + holdingPicker.transform.rotation * followCenter;
            }
            rigidbody.detectCollisions = tmp;

            // Set gyro target.
            var gyroTargetEuler = holdingPicker.transform.eulerAngles;
            gyroTargetEuler.x *= followUpwards;
            gyroJoint.target = followRotation * Quaternion.Euler(gyroTargetEuler);
        }

        protected virtual void TryBreak()
        {
            if (holdingPicker == null)
            {
                return;
            }

            var tmp = rigidbody.detectCollisions;
            rigidbody.detectCollisions = false;

            // Drop the pick up if the connection broke.
            if ((transform.position - holdingPicker.transform.position).sqrMagnitude >= breakDistance * breakDistance
                || Physics.Linecast(holdingPicker.transform.position, rigidbody.position, breakLayers, QueryTriggerInteraction.Ignore))
            {
                this.Drop();
            }

            rigidbody.detectCollisions = tmp;
        }

        protected override void OnHold(Picker picker)
        {
            // Add a TargetJoint and GyroJoint to be used by PickUp for following the Picker.
            targetJoint = gameObject.AddComponent<TargetJoint>();
            gyroJoint = gameObject.AddComponent<GyroJoint>();
            targetJoint.hideFlags = gyroJoint.hideFlags = HideFlags.HideInInspector;
            targetJoint.autoConfigureTarget = gyroJoint.autoConfigureTarget = false;

            if (usePickUpRotation)
            {
                followRotation = Quaternion.Inverse(picker.transform.rotation);
                //followRotation = Quaternion.Inverse(Quaternion.LookRotation(picker.transform.forward));
            }

            rigidbodySettings.Store(rigidbody);
        }

        protected override void OnDrop()
        {
            Destroy(targetJoint);
            Destroy(gyroJoint);

            rigidbodySettings.Restore(rigidbody);

            // Adjust the velocity for maxReleaseForce.
            if (rigidbody.velocity.sqrMagnitude > maxReleaseForce * maxReleaseForce)
            {
                rigidbody.velocity *= maxReleaseForce / rigidbody.velocity.magnitude;
            }

            // Adjust the torque for maxReleaseTorque.
            if (rigidbody.angularVelocity.sqrMagnitude > maxReleaseTorque * maxReleaseTorque)
            {
                rigidbody.angularVelocity *= maxReleaseTorque / rigidbody.angularVelocity.magnitude;
            }
        }
    }
}
