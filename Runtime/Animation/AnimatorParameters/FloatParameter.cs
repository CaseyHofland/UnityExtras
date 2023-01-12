#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
    [Serializable]
    public struct FloatParameter : IEquatable<FloatParameter>
    {
        [field: SerializeField] public bool hideSettings { get; private set; }
        public event Action? onValueChanged;

        public AnimatorHash parameterName;
        [SerializeField, LinkProperty(nameof(value))] private float _value;
        [SerializeField, LinkProperty(nameof(dampTime))] private float _dampTime;
        [SerializeField, LinkProperty(nameof(hasMinMax))] private bool _hasMinMax;
        [SerializeField, LinkProperty(nameof(min))] private float _min;
        [SerializeField, LinkProperty(nameof(max))] private float _max;

        public float value
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

        public float dampTime
        {
            get => _dampTime;
            set => _dampTime = Mathf.Max(0f, value);
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

        public float min
        {
            get => _min;
            set
            {
                _min = value;
                max = max;
            }
        }

        public float max
        {
            get => _max;
            set
            {
                _max = Mathf.Max(min, value);
                this.value = this.value;
            }
        }

        public FloatParameter(string parameterName, bool hideSettings = false) : this(parameterName, default, hideSettings) { }
        public FloatParameter(string parameterName, float dampTime, bool hideSettings = false) : this(parameterName, dampTime, default, default, hideSettings) { }
        public FloatParameter(string parameterName, float min, float max, bool hideSettings = false) : this(parameterName, default, min, max, hideSettings) { }
        public FloatParameter(string parameterName, float dampTime, float min, float max, bool hideSettings = false)
        {
            this.hideSettings = hideSettings;
            this.onValueChanged = default;

            this.parameterName = parameterName;
            _value = default;
            _dampTime = dampTime;
            _min = min;
            _max = max;

            if (_hasMinMax = min != default || max != default)
            {
                this.min = this.min;
            }
        }

        public override bool Equals(object? obj) => obj is FloatParameter parameter && Equals(parameter);
        public bool Equals(FloatParameter other)
        {
            return parameterName.Equals(other.parameterName) &&
                   _value == other._value &&
                   _dampTime == other._dampTime &&
                   _hasMinMax == other._hasMinMax &&
                   _min == other._min &&
                   _max == other._max;
        }

        public override int GetHashCode() => HashCode.Combine(parameterName, _value, _dampTime, _hasMinMax, _min, _max);
        public static bool operator ==(FloatParameter left, FloatParameter right) => left.Equals(right);
        public static bool operator !=(FloatParameter left, FloatParameter right) => !(left == right);
    }
}
