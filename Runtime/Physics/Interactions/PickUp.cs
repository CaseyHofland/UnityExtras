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

        [field: Header("Follow Settings")]
        [field: SerializeField] public Vector3 followCenter { get; set; } = Vector3.forward * 5f;
        [field: SerializeField] public Quaternion followRotation { get; set; } = Quaternion.identity;
        [field: SerializeField] public bool usePickUpRotation { get; set; }
        [field: SerializeField][field: Range(0f, 1f)] public float followUpwards { get; set; } = 0f;

        [field: Header("Release Settings")]
        [field: SerializeField] public float maxReleaseForce { get; set; } = float.PositiveInfinity;
        [field: SerializeField] public float maxReleaseTorque { get; set; } = float.PositiveInfinity;

        [field: Header("Physics Settings")]
        [field: SerializeField] public TargetSettings targetSettings { get; set; } = new TargetSettings();
        [field: SerializeField] public TargetSettings gyroSettings { get; set; } = new TargetSettings();
        [field: SerializeField] public RigidbodySettings rigidbodySettings { get; set; } = new RigidbodySettings();

        public enum StoreMethod
        {
            None,
            Override,
            Break,
        }

        [Serializable]
        public class Store<T>
        {
            [field: SerializeField] public T value { get; set; }
            [field: SerializeField] public StoreMethod storeMethod { get; set; }
            public T storedValue { get; private set; }

            public Store() : this(default, default) { }
            public Store(T value) : this(value, default) { }
            public Store(T value, StoreMethod storeMethod)
            {
                this.value = this.storedValue = value;
                this.storeMethod = storeMethod;
            }

            public T StoreValue(T value)
            {
                switch (storeMethod)
                {
                    case StoreMethod.Override:
                        this.storedValue = value;
                        return this.value;
                    case StoreMethod.Break:
                        this.storedValue = this.value;
                        return this.value;
                    default:
                        this.storedValue = value;
                        return value;
                }
            }
        }

        [Serializable]
        public class RigidbodySettings
        {
            [field: SerializeField] public Store<float> mass { get; set; } = new Store<float>(1f);
            [field: SerializeField] public Store<float> drag { get; set; } = new Store<float>(0f, StoreMethod.Override);
            [field: SerializeField] public Store<float> angularDrag { get; set; } = new Store<float>(0f, StoreMethod.Override);
            [field: SerializeField] public Store<bool> useGravity { get; set; } = new Store<bool>(false, StoreMethod.Override);
            [field: SerializeField] public Store<bool> isKinematic { get; set; } = new Store<bool>(false, StoreMethod.Break);
            [field: SerializeField] public Store<RigidbodyInterpolation> interpolation { get; set; } = new Store<RigidbodyInterpolation>();
            [field: SerializeField] public Store<CollisionDetectionMode> collisionDetectionMode { get; set; } = new Store<CollisionDetectionMode>();
            [field: SerializeField] public Store<RigidbodyConstraints> constraints { get; set; } = new Store<RigidbodyConstraints>();

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

        [field: Header("Break Settings")]
        [field: SerializeField] public float breakDistance { get; set; } = 8f;
        [field: SerializeField] public LayerMask breakLayers { get; set; }

        public Picker? holdingPicker { get; private set; }
        private TargetJoint? targetJoint;
        private GyroJoint? gyroJoint;

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

            rigidbodySettings.Store(rigidbody);

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

                rigidbodySettings.Restore(rigidbody);

                if (rigidbody.velocity.sqrMagnitude > maxReleaseForce * maxReleaseForce)
                {
                    rigidbody.velocity *= maxReleaseForce / rigidbody.velocity.magnitude;
                }

                if (rigidbody.angularVelocity.sqrMagnitude > maxReleaseTorque * maxReleaseTorque)
                {
                    rigidbody.angularVelocity *= maxReleaseTorque / rigidbody.angularVelocity.magnitude;
                }

                tmp.Drop();
            }
        }
    }
}
