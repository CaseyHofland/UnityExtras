#nullable enable
using UnityEngine;
using Unity.Mathematics;

using static UnityEngine.Mathf;
using System.Runtime.CompilerServices;

namespace UnityExtras
{
    /// <include file='./ExtraMath.xml' path='docs/ExtraMath/*'/>
    public class ExtraMath
    {
        /// <include file='./ExtraMath.xml' path='docs/Angle/*'/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(Vector2 direction) => direction != Vector2.right ? Vector2.SignedAngle(Vector2.right, direction) : 0f;

        /// <include file='./ExtraMath.xml' path='docs/Direction/*'/>
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

        /// <include file='./ExtraMath.xml' path='docs/Rotate2D/*'/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Rotate2D(Vector2 x, float degrees) => degrees != 0f ? Direction(Angle(x) + degrees) * x.magnitude : x;

        /// <include file='./ExtraMath.xml' path='docs/JumpVelocity/*'/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 JumpVelocity(float desiredHeight) => JumpVelocity(desiredHeight, Physics.gravity);

        /// <include file='./ExtraMath.xml' path='docs/JumpVelocity/*'/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 JumpVelocity(float desiredHeight, Vector3 gravity) => JumpVelocity(desiredHeight, gravity.normalized, gravity.magnitude);

        /// <include file='./ExtraMath.xml' path='docs/JumpVelocity/*'/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 JumpVelocity(float desiredHeight, Vector3 gravityDirection, float gravityMagnitude) => -gravityDirection * Sqrt(desiredHeight * 2f * gravityMagnitude);

        /// <include file='./ExtraMath.xml' path='docs/FetchRotation2D/*'/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FetchRotation2D(quaternion quaternion)
        {
            const float Rot2Rot2D = 2f * Rad2Deg;

            return Rot2Rot2D * (quaternion.value.w < 0f
                ? math.atan2(-quaternion.value.z, -quaternion.value.w)
                : math.atan2(quaternion.value.z, quaternion.value.w));
        }
    }
}
