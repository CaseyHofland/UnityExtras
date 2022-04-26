using System;
using UnityEngine;

namespace UnityExtras
{
    [AddComponentMenu("Physics/Slider Joint")]
    [RequireComponent(typeof(Rigidbody))]
    [ExecuteAlways]
    public class SliderJoint : MonoBehaviour
    {
        [SerializeField] private Rigidbody _connectedBody;
        [SerializeField] private ArticulationBody _connectedArticulationBody;
        [SerializeField] private Vector3 _anchor;
        [SerializeField] private bool _autoConfigureConnectedAnchor = true;
        [SerializeField] private Vector3 _connectedAnchor;
        [SerializeField] private SliderAxis _motion;

        [Header("Soft Joint Limit")]
        [SerializeField] private float _spring;
        [SerializeField] private float _damper;

        [SerializeField] private bool _useLimits;
        [SerializeField] private float _limit;
        //[SerializeField] private float _minLimit;
        //[SerializeField] private float _maxLimit;

        [SerializeField] private float _bounciness;
        [SerializeField] private float _contactDistance;
        [Space(12f)]

        // This is the order where the Unimplemented values should be.

        [SerializeField] private float _breakForce = float.PositiveInfinity;
        [SerializeField] private float _breakTorque = float.PositiveInfinity;
        [SerializeField] private bool _enableCollision = false;
        [SerializeField] private bool _enablePreprocessing = true;
        [SerializeField] private float _massScale = 1f;
        [SerializeField] private float _connectedMassScale = 1f;

        [Header("Unimplemented")]

        [SerializeField] private bool _autoConfigureAngle;
        [SerializeField] private Vector3 _angle;
        [SerializeField] private bool _useMotor;
        [SerializeField] private JointMotor _motor;

        #region ConfigurableJoint authoring
        [SerializeField] [HideInInspector] private ConfigurableJoint configurableJoint;

        public Rigidbody connectedBody
        {
            get => configurableJoint.connectedBody;
            set => configurableJoint.connectedBody = _connectedBody = value;
        }

        public ArticulationBody connectedArticulationBody
        {
            get => configurableJoint.connectedArticulationBody;
            set => configurableJoint.connectedArticulationBody = _connectedArticulationBody = value;
        }

        public bool autoConfigureConnectedAnchor
        {
            get => configurableJoint.autoConfigureConnectedAnchor;
            set => configurableJoint.autoConfigureConnectedAnchor = _autoConfigureConnectedAnchor = value;
        }

        public Vector3 anchor
        {
            get => configurableJoint.anchor;
            set => configurableJoint.anchor = _anchor = value;
        }

        public Vector3 connectedAnchor
        {
            get => configurableJoint.connectedAnchor;
            set => configurableJoint.connectedAnchor = _connectedAnchor = value;
        }

        public SliderAxis motion
        {
            get
            {
                switch (_motion)
                {
                    case SliderAxis.X when configurableJoint.xMotion != ConfigurableJointMotion.Locked
                        && configurableJoint.yMotion == ConfigurableJointMotion.Locked
                        && configurableJoint.zMotion == ConfigurableJointMotion.Locked:
                        return SliderAxis.X;
                    case SliderAxis.Y when configurableJoint.xMotion == ConfigurableJointMotion.Locked
                        && configurableJoint.yMotion != ConfigurableJointMotion.Locked
                        && configurableJoint.zMotion == ConfigurableJointMotion.Locked:
                        return SliderAxis.Y;
                    case SliderAxis.Z when configurableJoint.xMotion == ConfigurableJointMotion.Locked
                        && configurableJoint.yMotion == ConfigurableJointMotion.Locked
                        && configurableJoint.zMotion != ConfigurableJointMotion.Locked:
                        return SliderAxis.Z;
                    default:
                        throw new InvalidConfigurationException($"The underlying joint is not in any valid Axis configuration. The joint has likely been modified unintentionally.");
                }
            }
            set
            {
                switch (_motion = value)
                {
                    case SliderAxis.X:
                        configurableJoint.xMotion = _useLimits ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free;
                        configurableJoint.yMotion = ConfigurableJointMotion.Locked;
                        configurableJoint.zMotion = ConfigurableJointMotion.Locked;
                        break;
                    case SliderAxis.Y:
                        configurableJoint.xMotion = ConfigurableJointMotion.Locked;
                        configurableJoint.yMotion = _useLimits ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free;
                        configurableJoint.zMotion = ConfigurableJointMotion.Locked;
                        break;
                    case SliderAxis.Z:
                        configurableJoint.xMotion = ConfigurableJointMotion.Locked;
                        configurableJoint.yMotion = ConfigurableJointMotion.Locked;
                        configurableJoint.zMotion = _useLimits ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free;
                        break;
                }
            }
        }

        public float spring
        {
            get => configurableJoint.linearLimitSpring.spring;
            set
            {
                var linearLimitSpring = configurableJoint.linearLimitSpring;
                linearLimitSpring.spring = _spring = value;
                configurableJoint.linearLimitSpring = linearLimitSpring;
            }
        }

        public float damper
        {
            get => configurableJoint.linearLimitSpring.damper;
            set
            {
                var linearLimitSpring = configurableJoint.linearLimitSpring;
                linearLimitSpring.damper = _damper = value;
                configurableJoint.linearLimitSpring = linearLimitSpring;
            }
        }

        public bool useLimits
        {
            get
            {
                switch (_motion)
                {
                    case SliderAxis.X:
                        return configurableJoint.xMotion == ConfigurableJointMotion.Free;
                    case SliderAxis.Y:
                        return configurableJoint.yMotion == ConfigurableJointMotion.Free;
                    case SliderAxis.Z:
                        return configurableJoint.zMotion == ConfigurableJointMotion.Free;
                    default:
                        throw new InvalidConfigurationException($"The underlying joint is not in any valid Axis configuration. The joint has likely been modified unintentionally.");
                }
            }
            set
            {
                var jointMotion = (_useLimits = value) 
                    ? ConfigurableJointMotion.Limited 
                    : ConfigurableJointMotion.Free;

                switch (_motion)
                {
                    case SliderAxis.X:
                        configurableJoint.xMotion = jointMotion;
                        break;
                    case SliderAxis.Y:
                        configurableJoint.yMotion = jointMotion;
                        break;
                    case SliderAxis.Z:
                        configurableJoint.zMotion = jointMotion;
                        break;
                }
            }
        }

        public float limit
        {
            get => configurableJoint.linearLimit.limit;
            set
            {
                var linearLimit = configurableJoint.linearLimit;
                linearLimit.limit = _limit = value;
                configurableJoint.linearLimit = linearLimit;
            }
        }

        //public float minLimit
        //{
        //    get =>
        //    set =>
        //}

        //public float maxLimit
        //{
        //    get =>
        //    set =>
        //}

        public float bounciness
        {
            get => configurableJoint.linearLimit.bounciness;
            set
            {
                var linearLimit = configurableJoint.linearLimit;
                linearLimit.bounciness = _bounciness = value;
                configurableJoint.linearLimit = linearLimit;
            }
        }

        public float contactDistance
        {
            get => configurableJoint.linearLimit.contactDistance;
            set
            {
                var linearLimit = configurableJoint.linearLimit;
                linearLimit.contactDistance = _contactDistance = value;
                configurableJoint.linearLimit = linearLimit;
            }
        }

        public float breakForce
        {
            get => configurableJoint.breakForce;
            set => configurableJoint.breakForce = _breakForce = value;
        }

        public float breakTorque
        {
            get => configurableJoint.breakTorque;
            set => configurableJoint.breakTorque = _breakTorque = value;
        }

        public bool enableCollision
        {
            get => configurableJoint.enableCollision;
            set => configurableJoint.enableCollision = _enableCollision = value;
        }

        public bool enablePreprocessing
        {
            get => configurableJoint.enablePreprocessing;
            set => configurableJoint.enablePreprocessing = _enablePreprocessing = value;
        }

        public float massScale
        {
            get => configurableJoint.massScale;
            set => configurableJoint.massScale = _massScale = value;
        }

        public float connectedMassScale
        {
            get => configurableJoint.connectedMassScale;
            set => configurableJoint.connectedMassScale = _connectedMassScale = value;
        }

        public static implicit operator Joint(SliderJoint sliderJoint) => sliderJoint.configurableJoint;
        public static explicit operator SliderJoint(Joint joint)
        {
            var configurableJoint = (ConfigurableJoint)joint;
            var sliderJoints = configurableJoint.GetComponents<SliderJoint>();
            var sliderJoint = Array.Find(sliderJoints, sliderJoint => sliderJoint.configurableJoint == configurableJoint);

            return !sliderJoint ? throw new InvalidCastException("Specified cast is not valid.") : sliderJoint;
        }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            if (!configurableJoint)
            {
                configurableJoint = gameObject.AddComponent<ConfigurableJoint>();
            }

            OnValidate();
        }

        private void OnValidate()
        {
            connectedBody = _connectedBody;
            connectedArticulationBody = _connectedArticulationBody;
            autoConfigureConnectedAnchor = _autoConfigureConnectedAnchor;
            anchor = _anchor;

            if (autoConfigureConnectedAnchor)
            {
                _connectedAnchor = connectedAnchor;
            }
            else
            {
                connectedAnchor = _connectedAnchor;
            }

            motion = _motion;

            spring = _spring;
            damper = _damper;
            useLimits = _useLimits;
            limit = _limit;
            bounciness = _bounciness;
            contactDistance = _contactDistance;

            breakForce = _breakForce;
            breakTorque = _breakTorque;
            enableCollision = _enableCollision;
            enablePreprocessing = _enablePreprocessing;
            massScale = _massScale;
            connectedMassScale = _connectedMassScale;

            configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
            configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
            configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;

            configurableJoint.hideFlags |= HideFlags.NotEditable;
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (Application.IsPlaying(this))
            {
                Destroy(configurableJoint);
            }
            else if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(configurableJoint);
            }
#else
            Destroy(configurableJoint);
#endif
        }

        private void OnJointBreak(float breakForce)
        {
            if (breakForce > this.breakForce)
            {
                Destroy(this);
            }
        }
        #endregion
    }
}
