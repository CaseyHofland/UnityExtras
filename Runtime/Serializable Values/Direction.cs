#nullable enable
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityExtras
{
    [Serializable]
    public record Direction
    {
        [SerializeField] private Vector3 _value = Vector3.forward;
        public Vector3 value
        {
            get => _value;
            set => _value = value.magnitude > Vector3.kEpsilon ? value.normalized : Vector3.forward;
        }

        public Quaternion rotation
        {
            get => Quaternion.LookRotation(value);
            set => this.value = value * Vector3.forward;
        }

        public override string ToString() => value.ToString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Direction operator -(Direction direction) => new() { value = -direction.value };

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Vector3(Direction direction) => direction.value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Direction(Vector3 value) => new() { value = value };
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator *(Direction a, float d) => a.value * d;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator *(float d, Direction a) => d * a.value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator /(Direction a, float d) => a.value / d;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool operator ==(Direction lhs, Vector3 rhs) => lhs.value == rhs;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool operator !=(Direction lhs, Vector3 rhs) => !(lhs == rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Quaternion(Direction direction) => direction.rotation;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Direction(Quaternion rotation) => new() { rotation = rotation };
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Quaternion operator *(Direction direction, Quaternion rotation) => direction.rotation * rotation;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Quaternion operator *(Quaternion rotation, Direction direction) => rotation * direction.rotation;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator *(Direction direction, Vector3 point) => direction.rotation * point;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool operator ==(Direction lhs, Quaternion rhs) => lhs.rotation == rhs;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool operator !=(Direction lhs, Quaternion rhs) => !(lhs == rhs);
    }
}