#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityExtras
{
    [CreateAssetMenu(fileName = nameof(ScriptableAnimationEvent), menuName = nameof(ScriptableAnimationEvent) + "/Base" + nameof(ScriptableAnimationEvent))]
    public class ScriptableAnimationEvent : ScriptableObject, IEnumerable<ScriptableAnimationEvent>
    {
        public List<ScriptableAnimationEvent?> childEvents = new();

        public virtual void Play(ScriptableAnimationEventListener listener, AnimationEvent animationEvent)
        {
            foreach (var scriptableAnimationEvent in childEvents)
            {
                if (scriptableAnimationEvent != null)
                {
                    scriptableAnimationEvent.Play(listener, animationEvent);
                }
            }
        }

        private void OnValidate()
        {
            foreach (var childEvent in childEvents)
            {
                if (childEvent == null)
                {
                    continue;
                }

                if (childEvent == this
                    || childEvent.Contains(this))
                {
                    throw new CircularReferenceException($"{this} has a circular reference. Be aware that playing this event will cause a {nameof(StackOverflowException)}.");
                }
            }
        }

        private IEnumerable<ScriptableAnimationEvent> ConcatSafe(IEnumerable<ScriptableAnimationEvent> enumerable, ScriptableAnimationEvent @event)
        {
            enumerable = enumerable.Append(@event);
            foreach (var childEvent in childEvents)
            {
                if (childEvent == null
                    || enumerable.Contains(childEvent))
                {
                    continue;
                }

                enumerable = ConcatSafe(enumerable, childEvent);
            }
            return enumerable;
        }

        public IEnumerator<ScriptableAnimationEvent> GetEnumerator() => ConcatSafe(Enumerable.Empty<ScriptableAnimationEvent>(), this).Skip(1).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
