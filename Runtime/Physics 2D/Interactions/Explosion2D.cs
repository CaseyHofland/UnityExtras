#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtras
{
    public class Explosion2D : MonoBehaviour
    {
        [field: SerializeField] public float force { get; set; } = 10f;
        [field: SerializeField] public float radius { get; set; } = 5f;
        [field: SerializeField] public LayerMask explosionMask { get; set; } = Physics2D.AllLayers;
        [field: SerializeField] public float upModifier { get; set; } = 1f;
        [field: SerializeField] public ForceMode2D mode { get; set; } = ForceMode2D.Impulse;

        public void Explode()
        {
            Explode(force, transform.position, radius, explosionMask, upModifier, mode);
        }

        public static void Explode(float force, Vector2 position, float radius) => Explode(force, position, radius, default(float));
        public static void Explode(float force, Vector2 position, float radius, LayerMask layerMask) => Explode(force, position, radius, layerMask, default);
        public static void Explode(float force, Vector2 position, float radius, float upwardsModifier) => Explode(force, position, radius, upwardsModifier, default);
        public static void Explode(float force, Vector2 position, float radius, LayerMask layerMask, float upwardsModifier) => Explode(force, position, radius, layerMask, upwardsModifier, default);
        public static void Explode(float force, Vector2 position, float radius, float upwardsModifier, ForceMode2D mode) => Explode(force, position, radius, Physics2D.AllLayers, upwardsModifier, mode);
        public static void Explode(float force, Vector2 position, float radius, LayerMask layerMask, float upwardsModifier, ForceMode2D mode)
        {
            var contactFilter = new ContactFilter2D()
            {
                layerMask = layerMask,
                useLayerMask = true,
            };

            var collidersCount = Physics2D.OverlapCircle(position, radius, contactFilter, ExtraPhysics2D.collidersCache);
            var rigidbodies = new HashSet<Rigidbody2D>();

            for (int i = 0; i < collidersCount; i++)
            {
                var collider = ExtraPhysics2D.collidersCache[i];
                var rigidbody = collider.attachedRigidbody;

                if (rigidbody
                    && rigidbodies.Add(rigidbody))
                {
                    rigidbody.AddExplosionForce(force, position, radius, upwardsModifier, mode);
                }
            }
        }
    }
}
