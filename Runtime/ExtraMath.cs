#nullable enable
using UnityEngine;
using Unity.Mathematics;

namespace UnityExtras
{
    public class ExtraMath
    {
        public static float InverseSafe(float x, float defaultValue = default) => x == 0f ? defaultValue : 1f / x;
        public static float2 InverseSafe(float2 x, float2 defaultValue = default) => new float2(InverseSafe(x.x, defaultValue.x), InverseSafe(x.y, defaultValue.y));
        public static float3 InverseSafe(float3 x, float3 defaultValue = default) => new float3(InverseSafe(x.x, defaultValue.x), InverseSafe(x.yz, defaultValue.yz));
        public static float4 InverseSafe(float4 x, float4 defaultValue = default) => new float4(InverseSafe(x.x, defaultValue.x), InverseSafe(x.yzw, defaultValue.yzw));

        public static Vector2 RadianToVector2(float radian) => new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        public static Vector2 DegreeToVector2(float degree) => RadianToVector2(degree * Mathf.Deg2Rad);

        public static Vector3 JumpVelocity(float desiredHeight) => JumpVelocity(desiredHeight, Physics.gravity);
        public static Vector3 JumpVelocity(float desiredHeight, Vector3 gravity) => JumpVelocity(desiredHeight, gravity.normalized, gravity.magnitude);
        public static Vector3 JumpVelocity(float desiredHeight, Vector3 gravityDirection, float gravityMagnitude) => -gravityDirection * Mathf.Sqrt(desiredHeight * 2f * gravityMagnitude);
    }
}
