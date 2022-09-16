#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
    /// <include file='./PickUp.xml' path='docs/PickUp/*'/>
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
            [field: SerializeField][field: Tooltip("Maximum force the joint can withstand before breaking. Infinity means unbreakable. [0.001, infinity].")][field: Min(0f)] public float breakForce { get; set; } = float.PositiveInfinity;
        }


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

        public enum StoreMethod
        {
            None,
            Override,
            Break,
        }

        [Serializable]
        public class Store<T>
            where T : struct
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

            public static implicit operator T(Store<T> store) => store.storedValue;

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
        [field: SerializeField][field: Tooltip("Maximum distance the PickUp may have before breaking.")][field: Min(0f)] public float breakDistance { get; set; } = 8f;
        [field: SerializeField][field: Tooltip("Obstruction layers that break the PickUp when obstructing the PickUp and Picker.")] public LayerMask breakLayers { get; set; }

        /// <summary>The <see cref="Picker"/> that is currently holding this <see cref="PickUp"/>, or <see langword="null"/> otherwise.</summary>
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
                targetJoint.breakForce = targetSettings.breakForce;

                gyroJoint.enabled = gyroSettings.enabled;
                gyroJoint.maxTorque = gyroSettings.maxForce;
                gyroJoint.dampingRatio = gyroSettings.dampingRatio;
                gyroJoint.frequency = gyroSettings.frequency;
                gyroJoint.breakTorque = gyroSettings.breakForce;

                // Set position target.
                if (Physics.Raycast(holdingPicker.transform.position, holdingPicker.transform.forward, out var hit, followCenter.magnitude, ExtraPhysics.GetLayerCollisionMask(gameObject.layer), QueryTriggerInteraction.Ignore))
                {
                    targetJoint.target = hit.point;
                }
                else
                {
                    targetJoint.target = holdingPicker.transform.position + holdingPicker.transform.rotation * followCenter;
                }

                // Set gyro target.
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

        /// <include file='./PickUp.xml' path='docs/Hold/*'/>
        public void Hold(Picker picker)
        {
            if (holdingPicker == picker || !enabled)
            {
                return;
            }

            Drop();

            // Add a TargetJoint and GyroJoint to be used by PickUp for following the Picker.
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

        /// <include file='./PickUp.xml' path='docs/Drop/*'/>
        public void Drop()
        {
            var tmp = holdingPicker;
            holdingPicker = null;

            if (tmp != null)
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

                tmp.Drop();
            }
        }
    }
}
