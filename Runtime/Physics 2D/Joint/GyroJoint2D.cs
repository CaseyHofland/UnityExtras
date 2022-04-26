#nullable enable
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityExtras
{
    [AddComponentMenu("Physics 2D/Gyro Joint 2D")]
    [RequireComponent(typeof(Rigidbody2D))]
    [ExecuteAlways]
    public class GyroJoint2D : MonoBehaviour
    {
        private Rigidbody2D? _rigidbody2D;
        public new Rigidbody2D rigidbody2D => _rigidbody2D ? _rigidbody2D! : (_rigidbody2D = GetComponent<Rigidbody2D>());

        [field: SerializeField] public float target { get; set; }
        [field: SerializeField] public bool autoConfigureTarget { get; set; } = true;
        [SerializeField] private float _maxTorque = 10_000f;
        [SerializeField] private float _dampingRatio = 1f;
        [SerializeField] private float _frequency = 5f;
        [field: SerializeField] public float breakTorque { get; set; } = float.PositiveInfinity;

        public float maxTorque
        {
            get => _maxTorque;
            set => _maxTorque = Mathf.Clamp(value, 0f, 1_000_000f);
        }

        public float dampingRatio
        {
            get => _dampingRatio;
            set => _dampingRatio = Mathf.Clamp01(value);
        }

        public float frequency
        {
            get => _frequency;
            set => _frequency = Mathf.Clamp(value, 0f, 1_000_000f);
        }

        private float _smoothTorque;

        private void OnValidate()
        {
            maxTorque = _maxTorque;
            dampingRatio = _dampingRatio;
            frequency = _frequency;
        }

        private void FixedUpdate()
        {
            if (rigidbody2D.IsSleeping() && Mathf.DeltaAngle(target, rigidbody2D.rotation) < Vector2.kEpsilon * Mathf.Rad2Deg)
            {
                return;
            }

            AutoConfigureTarget();
            if (_gryoPullDirty)
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
            if (autoConfigureTarget && Mathf.DeltaAngle(transform.eulerAngles.z, rigidbody2D.rotation) >= Vector2.kEpsilon * Mathf.Rad2Deg)
            {
                target = transform.eulerAngles.z;
            }
        }

        #region Dirty
        private bool _gyroPullChanged = true;
        private float _lastDampingRatio;
        private float _lastFrequency;
        private float _lastFixedDeltaTime;
        private bool _gryoPullDirty
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

            // Collect impulse influencers.
            _torqueEqualizer = Mathf.Min(omegaFrequency * Time.fixedDeltaTime * Mathf.Lerp(Mathf.Deg2Rad, 1f, dampingRatio), 1f);

            // Reset the dirty flag.
            _gryoPullDirty = false;
        }
        #endregion

        private void SolveGyroPull()
        {
            // Cheat with some damping.
            if (Mathf.Abs(rigidbody2D.angularVelocity) >= Vector2.kEpsilon)
            {
                rigidbody2D.angularVelocity *= 0.98f;
            }

            // Warm starting.
            _smoothTorque *= Time.fixedDeltaTime;
            ApplyImpulse(_smoothTorque);

            // Calculate the pull's impulse.
            var deltaAngle = Mathf.DeltaAngle(target, rigidbody2D.rotation);
            var torque = _torqueEqualizer * -(rigidbody2D.angularVelocity + _beta * deltaAngle + _gamma * _smoothTorque);

            var lastTorque = _smoothTorque;
            _smoothTorque += torque;
            var maxDeltaTorque = Time.fixedDeltaTime * maxTorque;
            if (Mathf.Abs(_smoothTorque) > maxDeltaTorque)
            {
                _smoothTorque *= maxDeltaTorque / Mathf.Abs(_smoothTorque);
                torque = _smoothTorque - lastTorque;
            }

            ApplyImpulse(torque);

            void ApplyImpulse(float torque)
            {
                if (Mathf.Abs(torque) < Vector2.kEpsilon * Mathf.Rad2Deg)
                {
                    return;
                }

                rigidbody2D.angularVelocity += torque;
            }
        }

        private void TryJointBreak()
        {
            var torque = rigidbody2D.angularVelocity;
            if (torque >= breakTorque)
            {
                SendMessage("OnJointBreak", torque, SendMessageOptions.DontRequireReceiver);
                Destroy(this);
            }
        }
    }
}
