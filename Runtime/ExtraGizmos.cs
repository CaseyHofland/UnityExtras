#nullable enable
using UnityEngine;

using static UnityEngine.Gizmos;

namespace UnityExtras
{
    /// <include file='./ExtraGizmos.xml' path='docs/ExtraGizmos/*'/>
    public class ExtraGizmos
    {
        /// <include file='./ExtraGizmos.xml' path='docs/DrawWireCapsule/*'/>
        public static void DrawWireCapsule(Vector3 center, float radius, float height, Vector3 direction)
        {
            DrawWireCapsule(center - 0.5f * height * direction, center + 0.5f * height * direction, radius);
        }

        /// <include file='./ExtraGizmos.xml' path='docs/DrawWireCapsule/*'/>
        public static void DrawWireCapsule(Vector3 start, Vector3 end, float radius)
        {
            var lookRotation = Quaternion.LookRotation(end - start);
            var up = radius * (lookRotation * Vector3.up);
            var right = radius * (lookRotation * Vector3.right);

            DrawWireSphere(start, radius);
            DrawLine(start + up, end + up);
            DrawLine(start - up, end - up);
            DrawLine(start + right, end + right);
            DrawLine(start - right, end - right);
            DrawWireSphere(end, radius);
        }

        public static void DrawArrow(Vector3 from, Vector3 to)
        {
            DrawLine(from, to);

            var angle = ExtraMath.Angle(from - to);
            var distance = (from - to).magnitude * 0.25f;
            var leftCap = to + (Vector3)ExtraMath.Direction(angle - 30f) * distance;
            var rightCap = to + (Vector3)ExtraMath.Direction(angle + 30f) * distance;
            DrawLine(to, leftCap);
            DrawLine(to, rightCap);
        }
    }
}
