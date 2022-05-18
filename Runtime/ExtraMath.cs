#nullable enable
using UnityEngine;
using Unity.Mathematics;

using static UnityEngine.Mathf;

namespace UnityExtras
{
    public class ExtraMath
    {
        public static float InverseSafe(float x, float defaultValue = default) => x == 0f ? defaultValue : 1f / x;
        public static float2 InverseSafe(float2 x, float2 defaultValue = default) => new float2(InverseSafe(x.x, defaultValue.x), InverseSafe(x.y, defaultValue.y));
        public static float3 InverseSafe(float3 x, float3 defaultValue = default) => new float3(InverseSafe(x.x, defaultValue.x), InverseSafe(x.yz, defaultValue.yz));
        public static float4 InverseSafe(float4 x, float4 defaultValue = default) => new float4(InverseSafe(x.x, defaultValue.x), InverseSafe(x.yzw, defaultValue.yzw));

        public static float Angle(Vector2 direction) => direction != Vector2.right ? Vector2.SignedAngle(Vector2.right, direction) : 0f;
        public static Vector2 Direction(float degrees)
        {
            if (degrees == 0f)
            {
                return Vector2.right;
            }

            var radians = degrees * Deg2Rad;
            return new Vector2(Cos(radians), Sin(radians));

        }
        public static Vector2 Rotate2D(Vector2 x, float degrees) => degrees != 0f ? Direction(Angle(x.normalized) + degrees) * x.magnitude : x;

        public static Vector3 JumpVelocity(float desiredHeight) => JumpVelocity(desiredHeight, Physics.gravity);
        public static Vector3 JumpVelocity(float desiredHeight, Vector3 gravity) => JumpVelocity(desiredHeight, gravity.normalized, gravity.magnitude);
        public static Vector3 JumpVelocity(float desiredHeight, Vector3 gravityDirection, float gravityMagnitude) => -gravityDirection * Sqrt(desiredHeight * 2f * gravityMagnitude);
    }
}
