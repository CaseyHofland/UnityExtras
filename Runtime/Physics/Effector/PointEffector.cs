#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtras
{
    public class PointEffector : Effector
    {
        [Serializable]
        private struct Force
        {
            [Tooltip("The magnitude of the force to be applied.")] public float forceMagnitude;
            [Tooltip("The variation (a random value from zero to the variation) added to the magnitude of the force to be applied. If the variation is negative then the force will be randomly reduced.")] public float forceVariation;
            [Tooltip("The scale applied to the distance between the source and the target. When using a forceMode that is a function of distance, this is used to scale the calculated distance from source to target i.e. scale the effective distance, it does not change the actual range in which the effector works as that is always controlled by the collider.")] public float distanceScale;
            //[Tooltip("The source for where the effector calculates any force. This allows the 'point' to be defined as the center of mass or the collider position.")] public EffectorSelection2D forceSource;
            //[Tooltip("The target for where the effector applies any force. This allows the force to be applied to the center of mass or the collider position.")] public EffectorSelection2D forceTarget;
            [Tooltip("The mode used to apply the effector force. Constant applies the force the same independent of distance from the point whereas InverseLinear and InverseSquared apply the force as a function of distance.")] public EffectorForceMode2D forceMode;
        }

        [Serializable]
        private struct Damping
        {
            [Tooltip("The linear drag to apply to rigidbodies. This drag will be used on top of what already exists on the rigidbody and will result in reducing its linear speed.")] public float drag;
            [Tooltip("The angular drag to apply to rigidbodies. This drag will be used on top of what already exists on the rigidbody and will result in reducing its angular speed.")] public float angularDrag;
        }

        [SerializeField] private Force _force = new Force { forceMagnitude = -10f, distanceScale = 1f };
        [Space(6f)][SerializeField] private Damping _damping;

        #region public getters setters
        public float forceMagnitude
        {
            get => _force.forceMagnitude;
            set => _force.forceMagnitude = value;
        }

        public float forceVariation
        {
            get => _force.forceVariation;
            set => _force.forceVariation = value;
        }

        public float distanceScale
        {
            get => _force.distanceScale;
            set => _force.distanceScale = Mathf.Max(value, 0.0001f);
        }

        //public EffectorSelection2D forceSource
        //{
        //    get => _force.forceSource;
        //    set => _force.forceSource = value;
        //}

        //public EffectorSelection2D forceTarget
        //{
        //    get => _force.forceTarget;
        //    set => _force.forceTarget = value;
        //}

        public EffectorForceMode2D forceMode
        {
            get => _force.forceMode;
            set => _force.forceMode = value;
        }

        public float drag
        {
            get => _damping.drag;
            set => _damping.drag = Mathf.Max(value, 0f);
        }

        public float angularDrag
        {
            get => _damping.angularDrag;
            set => _damping.angularDrag = Mathf.Max(value, 0f);
        }

        private void OnValidate()
        {
            forceMagnitude = forceMagnitude;
            forceVariation = forceVariation;
            distanceScale = distanceScale;
            //forceSource = forceSource;
            //forceTarget = forceTarget;
            forceMode = forceMode;

            drag = drag;
            angularDrag = angularDrag;
        }
        #endregion

        private float _dragFactor;
        private float _angularDragFactor;
        private HashSet<Rigidbody> _rigidbodies = new();

        private void FixedUpdate()
        {
            _dragFactor = 1f - Mathf.Clamp01(Time.fixedDeltaTime * drag);
            _angularDragFactor = 1f - Mathf.Clamp01(Time.fixedDeltaTime * angularDrag);
            _rigidbodies.Clear();
        }

        private void OnTriggerStay(Collider other)
        {
            Rigidbody? rigidbody;
            if ((rigidbody = other.attachedRigidbody) != null && _rigidbodies.Add(rigidbody))
            {
                var translation = rigidbody.worldCenterOfMass - transform.position;

                var force = forceMagnitude + UnityEngine.Random.value * forceVariation;
                switch (forceMode)
                {
                    case EffectorForceMode2D.Constant:
                        break;
                    case EffectorForceMode2D.InverseLinear:
                        force /= Mathf.Max(translation.magnitude * distanceScale, 1f);
                        break;
                    case EffectorForceMode2D.InverseSquared:
                        force /= Mathf.Max(translation.sqrMagnitude * distanceScale * distanceScale, 1f);
                        break;
                    default:
                        throw new NotImplementedException($"{nameof(forceMode)} {forceMode} doesn't exist / is not implemented.");
                }

                rigidbody.AddForce(translation.normalized * force);

                rigidbody.velocity *= _dragFactor;
                rigidbody.angularVelocity *= _angularDragFactor;
            }
        }
    }
}