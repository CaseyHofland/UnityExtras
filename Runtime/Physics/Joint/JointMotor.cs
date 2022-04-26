#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
    /// <summary>
    /// The JointMotor is used to motorize a joint.
    /// </summary>
    [Serializable]
    public struct JointMotor : ISerializationCallbackReceiver
    {
        public UnityEngine.JointMotor motor;

        [Tooltip("The motor will apply a force up to force to achieve targetVelocity.")]
        [SerializeField] private float _targetVelocity;

        [Tooltip("The motor will apply a force.")]
        [SerializeField] private float _force;

        [Tooltip("If freeSpin is enabled the motor will only accelerate but never slow down.")]
        [SerializeField] private bool _freeSpin;

        /// <summary>
        /// The motor will apply a force up to force to achieve targetVelocity.
        /// </summary>
        public float targetVelocity
        {
            get => motor.targetVelocity;
            set => motor.targetVelocity = _targetVelocity = value;
        }


        /// <summary>
        /// The motor will apply a force.
        /// </summary>
        public float force
        {
            get => motor.force;
            set => motor.force = _force = value;
        }

        /// <summary>
        /// If freeSpin is enabled the motor will only accelerate but never slow down.
        /// </summary>
        public bool freeSpin
        {
            get => motor.freeSpin;
            set => motor.freeSpin = _freeSpin = value;
        }

        public void OnBeforeSerialize()
        {
            _targetVelocity = motor.targetVelocity;
            _force = motor.force;
            _freeSpin = motor.freeSpin;
        }

        public void OnAfterDeserialize()
        {
            motor.targetVelocity = _targetVelocity;
            motor.force = _force;
            motor.freeSpin = _freeSpin;
        }
    }
}
