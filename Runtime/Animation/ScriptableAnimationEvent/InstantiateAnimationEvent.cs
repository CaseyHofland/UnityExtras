#nullable enable
using UnityEngine;

namespace UnityExtras
{
    [CreateAssetMenu(fileName = nameof(InstantiateAnimationEvent), menuName = nameof(ScriptableAnimationEvent) + "/" + nameof(InstantiateAnimationEvent))]
    public class InstantiateAnimationEvent : ScriptableAnimationEvent
    {
        public GameObject? prefab;
        public Vector3 rootOffset;
        public Quaternion rootOrbit = Quaternion.identity;
        public bool asChild;

        public override void Play(ScriptableAnimationEventListener listener, AnimationEvent animationEvent)
        {
            var instance = Instantiate(prefab, listener.transform.position + rootOffset, listener.transform.rotation * rootOrbit, asChild ? listener.transform : null);
            base.Play(listener, animationEvent);
        }
    }
}