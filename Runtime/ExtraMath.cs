#nullable enable
using UnityEngine;
using Unity.Mathematics;

using static UnityEngine.Mathf;
using static Unity.Mathematics.math;
using System.Runtime.CompilerServices;

namespace UnityExtras
{
    public class ExtraMath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(Vector2 direction) => direction != Vector2.right ? Vector2.SignedAngle(Vector2.right, direction) : 0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Direction(float degrees)
        {
            if (degrees == 0f)
            {
                return Vector2.right;
            }

            var radians = degrees * Deg2Rad;
            return new Vector2(Cos(radians), Sin(radians));

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Rotate2D(Vector2 x, float degrees) => degrees != 0f ? Direction(Angle(x.normalized) + degrees) * x.magnitude : x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 JumpVelocity(float desiredHeight) => JumpVelocity(desiredHeight, Physics.gravity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 JumpVelocity(float desiredHeight, Vector3 gravity) => JumpVelocity(desiredHeight, gravity.normalized, gravity.magnitude);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 JumpVelocity(float desiredHeight, Vector3 gravityDirection, float gravityMagnitude) => -gravityDirection * Sqrt(desiredHeight * 2f * gravityMagnitude);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FetchRotation2D(quaternion quaternion)
        {
            // NOTE: the code that's commented out is an optimization we can't use because it doesn't simulate Unity's own behavior in special cases.

            //var eulerAngles = quaternion.eulerAngles;
            //if (Mathf.Abs(eulerAngles.x) < Vector2.kEpsilon || Mathf.Abs(eulerAngles.y) < Vector2.kEpsilon)
            //{
            //    return (Mathf.Sign(quaternion.z) * Mathf.Sign(quaternion.w) == -1f)
            //        ? eulerAngles.z - 360f
            //        : eulerAngles.z;
            //}

            const float Rot2Rot2D = 2f * Rad2Deg;

            return Rot2Rot2D * (quaternion.value.w < 0f
                ? atan2(-quaternion.value.z, -quaternion.value.w)
                : atan2(quaternion.value.z, quaternion.value.w));
        }
    }
}
