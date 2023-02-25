#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
    [Serializable]
    public class TriggerParameter : IEquatable<TriggerParameter>
    {
        [field: SerializeField] public bool hideSettings { get; private set; }

        public AnimatorHash parameterName;

        public TriggerParameter(string parameterName, bool hideSettings = false)
        {
            this.hideSettings = hideSettings;

            this.parameterName = parameterName;
        }

        public override bool Equals(object? obj) => obj is TriggerParameter parameter && Equals(parameter);
        public bool Equals(TriggerParameter other) => parameterName.Equals(other.parameterName);
        public override int GetHashCode() => HashCode.Combine(parameterName);
        public static bool operator ==(TriggerParameter left, TriggerParameter right) => left.Equals(right);
        public static bool operator !=(TriggerParameter left, TriggerParameter right) => !(left == right);

        public override string ToString() => $"{parameterName} trigger";
    }
}
