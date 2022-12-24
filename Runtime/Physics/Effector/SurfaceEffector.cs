#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtras
{
    public class SurfaceEffector : Effector
    {
        [Serializable]
        private struct Force
        {
            public Vector2 speed;
            public Vector2 speedVariation;
            public float forceScale;
        }

        [Serializable]
        private struct Options
        {
            public bool useContactForce;
            public bool useFriction;
            public bool useBounce;
        }

        [SerializeField] private Force _force = new() { speed = Vector2.right, forceScale = 0.1f };
        [Space(6f)][SerializeField] private Options _options = new() { useFriction = true, useBounce = true };

        #region public getters setters
        public Vector2 speed
        {
            get => _force.speed;
            set => _force.speed = value;
        }

        public Vector2 speedVariation
        {
            get => _force.speedVariation;
            set => _force.speedVariation = value;
        }

        public float forceScale
        {
            get => _force.forceScale;
            set => _force.forceScale = Mathf.Clamp01(value);
        }

        public bool useContactForce
        {
            get => _options.useContactForce;
            set => _options.useContactForce = value;
        }

        public bool useFriction
        {
            get => _options.useFriction;
            set => _options.useFriction = value;
        }

        public bool useBounce
        {
            get => _options.useBounce;
            set => _options.useBounce = value;
        }

        private void OnValidate()
        {
            speed = speed;
            speedVariation = speedVariation;
            forceScale = forceScale;
            useContactForce = useContactForce;
            useFriction = useFriction;
            useBounce = useBounce;
        }
        #endregion

        private List<Collider> colliders = new();

        private void OnEnable()
        {
            Physics.ContactModifyEvent += ModifyContacts;
            Physics.ContactModifyEventCCD += ModifyContacts;
        }

        private void OnDisable()
        {
            Physics.ContactModifyEvent -= ModifyContacts;
            Physics.ContactModifyEventCCD -= ModifyContacts;
        }

        private void ModifyContacts(PhysicsScene physicsScene, Unity.Collections.NativeArray<ModifiableContactPair> contactPairs)
        {
            for (int i = 0; i < contactPairs.Length; i++)
            {
                var contactPair = contactPairs[i];

                Vector3 avgVelocity;
                Vector3 velocityNormal;
                if (useContactForce && contactPair.contactCount > 0)
                {
                    avgVelocity = _velocity / contactPair.contactCount;
                    velocityNormal = _velocity.normalized;
                }
                else
                {
                    avgVelocity = velocityNormal = default;
                }

                for (int c = 0; c < contactPair.contactCount; c++)
                {
                    if (useContactForce)
                    {
                        var normal = contactPair.GetNormal(c);
                        contactPair.SetNormal(c, velocityNormal);
                        contactPair.SetTargetVelocity(c, avgVelocity);
                        contactPair.SetNormal(c, normal);
                    }
                    if (!useFriction)
                    {
                        contactPair.SetDynamicFriction(c, 0f);
                        contactPair.SetStaticFriction(c, 0f);
                    }
                    if (!useBounce)
                    {
                        contactPair.SetBounciness(c, 0f);
                    }
                }
            }
        }

        private void Update()
        {
            GetComponents(colliders);
            foreach (var collider in colliders)
            {
                collider.hasModifiableContacts = true;
            }
        }

        private Vector3 _velocity;
        private HashSet<Rigidbody> _rigidbodies = new();

        private void FixedUpdate()
        {
            _velocity = transform.rotation * (new Vector3(speed.x, 0f, speed.y) + UnityEngine.Random.value * new Vector3(speedVariation.x, 0f, speedVariation.y));
            _rigidbodies.Clear();
        }

        private void OnCollisionStay(Collision collision)
        {
            if (useContactForce)
            {
                return;
            }

            Rigidbody? rigidbody;
            if ((rigidbody = collision.rigidbody) != null && _rigidbodies.Add(rigidbody))
            {
                rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, _velocity, forceScale);
            }
        }
    }
}