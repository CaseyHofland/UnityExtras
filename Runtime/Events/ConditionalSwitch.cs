using UnityEngine.Events;

namespace UnityExtras.Events
{
    /// <summary>Call <see cref="UnityEvent"/> based on its <see cref="condition">condition</see>.</summary>
    public abstract class ConditionalSwitch : Switch
    {
        /// <summary>The condition to determine if the <see cref="Switch"/> <see cref="isOn">is on</see>.</summary>
        public abstract bool condition { get; }

        /// <inheritdoc cref="Switch.isOn"/>
        public new bool isOn => condition ^ invertSwitch;

        protected override void OnEnable()
        {
            base._isOn = condition;
            base.OnEnable();
        }

        protected virtual void Update()
        {
            base.isOn = isOn;
        }
    }
}
