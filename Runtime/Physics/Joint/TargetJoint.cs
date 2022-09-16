#nullable enable
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using static Unity.Mathematics.math;

namespace UnityExtras
{
    [AddComponentMenu("Physics/Target Joint")]
    [RequireComponent(typeof(Rigidbody))]
    [ExecuteAlways]
    public class TargetJoint : MonoBehaviour
    {
        private Rigidbody? _rigidbody;
        public new Rigidbody rigidbody => _rigidbody ? _rigidbody! : (_rigidbody = GetComponent<Rigidbody>());

        [field: SerializeField] public Vector3 anchor { get; set; }
        [field: SerializeField] public Vector3 target { get; set; }
        [field: SerializeField] public bool autoConfigureTarget { get; set; } = true;
        [field: SerializeField][field: Min(0f)] public float maxForce { get; set; } = 10_000f;
        [SerializeField] private float _dampingRatio = 1f;
        [field: SerializeField][field: Min(0f)] public float frequency { get; set; } = 5f;
        [field: SerializeField][field: Tooltip("Maximum force the joint can withstand before breaking. Infinity means unbreakable. [0.001, infinity].")][field: Min(0.001f)] public float breakForce { get; set; } = float.PositiveInfinity;

        public float dampingRatio
        {
            get => _dampingRatio;
            set => _dampingRatio = Mathf.Clamp01(value);
        }

        private float3 _smoothImpulse;

        private void OnValidate()
        {
            dampingRatio = _dampingRatio;
        }

        private void FixedUpdate()
        {
            if (rigidbody.IsSleeping() && target == rigidbody.position)
            {
                return;
            }

            AutoConfigureTarget();
            if (_gyroPullDirty)
            {
                PrepareTargetPull();
            }
            SolveTargetPull();
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
            if (autoConfigureTarget && transform.position != rigidbody.position)
            {
                target = transform.position;
            }
        }

        #region Dirty
        private bool _gyroPullChanged = true;
        private Vector3 _lastAnchor;
        private float _lastDampingRatio;
        private float _lastFrequency;
        private Vector3 _lastCenterOfMass;
        private Vector3 _lastInertiaTensor;
        private float _lastMass;
        private float _lastFixedDeltaTime;
        private bool _gyroPullDirty
        {
            get => _gyroPullChanged
                || !_lastAnchor.Equals(anchor)
                || !_lastDampingRatio.Equals(dampingRatio)
                || !_lastFrequency.Equals(frequency)
                || !_lastCenterOfMass.Equals(rigidbody.centerOfMass)
                || !_lastInertiaTensor.Equals(rigidbody.inertiaTensor)
                || !_lastMass.Equals(rigidbody.mass)
                || !_lastFixedDeltaTime.Equals(Time.fixedDeltaTime);
            set
            {
                if (_gyroPullChanged = value)
                {
                    return;
                }

                _lastAnchor = anchor;
                _lastDampingRatio = dampingRatio;
                _lastFrequency = frequency;
                _lastCenterOfMass = rigidbody.centerOfMass;
                _lastInertiaTensor = rigidbody.inertiaTensor;
                _lastMass = rigidbody.mass;
                _lastFixedDeltaTime = Time.fixedDeltaTime;
            }
        }

        private Vector3 _normalAnchor;
        private bool _normalAnchorNotZero;
        private float _inverseMass;
        private float3 _inverseInertia;
        private float _gamma;
        private float _beta;
        private float3x3 _effectiveMass;

        private void PrepareTargetPull()
        {
            // Collect parameterized values.
            var omegaFrequency = 2.0f * Mathf.PI * frequency;
            var dampingCoefficient = 2.0f * rigidbody.mass * dampingRatio * omegaFrequency;
            var springStiffness = rigidbody.mass * (omegaFrequency * omegaFrequency);
            Assert.IsTrue(dampingCoefficient + Time.fixedDeltaTime * springStiffness > float.Epsilon);

            // Magic formulas.
            _gamma = 1.0f / (Time.fixedDeltaTime * (dampingCoefficient + Time.fixedDeltaTime * springStiffness));    // gamma has units of inverse mass.
            _beta = Time.fixedDeltaTime * springStiffness * _gamma;                                                  // beta has units of inverse time.

            // Cache common operations.
            _normalAnchor = rigidbody.transform.TransformDirection(anchor - rigidbody.centerOfMass);
            _normalAnchorNotZero = !_normalAnchor.Equals(Vector3.zero);

            _inverseMass = select(rcp(rigidbody.mass), 0f, rigidbody.mass == 0f);
            _inverseInertia = select(rcp(rigidbody.inertiaTensor), 0f, (float3)rigidbody.inertiaTensor == 0f);

            // Compute the effective mass matrix.
            {
                var tmp = _inverseMass + _gamma;
                _effectiveMass = new float3x3
                (
                    tmp, 0f, 0f,
                    0f, tmp, 0f,
                    0f, 0f, tmp
                );
            }

            // Add inertia to the effective mass matrix.
            if (_normalAnchorNotZero)
            {
                // X Inertia
                if (_inverseInertia.x != 0f)
                {
                    _effectiveMass += new float3x3
                    (
                        0f, 0f, 0f,
                        0f, _inverseInertia.x * _normalAnchor.z * _normalAnchor.z, -_inverseInertia.x * _normalAnchor.z * _normalAnchor.y,
                        0f, -_inverseInertia.x * _normalAnchor.y * _normalAnchor.z, _inverseInertia.x * _normalAnchor.y * _normalAnchor.y
                    );
                }

                // Y Inertia
                if (_inverseInertia.y != 0f)
                {
                    _effectiveMass += new float3x3
                    (
                        -_inverseInertia.y * _normalAnchor.z * _normalAnchor.x, 0f, _inverseInertia.y * _normalAnchor.x * _normalAnchor.x,
                        0f, 0f, 0f,
                        _inverseInertia.y * _normalAnchor.z * _normalAnchor.z, 0f, -_inverseInertia.y * _normalAnchor.x * _normalAnchor.z
                    );
                }

                // Z Inertia
                if (_inverseInertia.z != 0f)
                {
                    _effectiveMass += new float3x3
                    (
                        _inverseInertia.z * _normalAnchor.y * _normalAnchor.y, -_inverseInertia.z * _normalAnchor.y * _normalAnchor.x, 0f,
                        -_inverseInertia.z * _normalAnchor.x * _normalAnchor.y, _inverseInertia.z * _normalAnchor.x * _normalAnchor.x, 0f,
                        0f, 0f, 0f
                    );
                }
            }

            _effectiveMass = inverse(_effectiveMass);

            // Reset the dirty flag.
            _gyroPullDirty = false;
        }
        #endregion

        private void SolveTargetPull()
        {
            // Cheat with some damping.
            if (rigidbody.angularVelocity != Vector3.zero)
            {
                rigidbody.angularVelocity *= 0.98f;
            }

            // Warm starting.
            _smoothImpulse *= Time.fixedDeltaTime;
            ApplyImpulse(_smoothImpulse);

            // Calculate the pull's impulse.
            float3 cDot = rigidbody.velocity;
            if (_normalAnchorNotZero)
            {
                cDot += cross(rigidbody.angularVelocity, _normalAnchor);
            }
            float3 targetVector = rigidbody.worldCenterOfMass + _normalAnchor - target;
            float3 impulse = mul(_effectiveMass, -(cDot + _beta * targetVector + _gamma * _smoothImpulse));

            // Clamp the impulse to the max force.
            var lastImpulse = _smoothImpulse;
            _smoothImpulse += impulse;
            var maxImpulse = Time.fixedDeltaTime * maxForce;
            if (lengthsq(_smoothImpulse) > maxImpulse * maxImpulse)
            {
                _smoothImpulse *= maxImpulse / length(_smoothImpulse);
                impulse = _smoothImpulse - lastImpulse;
            }

            ApplyImpulse(impulse);

            void ApplyImpulse(Vector3 impulse)
            {
                if (impulse == Vector3.zero)
                {
                    return;
                }

                rigidbody.velocity += _inverseMass * impulse;

                if (_normalAnchorNotZero)
                {
                    rigidbody.angularVelocity += (Vector3)(_inverseInertia * cross(_normalAnchor, impulse));
                }
            }
        }

        private void TryJointBreak()
        {
            if (rigidbody.velocity.sqrMagnitude >= breakForce * breakForce)
            {
                SendMessage("OnJointBreak", rigidbody.velocity.magnitude, SendMessageOptions.DontRequireReceiver);
                Destroy(this);
            }
        }
    }
}