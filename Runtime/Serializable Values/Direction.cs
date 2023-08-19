#nullable enable
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityExtras
{
    [Serializable]
    public struct Direction : IEquatable<Direction>
    {
        [SerializeField] private Vector3 _value;
        public Vector3 value
        {
            get => _value;
            set => _value = value.sqrMagnitude > Vector3.kEpsilonNormalSqrt ? value.normalized : Vector3.forward;
        }

        public Quaternion rotation
        {
            get => Quaternion.LookRotation(value);
            set => this.value = value * Vector3.forward;
        }

        public override string ToString() => value.ToString();

        public override bool Equals(object? obj) => obj is Direction direction && Equals(direction);
        public bool Equals(Direction other) => _value == other._value;
        public override int GetHashCode() => _value.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Direction operator -(Direction direction) => new() { value = -direction.value };
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool operator ==(Direction lhs, Direction rhs) => lhs.Equals(rhs);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool operator !=(Direction lhs, Direction rhs) => !(lhs == rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Vector3(Direction direction) => direction.value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Direction(Vector3 value) => new() { value = value };
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator *(Direction a, float d) => a.value * d;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator *(float d, Direction a) => d * a.value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator /(Direction a, float d) => a.value / d;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool operator ==(Direction lhs, Vector3 rhs) => lhs.value.Equals(rhs);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool operator !=(Direction lhs, Vector3 rhs) => !(lhs == rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Quaternion(Direction direction) => direction.rotation;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Direction(Quaternion rotation) => new() { rotation = rotation };
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Quaternion operator *(Direction direction, Quaternion rotation) => direction.rotation * rotation;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Quaternion operator *(Quaternion rotation, Direction direction) => rotation * direction.rotation;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator *(Direction direction, Vector3 point) => direction.rotation * point;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool operator ==(Direction lhs, Quaternion rhs) => lhs.rotation.Equals(rhs);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool operator !=(Direction lhs, Quaternion rhs) => !(lhs == rhs);
    }
}