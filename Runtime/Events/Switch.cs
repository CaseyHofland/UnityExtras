#nullable enable
using UnityEngine;
using UnityEngine.Events;

namespace UnityExtras.Events
{
    /// <summary>Call <see cref="UnityEvent"/> based on a condition.</summary>
    public abstract class Switch : MonoBehaviour
    {
        /// <summary>
        /// Invoked when the switch is turned on.
        /// </summary>
        [field: SerializeField, Tooltip("Called when the switch is turned on.")] public UnityEvent on { get; set; } = new();

        /// <summary>
        /// Invoked when the switch is turned off.
        /// </summary>
        [field: SerializeField, Tooltip("Called when the switch is turned off.")] public UnityEvent off { get; set; } = new();

        protected bool _isOn;

        /// <summary>If the switch is on or not.</summary>
        public bool isOn
        {
            get => _isOn;
            set
            {
                if (_isOn == value)
                {
                    return;
                }

                _isOn = value;
                if (isOn)
                {
                    on.Invoke();
                }
                else
                {
                    off.Invoke();
                }
            }
        }
    }
}