using UnityEngine;
using UnityEngine.Events;

namespace UnityExtras.Events
{
    /// <summary>Call <see cref="UnityEvent"/> based on its <see cref="condition">condition</see>.</summary>
    public abstract class ConditionalSwitch : Switch
    {
        /// <summary>If the switch should invert based on whether the <see cref="condition">condition</see> is met.</summary>
        [field: SerializeField, Tooltip("Invert the switch, so that it is on by default.")] public bool invertSwitch { get; set; }

        /// <summary>The condition to determine if the <see cref="Switch"/> <see cref="isOn">is on</see>.</summary>
        public abstract bool condition { get; }

        /// <inheritdoc cref="Switch.isOn"/>
        public new bool isOn => condition ^ invertSwitch;

        protected virtual void Update()
        {
            base.isOn = isOn;
        }
    }
}
