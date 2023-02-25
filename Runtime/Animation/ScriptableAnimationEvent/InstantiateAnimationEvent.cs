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
        [Min(0f)] public float destroyDelay;

        public override void Play(ScriptableAnimationEventListener listener)
        {
            var instance = Instantiate(prefab, listener.transform.position + rootOffset, listener.transform.rotation * rootOrbit, asChild ? listener.transform : null);
            if (destroyDelay > 0f)
            {
                Destroy(instance, destroyDelay);
            }
            base.Play(listener);
        }
    }
}