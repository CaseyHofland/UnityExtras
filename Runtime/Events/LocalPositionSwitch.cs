#nullable enable
using UnityEngine;

namespace UnityExtras
{
    public class LocalPositionSwitch : Switch
    {
        [field: SerializeField][field: Tooltip("Meets the condition when the local position equals the target.")] public Vector3 targetLocalPosition;
        [field: SerializeField][field: Tooltip("A safety margin for meeting the target.")][field: Min(0f)] public float safetyMargin = Vector3.kEpsilon;

        protected override bool Condition()
        {
            return (transform.localPosition - targetLocalPosition).sqrMagnitude <= safetyMargin * safetyMargin;
        }
    }
}