#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
    [AddComponentMenu("Physics/Slider Joint")]
    [RequireComponent(typeof(Rigidbody))]
    public class SliderJoint : MonoBehaviour, IAuthor
    {
        [SerializeField][HideInInspector] private Rigidbody? _rigidbody;
        public new Rigidbody rigidbody => _rigidbody ? _rigidbody! : (_rigidbody = GetComponent<Rigidbody>());

        [SerializeField][HideInInspector] private RequiredComponent<ConfigurableJoint> _configurableJoint;
        public ConfigurableJoint configurableJoint => _configurableJoint.GetComponent(gameObject, HideFlags.HideInInspector);

        public static implicit operator ConfigurableJoint(SliderJoint sliderJoint) => sliderJoint.configurableJoint;

        [SerializeField] private Rigidbody? _connectedBody;
        public Rigidbody? connectedBody
        {
            get => configurableJoint.connectedBody;
            set => configurableJoint.connectedBody = _connectedBody = value;
        }

        //[SerializeField] private ArticulationBody? _connectedArticulationBody;
        //public ArticulationBody? connectedArticulationBody
        //{
        //    get => configurableJoint.connectedArticulationBody;
        //    set => configurableJoint.connectedArticulationBody = _connectedArticulationBody = value;
        //}

        [SerializeField] private Vector3 _anchor;
        public Vector3 anchor
        {
            get => configurableJoint.anchor;
            set => configurableJoint.anchor = _anchor = value;
        }

        [SerializeField] private Vector3 _connectedAnchor;
        public Vector3 connectedAnchor
        {
            get => configurableJoint.connectedAnchor - _connectedAnchorOffset;
            set => configurableJoint.connectedAnchor = (_connectedAnchor = value) + _connectedAnchorOffset;
        }

        [SerializeField] private Quaternion _angle;
        public Quaternion angle
        {
            get => rigidbody.rotation * Quaternion.LookRotation(Vector3.Cross(configurableJoint.axis, configurableJoint.secondaryAxis), configurableJoint.secondaryAxis);
            set
            {
                var a = Quaternion.Inverse(rigidbody.rotation) * (_angle = value);
                configurableJoint.axis = a * Vector3.right;
                configurableJoint.secondaryAxis = a * Vector3.up;

                connectedAnchor = _connectedAnchor;
            }
        }

        [SerializeField] private bool _revolving = false;
        public bool revolving
        {
            get => configurableJoint.angularXMotion == ConfigurableJointMotion.Free;
            set => configurableJoint.angularXMotion = (_revolving = value) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked;
        }

        [field: SerializeField] public bool useLimits { get; set; }

        [SerializeField] private Limits _limits;

        public float minDistance
        {
            get => _limits.min;
            set
            {
                _limits.min = value;
                maxDistance = _limits.max;
            }
        }
        
        public float maxDistance
        {
            get => _limits.max;
            set
            {
                _limits.max = Mathf.Max(value, minDistance);
                if (!_useLimits)
                {
                    configurableJoint.xMotion = ConfigurableJointMotion.Free;
                }
                else
                {
                    configurableJoint.xMotion = ConfigurableJointMotion.Limited;

                    var linearLimit = configurableJoint.linearLimit;
                    linearLimit.limit = _limit;
                    configurableJoint.linearLimit = linearLimit;
                }

                connectedAnchor = _connectedAnchor;
            }
        }

        public float bounciness
        {
            get => configurableJoint.linearLimit.bounciness;
            set
            {
                var linearLimit = configurableJoint.linearLimit;
                linearLimit.bounciness = _limits.bounciness = Mathf.Clamp01(value);
                configurableJoint.linearLimit = linearLimit;
            }
        }

        public float contactDistance
        {
            get => configurableJoint.linearLimit.contactDistance;
            set
            {
                var linearLimit = configurableJoint.linearLimit;
                linearLimit.contactDistance = _limits.contactDistance = Mathf.Clamp(value, 0f, float.MaxValue);
                configurableJoint.linearLimit = linearLimit;
            }
        }

        [SerializeField] private float _breakForce = float.PositiveInfinity;
        public float breakForce
        {
            get => configurableJoint.breakForce;
            set => configurableJoint.breakForce = _breakForce = Mathf.Max(value, 0.001f);
        }

        [SerializeField] private float _breakTorque = float.PositiveInfinity;
        public float breakTorque
        {
            get => configurableJoint.breakTorque;
            set => configurableJoint.breakTorque = _breakTorque = Mathf.Max(value, 0.001f);
        }

        [SerializeField] private bool _enableCollision;
        public bool enableCollision
        {
            get => configurableJoint.enableCollision;
            set => configurableJoint.enableCollision = _enableCollision = value;
        }

        [SerializeField] private bool _enablePreprocessing = true;
        public bool enablePreprocessing
        {
            get => configurableJoint.enablePreprocessing;
            set => configurableJoint.enablePreprocessing = _enablePreprocessing = value;
        }

        [SerializeField] private float _massScale = 1f;
        public float massScale
        {
            get => configurableJoint.massScale;
            set => configurableJoint.massScale = _massScale = Mathf.Clamp(value, 0.00001f, float.MaxValue);
        }

        [SerializeField] private float _connectedMassScale = 1f;
        public float connectedMassScale
        {
            get => configurableJoint.connectedMassScale;
            set => configurableJoint.connectedMassScale = _connectedMassScale = Mathf.Clamp(value, 0.00001f, float.MaxValue);
        }

        private float _limit => (maxDistance - minDistance) * 0.5f;
        private bool _limitIsFinite => !float.IsNaN(_limit) && !float.IsInfinity(_limit);
        private bool _useLimits => useLimits && _limitIsFinite;
        private Vector3 _connectedAnchorOffset => _useLimits ? rigidbody.rotation * configurableJoint.axis * (_limit + minDistance) : default;

        [Serializable]
        private struct Limits
        {
            public float min;
            public float max;
            public float bounciness;
            public float contactDistance;
        }

        [ContextMenu(nameof(AutoConfigureConnectedAnchor))]
        public void AutoConfigureConnectedAnchor()
        {
            connectedAnchor = transform.TransformPoint(anchor);
            
            if (connectedBody != null)
            {
                connectedAnchor = connectedBody.transform.InverseTransformPoint(connectedAnchor);
            }
        }

        [ContextMenu(nameof(AutoConfigureAngle))]
        public void AutoConfigureAngle()
        {
            var anchorPosition = transform.TransformPoint(anchor);
            var connectedAnchorPosition = connectedBody?.transform.TransformPoint(connectedAnchor) ?? connectedAnchor;

            angle = Quaternion.Inverse(transform.rotation) * (Quaternion.LookRotation(anchorPosition - connectedAnchorPosition) * Quaternion.FromToRotation(Vector3.right, Vector3.forward));
        }

        private void TryJointBreak()
        {
            if (rigidbody.velocity.sqrMagnitude >= breakForce * breakForce
                || rigidbody.angularVelocity.sqrMagnitude >= breakTorque * breakTorque)
            {
                SendMessage("OnJointBreak", rigidbody.velocity.magnitude, SendMessageOptions.DontRequireReceiver);
                Destroy(this);
            }
        }

        #region Unity methods.
        private void FixedUpdate()
        {
            if (rigidbody.IsSleeping())
            {
                return;
            }

            TryJointBreak();
        }

        private void OnDisable()
        {
            enabled = true;
        }

        private void Reset()
        {
            ((IAuthor)this).ResetAuthor();
        }

        private void OnDestroy()
        {
            ((IAuthor)this).DestroyAuthor();
        }
        #endregion

        #region IAuthor methods.
        [field: SerializeField][field: HideInInspector] bool IAuthor.isDeserializing { get; set; }

        void IAuthor.Serialize()
        {
            configurableJoint.autoConfigureConnectedAnchor = false;
            configurableJoint.yMotion = configurableJoint.zMotion = configurableJoint.angularYMotion = configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;

            connectedAnchor = connectedAnchor;

            anchor = anchor;
            angle = angle;
            revolving = revolving;
            useLimits = useLimits;
            maxDistance = maxDistance;
            minDistance = minDistance;
            bounciness = bounciness;
            contactDistance = contactDistance;

            connectedBody = connectedBody;
            //connectedArticulationBody = connectedArticulationBody;
            breakForce = breakForce;
            breakTorque = breakTorque;
            enableCollision = enableCollision;
            enablePreprocessing = enablePreprocessing;
            massScale = massScale;
            connectedMassScale = connectedMassScale;
        }

        void IAuthor.Deserialize()
        {
            configurableJoint.autoConfigureConnectedAnchor = false;
            configurableJoint.yMotion = configurableJoint.zMotion = configurableJoint.angularYMotion = configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;

            anchor = _anchor;
            angle = _angle;
            revolving = _revolving;
            useLimits = _useLimits;
            minDistance = _limits.min;
            bounciness = _limits.bounciness;
            contactDistance = _limits.contactDistance;

            connectedBody = _connectedBody;
            //connectedArticulationBody = _connectedArticulationBody;
            breakForce = _breakForce;
            breakTorque = _breakTorque;
            enableCollision = _enableCollision;
            enablePreprocessing = _enablePreprocessing;
            massScale = _massScale;
            connectedMassScale = _connectedMassScale;
        }

        void IAuthor.DestroyAuthor()
        {
            ExtraObject.DestroyImmediateSafe(_configurableJoint);
        }
        #endregion
    }
}
