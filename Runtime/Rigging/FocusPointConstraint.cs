#nullable enable
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

namespace UnityExtras.Rigging
{
    [DisallowMultipleComponent]
    public class FocusPointConstraint : MonoBehaviour
    {
        [field: SerializeField, Tooltip("The focus points to use when constraining the object. The highest priority focus point will be used as the constraint, so long as the required head turn doesn't exceed the max Y Angle")] public FocusPoints? focusPoints { get; set; }
        [field: SerializeField, Range(0f, 180f), Tooltip("The maximum allowed head turn angle before the focus point is discarded.")] private float _maxYAngle = 110f;
        [field: SerializeField, Min(0f), Tooltip("How long it takes for the focus point constraint to turn on or off.")] private float _influenceTransitionTime = 1f;
        [field: SerializeField, Tooltip("An influence callback that fires everytime this constraints influence changes, as is the case when going from no focus points to one or from one focus point to none. This can be coupled with e.g. Rig.weight to make a rig turn on or off, allowing the Focus Feature to be disabled when there's nothing to focus on.")] public UnityEvent<float> onInfluenceChanged { get; set; } = new();
        [field: SerializeField, Header("Fallback"), Tooltip("The fallback position source. This will ne used as the constrained in case no valid focus point can be selected.")] public Transform? fallbackSource { get; set; }
        [field: SerializeField, Tooltip("The fallback offset added on top of the fallback position. A value for z is recommended to make the fallback stay a little in front of the fallback source.")] public Vector3 fallbackOffset { get; set; }

        public float influenceTransitionTime
        {
            get => _influenceTransitionTime;
            set => _influenceTransitionTime = Mathf.Max(value, 0f);
        }

        public float maxYAngle
        {
            get => _maxYAngle;
            set => _maxYAngle = Mathf.Clamp(0f, 180f, value);
        }

        private Transform _parentRig;
        private float _currentInfluenceTransitionTime;

        private void Start()
        {
            _parentRig = GetComponentInParent<Rig>().transform;
            onInfluenceChanged.Invoke(Mathf.InverseLerp(0f, influenceTransitionTime, _currentInfluenceTransitionTime));
        }

        private void Update()
        {
            var focusPoint = FindHighestPriorityFocusPoint();
            bool hasInfluenceChanged;

            if (focusPoint != null)
            {
                transform.position = focusPoint.position;
                if (hasInfluenceChanged = _currentInfluenceTransitionTime < influenceTransitionTime)
                {
                    _currentInfluenceTransitionTime = Mathf.Min(_currentInfluenceTransitionTime + Time.deltaTime, influenceTransitionTime);
                }
            }
            else
            {
                transform.position = (fallbackSource != null ? fallbackSource.position : default) + _parentRig.rotation * fallbackOffset;
                if (hasInfluenceChanged = _currentInfluenceTransitionTime > 0f)
                {
                    _currentInfluenceTransitionTime = Mathf.Max(_currentInfluenceTransitionTime - Time.deltaTime, 0f);
                }
            }

            if (hasInfluenceChanged)
            {
                onInfluenceChanged.Invoke(Mathf.InverseLerp(0f, influenceTransitionTime, _currentInfluenceTransitionTime));
            }
        }

        private Transform? FindHighestPriorityFocusPoint()
        {
            if (focusPoints == null)
            {
                return null;
            }

            foreach (var focusPointByPriority in focusPoints.focusPointsByPriorities)
            {
                foreach (var focusPoint in focusPointByPriority.Value)
                {
                    var lookAtDirection = (focusPoint.position - _parentRig.position);
                    lookAtDirection.y = 0;
                    lookAtDirection = lookAtDirection.normalized;
                    var adjustmentRotation = Quaternion.FromToRotation(_parentRig.forward, lookAtDirection);

                    var adjustmentAngle = adjustmentRotation.eulerAngles.y;
                    if (adjustmentAngle <= maxYAngle || adjustmentAngle >= 360f - maxYAngle)
                    {
                        return focusPoint;
                    }
                }
            }

            return null;
        }
    }
}