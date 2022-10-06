#nullable enable
using UnityEngine;
using UnityEngine.Events;

namespace UnityExtras.Events
{
    /// <summary>Call <see cref="UnityEvent"/> based on a condition.</summary>
    public abstract class Switch : MonoBehaviour
    {
        [SerializeField][Tooltip("If the condition should reverse its \"is on\" status.\n\nExample: if the condition is true, the Switch is off. If it is false, the Switch is on.")] private bool _reverse = false;
        /// <summary>If the condition should reverse its "<see cref="isOn">is on</see>" status.</summary>
        /// <remarks>
        /// <example>Example: if the condition is <see langword="true"/>, the <see cref="Switch"/> is off. If it is <see langword="false"/>, the <see cref="Switch"/> is on.</example>
        /// </remarks>
        public bool reverse
        {
            get => _reverse;
            set
            {
                if (_reverse != value)
                {
                    _reverse = value;
                    isOn = !isOn;
                }
            }
        }

        private bool _isOn;
        /// <summary>If the <see cref="Switch"/> is on or not. Inverting the value will invoke the corresponding <see cref="UnityEvent"/>.</summary>
        public bool isOn
        {
            get => _isOn;
            private set
            {
                if (_isOn != value)
                {
                    if (value)
                    {
                        on.Invoke();
                    }
                    else
                    {
                        off.Invoke();
                    }
                }

                _isOn = value;
            }
        }

        [field: SerializeField][field: Tooltip("Called when the switch is turned on.")] public UnityEvent on { get; set; } = new();
        [field: SerializeField][field: Tooltip("Called when the switch is turned off.")] public UnityEvent off { get; set; } = new();

        protected virtual void OnEnable()
        {
            _isOn = reverse ? !Condition() : Condition();
            if (_isOn)
            {
                on.Invoke();
            }
            else
            {
                off.Invoke();
            }
        }

        protected virtual void Update()
        {
            isOn = reverse ? !Condition() : Condition();
        }

        protected abstract bool Condition();
    }
}