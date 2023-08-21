#nullable enable
using UnityEngine;
using UnityEngine.Events;

namespace UnityExtras.Events
{
    /// <summary>Call <see cref="UnityEvent"/> based on a condition.</summary>
    public abstract class Switch : MonoBehaviour
    {
        public enum StartBehaviour
        {
            Nothing,
            Invoke,
            InvokeBoth
        }

        [field: SerializeField, Tooltip("The invoke behaviour of the switch at start.")] public StartBehaviour startBehaviour { get; set; }

        //[field: SerializeField, Tooltip("Use a single toggle switch instead of a separate on/off switch.")] public bool useToggleSwitch { get; set; }

        /// <summary>
        /// Invoked when the switch is turned on.
        /// </summary>
        [field: SerializeField, Tooltip("Called when the switch is turned on.")] public UnityEvent on { get; set; } = new();

        /// <summary>
        /// Invoked when the switch is turned off.
        /// </summary>
        [field: SerializeField, Tooltip("Called when the switch is turned off.")] public UnityEvent off { get; set; } = new();
        //[field: SerializeField, Tooltip("Called when the switch is toggled.")] public UnityEvent<bool> toggle { get; set; } = new();
        [SerializeField, Tooltip("Invert the switch, so that it is on by default."), LinkProperty(nameof(invertSwitch))] private bool _invertSwitch = false;

        protected bool _isOn;

        /// <summary>If the switch should invert when it <see cref="isOn">is on</see>.</summary>
        public bool invertSwitch
        {
            get => _invertSwitch;
            set
            {
                if (_invertSwitch == value)
                {
                    return;
                }

                InvokeSwitch(_isOn ^ (_invertSwitch = value));
            }
        }

        /// <summary>If the switch is on or not.</summary>
        public bool isOn
        {
            get => _isOn ^ invertSwitch;
            set
            {
                if (_isOn == value ^ invertSwitch)
                {
                    return;
                }

                InvokeSwitch(_isOn = value ^ invertSwitch);
            }
        }

        protected void InvokeSwitch(bool on)
        {
            if (on)
            {
                this.on.Invoke();
            }
            else
            {
                off.Invoke();
            }
        }

        protected virtual void OnEnable()
        {
            switch (startBehaviour)
            {
                case StartBehaviour.InvokeBoth:
                    if (isOn)
                    {
                        off.Invoke();
                        on.Invoke();
                    }
                    else
                    {
                        on.Invoke();
                        off.Invoke();
                    }
                    break;
                case StartBehaviour.Invoke:
                    InvokeSwitch(isOn);
                    break;
            }
        }
    }
}