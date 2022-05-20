#nullable enable
using UnityEngine;

namespace UnityExtras
{
    public class ExtraGizmos
    {
        public static void DrawWireCapsule(Vector3 center, float radius, float height, Vector3 direction)
        {
            DrawWireCapsule(center - direction * height * 0.5f, center + direction * height * 0.5f, radius);
        }

        public static void DrawWireCapsule(Vector3 start, Vector3 end, float radius)
        {
            var lookRotation = Quaternion.LookRotation(end - start);
            var up = radius * (lookRotation * Vector3.up);
            var right = radius * (lookRotation * Vector3.right);

            Gizmos.DrawWireSphere(start, radius);
            Gizmos.DrawLine(start + up, end + up);
            Gizmos.DrawLine(start - up, end - up);
            Gizmos.DrawLine(start + right, end + right);
            Gizmos.DrawLine(start - right, end - right);
            Gizmos.DrawWireSphere(end, radius);
        }

        public static void DrawArrow(Vector3 from, Vector3 to)
        {
            Gizmos.DrawLine(from, to);

            var angle = ExtraMath.Angle(from - to);
            var distance = (from - to).magnitude * 0.25f;
            var leftCap = to + (Vector3)ExtraMath.Direction(angle - 30f) * distance;
            var rightCap = to + (Vector3)ExtraMath.Direction(angle + 30f) * distance;
            Gizmos.DrawLine(to, leftCap);
            Gizmos.DrawLine(to, rightCap);
        }
    }
}
