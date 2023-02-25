#nullable enable
using UnityEngine;

namespace UnityExtras
{
    [RequireComponent(typeof(Animator))]
    public class ScriptableAnimationEventListener : MonoBehaviour
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [field: SerializeField, HideInInspector] public Animator animator { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private void Reset()
        {
            animator = GetComponent<Animator>();
        }

        private void Awake()
        {
            Reset();
        }

        public void Play(ScriptableAnimationEvent @event)
        {
            @event.Play(this);
        }
    }
}
