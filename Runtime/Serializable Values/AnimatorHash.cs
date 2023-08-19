#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
    /// <summary>
    /// A generated parameter id from a string.
    /// </summary>
    [Serializable]
    public struct AnimatorHash : ISerializationCallbackReceiver, IEquatable<AnimatorHash>
    {
        [SerializeField] private string? _name;

        public string? name
        {
            get => _name;
            set => id = Animator.StringToHash(_name = value);
        }

        public int id { get; private set; }

        public AnimatorHash(string name)
        {
            id = Animator.StringToHash(_name = name);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            id = Animator.StringToHash(name);
        }

        public override bool Equals(object? obj) => obj is AnimatorHash hash && Equals(hash);
        public bool Equals(AnimatorHash other) => _name == other._name && id == other.id;
        public override int GetHashCode() => HashCode.Combine(_name, id);
        public static bool operator ==(AnimatorHash left, AnimatorHash right) => left.Equals(right);
        public static bool operator !=(AnimatorHash left, AnimatorHash right) => !(left == right);

        public override string ToString() => name ?? string.Empty;

        public static implicit operator int(AnimatorHash animatorHash) => animatorHash.id;
        public static implicit operator AnimatorHash(string name) => new(name);
    }
}
