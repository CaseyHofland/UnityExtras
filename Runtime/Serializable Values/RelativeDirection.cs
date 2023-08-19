#nullable enable
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityExtras
{
    /// <summary>
    /// Specifies a direction relative to a pivot.
    /// </summary>
    [Serializable]
    public struct RelativeDirection : IEquatable<RelativeDirection>
    {
        public Transform? pivot;
        public Direction direction;

        private Quaternion pivotRotation => pivot == null ? Quaternion.identity : pivot.rotation;

        public Vector3 value => pivotRotation * direction.value;
        public Quaternion rotation => direction.rotation * pivotRotation;

        public override string ToString() => value.ToString();

        public override bool Equals(object? obj) => obj is RelativeDirection relativeDirection && Equals(relativeDirection);
        public bool Equals(RelativeDirection other) => value == other.value;
        public override int GetHashCode() => HashCode.Combine(pivot, direction);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static RelativeDirection operator -(RelativeDirection relativeDirection) => new() { pivot = relativeDirection.pivot, direction = -relativeDirection.direction };
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool operator ==(RelativeDirection lhs, RelativeDirection rhs) => lhs.Equals(rhs);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool operator !=(RelativeDirection lhs, RelativeDirection rhs) => !(lhs == rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Direction(RelativeDirection relativeDirection) => relativeDirection.value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Vector3(RelativeDirection relativeDirection) => relativeDirection.value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator *(RelativeDirection a, float d) => a.value * d;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator *(float d, RelativeDirection a) => d * a.value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 operator /(RelativeDirection a, float d) => a.value / d;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool operator ==(RelativeDirection lhs, Vector3 rhs) => lhs.value.Equals(rhs);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool operator !=(RelativeDirection lhs, Vector3 rhs) => !(lhs == rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator Quaternion(RelativeDirection relativeDirection) => relativeDirection.rotation;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool operator ==(RelativeDirection lhs, Quaternion rhs) => lhs.rotation.Equals(rhs);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool operator !=(RelativeDirection lhs, Quaternion rhs) => !(lhs == rhs);
    }
}
