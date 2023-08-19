#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
    [Serializable]
    public struct ShaderId : ISerializationCallbackReceiver, IEquatable<ShaderId>
    {
        [SerializeField] private string? _name;

        public string? name
        {
            get => _name;
            set => id = Shader.PropertyToID(_name = value);
        }

        public int id { get; private set; }

        public ShaderId(string name)
        {
            id = Shader.PropertyToID(_name = name);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            id = Shader.PropertyToID(name);
        }

        public override bool Equals(object? obj) => obj is ShaderId id && Equals(id);
        public bool Equals(ShaderId other) => _name == other._name && id == other.id;
        public override int GetHashCode() => HashCode.Combine(_name, id);
        public static bool operator ==(ShaderId left, ShaderId right) => left.Equals(right);
        public static bool operator !=(ShaderId left, ShaderId right) => !(left == right);

        public override string ToString() => name ?? string.Empty;

        public static implicit operator int(ShaderId id) => id.id;
        public static implicit operator ShaderId(string name) => new(name);
    }
}
