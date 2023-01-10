#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
    [Serializable]
    public struct IntParameter : IEquatable<IntParameter>
    {
        [field: SerializeField] public bool hideSettings { get; private set; }
        public event Action? onValueChanged;

        public AnimatorHash parameterName;
        [SerializeField, LinkProperty(nameof(value))] private int _value;
        [SerializeField, LinkProperty(nameof(hasMinMax))] private bool _hasMinMax;
        [SerializeField, LinkProperty(nameof(min))] private int _min;
        [SerializeField, LinkProperty(nameof(max))] private int _max;

        public int value
        {
            get => _value;
            set
            {
                var oldValue = _value;
                _value = hasMinMax ? Mathf.Clamp(value, min, max) : value;
                if (oldValue != _value)
                {
                    onValueChanged?.Invoke();
                }
            }
        }

        public bool hasMinMax
        {
            get => _hasMinMax;
            set
            {
                if (_hasMinMax = value)
                {
                    this.value = this.value;
                }
            }
        }

        public int min
        {
            get => _min;
            set
            {
                _min = value;
                max = max;
            }
        }

        public int max
        {
            get => _max;
            set
            {
                _max = Mathf.Max(min, value);
                this.value = this.value;
            }
        }

        public IntParameter(string parameterName, bool hideSettings = false) : this(parameterName, default, default, hideSettings) { }
        public IntParameter(string parameterName, int min, int max, bool hideSettings = false)
        {
            this.hideSettings = hideSettings;
            this.onValueChanged = default;

            this.parameterName = parameterName;
            _value = default;
            _min = min;
            _max = max;

            if (_hasMinMax = min != default || max != default)
            {
                this.min = this.min;
            }
        }

        public override bool Equals(object? obj) => obj is IntParameter parameter && Equals(parameter);
        public bool Equals(IntParameter other)
        {
            return parameterName.Equals(other.parameterName) &&
                   _value == other._value &&
                   _hasMinMax == other._hasMinMax &&
                   _min == other._min &&
                   _max == other._max;
        }

        public override int GetHashCode() => HashCode.Combine(parameterName, _value, _hasMinMax, _min, _max);
        public static bool operator ==(IntParameter left, IntParameter right) => left.Equals(right);
        public static bool operator !=(IntParameter left, IntParameter right) => !(left == right);
    }
}
