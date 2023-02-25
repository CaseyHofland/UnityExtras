#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
    [Serializable]
    public class BoolParameter : IEquatable<BoolParameter>
    {
        [field: SerializeField] public bool hideSettings { get; private set; }
        public event Action<BoolParameter>? onValueChanged;

        public AnimatorHash parameterName;
        [SerializeField] private bool _value;

        public bool value
        {
            get => _value;
            set
            {
                _value = value;
                onValueChanged?.Invoke(this);
            }
        }

        public BoolParameter(string parameterName, bool hideSettings = false)
        {
            this.hideSettings = hideSettings;
            onValueChanged = default;

            this.parameterName = parameterName;
            _value = default;
        }

        public override bool Equals(object? obj) => obj is BoolParameter parameter && Equals(parameter);
        public bool Equals(BoolParameter other) => parameterName.Equals(other.parameterName) && _value == other._value;
        public override int GetHashCode() => HashCode.Combine(parameterName, _value);
        public static bool operator ==(BoolParameter left, BoolParameter right) => left.Equals(right);
        public static bool operator !=(BoolParameter left, BoolParameter right) => !(left == right);
    }
}
