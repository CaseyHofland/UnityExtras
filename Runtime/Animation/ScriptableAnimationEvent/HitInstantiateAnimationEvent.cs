#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtras
{
    [CreateAssetMenu(fileName = nameof(HitInstantiateAnimationEvent), menuName = nameof(ScriptableAnimationEvent) + "/" + nameof(HitInstantiateAnimationEvent))]
    public class HitInstantiateAnimationEvent : ScriptableAnimationEvent, IDataReceiver<RaycastHit>, IDataReceiver<Texture?>
    {
        [Serializable]
        public struct Wrapper
        {
            public Texture texture;
            public GameObject prefab;
        }

        public List<Wrapper> texturePrefabs = new();
        public GameObject? prefab;

        RaycastHit IDataReceiver<RaycastHit>.value { set => hit = value; }
        Texture? IDataReceiver<Texture?>.value { set => hitTexture = value; }

        private RaycastHit hit;
        private Texture? hitTexture;

        public override void Play(ScriptableAnimationEventListener listener, AnimationEvent animationEvent)
        {
            if (hit.collider != null)
            {
                var wrapperIndex = texturePrefabs.FindIndex(wrapper => wrapper.texture == hitTexture);
                var instance = Instantiate(wrapperIndex == -1 ? prefab : texturePrefabs[wrapperIndex].prefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            }

            base.Play(listener, animationEvent);
        }
    }
}