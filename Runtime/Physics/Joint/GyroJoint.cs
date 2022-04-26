#nullable enable
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityExtras
{
    [AddComponentMenu("Physics/Gyro Joint")]
    [RequireComponent(typeof(Rigidbody))]
    [ExecuteAlways]
    public class GyroJoint : MonoBehaviour
    {
        private Rigidbody? _rigidbody;
        public new Rigidbody rigidbody => _rigidbody ? _rigidbody! : (_rigidbody = GetComponent<Rigidbody>());

        [field: SerializeField] public Quaternion target { get; set; }
        [field: SerializeField] public bool autoConfigureTarget { get; set; } = true;
        [field: SerializeField][field: Min(0f)] public float maxTorque { get; set; } = 10_000f;
        [SerializeField] private float _dampingRatio = 1f;
        [field: SerializeField][field: Min(0f)] public float frequency { get; set; } = 5f;
        [field: SerializeField] public float breakTorque { get; set; } = float.PositiveInfinity;

        public float dampingRatio
        {
            get => _dampingRatio;
            set => _dampingRatio = Mathf.Clamp01(value);
        }

        private Vector3 _smoothTorque;

        private void OnValidate()
        {
            dampingRatio = _dampingRatio;
        }

        private void FixedUpdate()
        {
            if (rigidbody.IsSleeping() && target == rigidbody.rotation)
            {
                return;
            }

            AutoConfigureTarget();
            if (_gyroPullDirty)
            {
                PrepareGyroPull();
            }
            SolveGyroPull();
            TryJointBreak();
        }

        private void Update()
        {
            if (!Application.IsPlaying(this))
            {
                AutoConfigureTarget();
            }
        }

        private void AutoConfigureTarget()
        {
            if (autoConfigureTarget && Quaternion.Angle(transform.rotation, rigidbody.rotation) >= Vector2.kEpsilon * Mathf.Rad2Deg)
            {
                target = transform.rotation;
            }
        }

        #region Dirty
        private bool _gyroPullChanged = true;
        private float _lastDampingRatio;
        private float _lastFrequency;
        private float _lastFixedDeltaTime;
        private bool _gyroPullDirty
        {
            get => _gyroPullChanged
                || !_lastDampingRatio.Equals(dampingRatio)
                || !_lastFrequency.Equals(frequency)
                || !_lastFixedDeltaTime.Equals(Time.fixedDeltaTime);
            set
            {
                if (_gyroPullChanged = value)
                {
                    return;
                }

                _lastDampingRatio = dampingRatio;
                _lastFrequency = frequency;
                _lastFixedDeltaTime = Time.fixedDeltaTime;
            }
        }

        private float _gamma;
        private float _beta;
        private float _torqueEqualizer;

        private void PrepareGyroPull()
        {
            // Collect parameterized values.
            var omegaFrequency = 2.0f * Mathf.PI * frequency;
            var dampingCoefficient = 2.0f * dampingRatio * omegaFrequency;
            var springStiffness = (omegaFrequency * omegaFrequency);
            Assert.IsTrue(dampingCoefficient + Time.fixedDeltaTime * springStiffness > float.Epsilon);

            // Magic formulas.
            _gamma = 1.0f / (Time.fixedDeltaTime * (dampingCoefficient + Time.fixedDeltaTime * springStiffness));    // gamma has inverse units.
            _beta = Time.fixedDeltaTime * springStiffness * _gamma;                                                  // beta has units of inverse time.

            // Cache common operations.
            _torqueEqualizer = Mathf.Min(omegaFrequency * Time.fixedDeltaTime * Mathf.Lerp(Mathf.Deg2Rad, 1f, dampingRatio), 1f);

            // Reset the dirty flag.
            _gyroPullDirty = false;
        }
        #endregion

        private void SolveGyroPull()
        {
            // Cheat with some damping.
            if (rigidbody.angularVelocity != Vector3.zero)
            {
                rigidbody.angularVelocity *= 0.98f;
            }

            // Warm starting.
            _smoothTorque *= Time.fixedDeltaTime;
            ApplyTorque(_smoothTorque);

            // Calculate the pull's impulse.
            var deltaEuler = (rigidbody.rotation * Quaternion.Inverse(target)).eulerAngles;
            deltaEuler = new Vector3
            (
                deltaEuler.x > 180f ? deltaEuler.x - 360f : deltaEuler.x,
                deltaEuler.y > 180f ? deltaEuler.y - 360f : deltaEuler.y,
                deltaEuler.z > 180f ? deltaEuler.z - 360f : deltaEuler.z
            ) * Mathf.Deg2Rad;
            var torque = _torqueEqualizer * -(rigidbody.angularVelocity + _beta * deltaEuler + _gamma * _smoothTorque);

            var lastTorque = _smoothTorque;
            _smoothTorque += torque;
            var maxDeltaTorque = Time.fixedDeltaTime * maxTorque * Mathf.Deg2Rad;
            if (_smoothTorque.sqrMagnitude > maxDeltaTorque * maxDeltaTorque)
            {
                _smoothTorque *= maxDeltaTorque / _smoothTorque.magnitude;
                torque = _smoothTorque - lastTorque;
            }

            ApplyTorque(torque);

            void ApplyTorque(Vector3 torque)
            {
                if (torque == Vector3.zero)
                {
                    return;
                }

                rigidbody.angularVelocity += torque;
            }
        }

        private void TryJointBreak()
        {
            if (rigidbody.angularVelocity.sqrMagnitude >= breakTorque * breakTorque)
            {
                SendMessage("OnJointBreak", rigidbody.angularVelocity.magnitude, SendMessageOptions.DontRequireReceiver);
                Destroy(this);
            }
        }
    }
}
