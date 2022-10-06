#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
    /// <summary>Follow a <see cref="Picker2D"/> by follow settings.</summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [DisallowMultipleComponent]
    public class PickUp2D : PickUpBase<Picker2D, PickUp2D>
    {
        private Rigidbody2D? _rigidbody2D;
        public new Rigidbody2D rigidbody2D => _rigidbody2D ? _rigidbody2D! : (_rigidbody2D = GetComponent<Rigidbody2D>());

        [field: Header("Follow Settings")]
        [field: SerializeField][field: Tooltip("The position to follow relative to the Picker holding this PickUp.")] public Vector3 followCenter { get; set; }
        [field: SerializeField][field: Tooltip("If the follow position should factor in the Pickers depth (z position). If true, the depth will equal the Pickers plus the follow centers depth until dropped.")] public bool followDepth { get; set; }
        [field: SerializeField][field: Tooltip("The rotation to follow when held.")] public float followRotation { get; set; }
        [field: SerializeField][field: Tooltip("If the follow rotation should be set based on the PickUp rotation when first held.")] public bool usePickUpRotation { get; set; }
        [field: SerializeField][field: Tooltip("If the follow rotation should factor in the Pickers pitch (z rotation). 0 will always keep the PickUp level, 1 will always point towards the Picker exactly the same no matter of pitch.")][field: Range(0f, 1f)] public float followUpwards { get; set; } = 0f;

        [field: Header("Release Settings")]
        [field: SerializeField][field: Tooltip("Maximum force the PickUp may have when released.")][field: Min(0f)] public float maxReleaseForce { get; set; } = float.PositiveInfinity;
        [field: SerializeField][field: Tooltip("Maximum torque the PickUp may have when released.")][field: Min(0f)] public float maxReleaseTorque { get; set; } = float.PositiveInfinity;

        [field: Header("Physics Settings 2D")]
        [field: SerializeField][field: Tooltip("Settings for the underlying TargetJoint.")] public TargetSettings2D targetSettings2D { get; set; } = new();
        [field: SerializeField][field: Tooltip("Settings for the underlying GyroJoint.")] public TargetSettings2D gyroSettings2D { get; set; } = new();
        [field: SerializeField][field: Tooltip("Settings for how to manipulate the Rigidbody on held.")] public RigidbodySettings2D rigidbodySettings2D { get; set; } = new();

        [field: Header("Break Settings")]
        [field: SerializeField][field: Tooltip("Maximum distance the PickUp may have before breaking.")][field: Min(0f)] public float breakDistance { get; set; } = 3f;
        [field: SerializeField][field: Tooltip("Obstruction layers that break the PickUp when obstructing the PickUp and Picker.")] public LayerMask breakLayers { get; set; }

        #region Wrappers
        [Serializable]
        public class TargetSettings2D
        {
            [field: SerializeField] public bool enabled { get; set; } = true;
            [field: SerializeField][field: Min(0f)] public float maxForce { get; set; } = 1_000_000f;
            [field: SerializeField][field: Range(0f, 1f)] public float dampingRatio { get; set; } = 1f;
            [field: SerializeField][field: Min(0f)] public float frequency { get; set; } = 5f;
            [field: SerializeField][field: Tooltip("Maximum force the joint can withstand before breaking. Infinity means unbreakable. [0.001, infinity].")][field: Min(0f)] public float breakForce { get; set; } = float.PositiveInfinity;
        }

        [Serializable]
        public class RigidbodySettings2D
        {
            [field: SerializeField] public ValueStore<RigidbodyType2D> bodyType { get; set; } = new(RigidbodyType2D.Dynamic, StoreMethod.Override);
            [field: SerializeField] public ValueStore<bool> useAutoMass { get; set; } = new(false);
            [field: SerializeField] public ValueStore<float> mass { get; set; } = new(1f);
            [field: SerializeField] public ValueStore<float> drag { get; set; } = new(0f, StoreMethod.Store);
            [field: SerializeField] public ValueStore<float> angularDrag { get; set; } = new(0f, StoreMethod.Store);
            [field: SerializeField] public ValueStore<float> gravityScale { get; set; } = new(0f, StoreMethod.Store);
            [field: SerializeField] public ValueStore<CollisionDetectionMode2D> collisionDetectionMode { get; set; } = new();
            [field: SerializeField] public ValueStore<RigidbodySleepMode2D> sleepMode { get; set; } = new();
            [field: SerializeField] public ValueStore<RigidbodyInterpolation2D> interpolation { get; set; } = new(RigidbodyInterpolation2D.Interpolate, StoreMethod.Store);
            [field: SerializeField] public ValueStore<RigidbodyConstraints2D> constraints { get; set; } = new();

            public void Store(Rigidbody2D rigidbody2D)
            {
                rigidbody2D.bodyType = bodyType.StoreValue(rigidbody2D.bodyType);
                rigidbody2D.useAutoMass = useAutoMass.StoreValue(rigidbody2D.useAutoMass);
                rigidbody2D.mass = mass.StoreValue(rigidbody2D.mass);
                rigidbody2D.drag = drag.StoreValue(rigidbody2D.drag);
                rigidbody2D.angularDrag = angularDrag.StoreValue(rigidbody2D.angularDrag);
                rigidbody2D.gravityScale = gravityScale.StoreValue(rigidbody2D.gravityScale);
                rigidbody2D.collisionDetectionMode = collisionDetectionMode.StoreValue(rigidbody2D.collisionDetectionMode);
                rigidbody2D.sleepMode = sleepMode.StoreValue(rigidbody2D.sleepMode);
                rigidbody2D.interpolation = interpolation.StoreValue(rigidbody2D.interpolation);
                rigidbody2D.constraints = constraints.StoreValue(rigidbody2D.constraints);
            }

            public void Restore(Rigidbody2D rigidbody2D)
            {
                rigidbody2D.bodyType = bodyType.storedValue;
                rigidbody2D.useAutoMass = useAutoMass.storedValue;
                rigidbody2D.mass = mass.storedValue;
                rigidbody2D.drag = drag.storedValue;
                rigidbody2D.angularDrag = angularDrag.storedValue;
                rigidbody2D.gravityScale = gravityScale.storedValue;
                rigidbody2D.collisionDetectionMode = collisionDetectionMode.storedValue;
                rigidbody2D.sleepMode = sleepMode.storedValue;
                rigidbody2D.interpolation = interpolation.storedValue;
                rigidbody2D.constraints = constraints.storedValue;
            }
        }
        #endregion

        private TargetJoint2D? targetJoint2D;
        private GyroJoint2D? gyroJoint2D;

        private float depthOnHold;

        protected virtual void LateUpdate()
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

            if (targetJoint2D == null
                || gyroJoint2D == null)
            {
                this.Drop();
                return;
            }

            // Set target settings.
            targetJoint2D.enabled = targetSettings2D.enabled;
            targetJoint2D.maxForce = targetSettings2D.maxForce;
            targetJoint2D.dampingRatio = targetSettings2D.dampingRatio;
            targetJoint2D.frequency = targetSettings2D.frequency;
            targetJoint2D.breakForce = targetSettings2D.breakForce;

            gyroJoint2D.enabled = gyroSettings2D.enabled;
            gyroJoint2D.maxTorque = gyroSettings2D.maxForce;
            gyroJoint2D.dampingRatio = gyroSettings2D.dampingRatio;
            gyroJoint2D.frequency = gyroSettings2D.frequency;
            gyroJoint2D.breakTorque = gyroSettings2D.breakForce;

            // Set position target.
            var tmp = rigidbody2D.simulated;
            rigidbody2D.simulated = false;
            var hit = Physics2D.Linecast(holdingPicker.transform.position, holdingPicker.transform.position + followCenter, Physics2D.GetLayerCollisionMask(gameObject.layer));
            if (hit.collider)
            {
                targetJoint2D.target = hit.point;
            }
            else
            {
                targetJoint2D.target = holdingPicker.transform.position + holdingPicker.transform.rotation * followCenter;
            }
            rigidbody2D.simulated = tmp;

            // Set gyro target.
            gyroJoint2D.target = followRotation + holdingPicker.transform.eulerAngles.z * followUpwards;

            // Set position depth.
            if (followDepth)
            {
                var position = transform.position;
                position.z = holdingPicker.transform.position.z + followCenter.z;
                transform.position = position;
            }
        }

        protected virtual void TryBreak()
        {
            if (holdingPicker == null)
            {
                return;
            }

            var tmp = rigidbody2D.simulated;
            rigidbody2D.simulated = false;

            // Drop the pick up if the connection broke.
            if ((transform.position - holdingPicker.transform.position).sqrMagnitude >= breakDistance * breakDistance
                || Physics2D.Linecast(holdingPicker.transform.position, rigidbody2D.position, breakLayers))
            {
                this.Drop();
            }

            rigidbody2D.simulated = tmp;
        }

        protected override void OnHold(Picker2D picker)
        {
            // Add a TargetJoint and GyroJoint to be used by PickUp for following the Picker.
            targetJoint2D = gameObject.AddComponent<TargetJoint2D>();
            gyroJoint2D = gameObject.AddComponent<GyroJoint2D>();
            targetJoint2D.hideFlags = gyroJoint2D.hideFlags = HideFlags.HideInInspector;
            targetJoint2D.autoConfigureTarget = gyroJoint2D.autoConfigureTarget = false;

            if (usePickUpRotation)
            {
                followRotation = -picker.transform.eulerAngles.z;
            }

            if (followDepth)
            {
                depthOnHold = transform.position.z;
            }

            rigidbodySettings2D.Store(rigidbody2D);
        }

        protected override void OnDrop()
        {
            Destroy(targetJoint2D);
            Destroy(gyroJoint2D);

            rigidbodySettings2D.Restore(rigidbody2D);

            // Adjust the velocity for maxReleaseForce.
            if (rigidbody2D.velocity.sqrMagnitude > maxReleaseForce * maxReleaseForce)
            {
                rigidbody2D.velocity *= maxReleaseForce / rigidbody2D.velocity.magnitude;
            }

            // Adjust the torque for maxReleaseTorque.
            if (rigidbody2D.angularVelocity > maxReleaseTorque)
            {
                rigidbody2D.angularVelocity = maxReleaseTorque;
            }

            var position = transform.position;
            position.z = depthOnHold;
            transform.position = position;
        }
    }
}
