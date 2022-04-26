#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtras
{
    public class Explosion : MonoBehaviour
    {
        private static Collider[] collidersCache = new Collider[256];

        [field: SerializeField] public float force { get; set; } = 10f;
        [field: SerializeField] public float radius { get; set; } = 5f;
        [field: SerializeField] public LayerMask explosionMask { get; set; } = Physics.AllLayers;
        [field: SerializeField] public float upModifier { get; set; } = 1f;
        [field: SerializeField] public ForceMode mode { get; set; } = ForceMode.Impulse;

        public void Explode()
        {
            ExplodeNonAlloc(force, transform.position, radius, collidersCache, explosionMask, upModifier, mode);
        }

        public static void Explode(float force, Vector3 position, float radius) => Explode(force, position, radius, default(float));
        public static void Explode(float force, Vector3 position, float radius, LayerMask layerMask) => Explode(force, position, radius, layerMask, default);
        public static void Explode(float force, Vector3 position, float radius, float upwardsModifier) => Explode(force, position, radius, upwardsModifier, default);
        public static void Explode(float force, Vector3 position, float radius, LayerMask layerMask, float upwardsModifier) => Explode(force, position, radius, layerMask, upwardsModifier, default);
        public static void Explode(float force, Vector3 position, float radius, float upwardsModifier, ForceMode mode) => Explode(force, position, radius, Physics.AllLayers, upwardsModifier, mode);
        public static void Explode(float force, Vector3 position, float radius, LayerMask layerMask, float upwardsModifier, ForceMode mode)
        {
            var colliders = Physics.OverlapSphere(position, radius, layerMask, QueryTriggerInteraction.Ignore);
            var rigidbodies = new HashSet<Rigidbody>();

            for (int i = 0; i < colliders.Length; i++)
            {
                var collider = colliders[i];
                var rigidbody = collider.attachedRigidbody;

                if (rigidbody
                    && rigidbodies.Add(rigidbody))
                {
                    rigidbody.AddExplosionForce(force, position, radius, upwardsModifier, mode);
                }
            }
        }

        public static void ExplodeNonAlloc(float force, Vector3 position, float radius, Collider[] results) => ExplodeNonAlloc(force, position, radius, results, default(float));
        public static void ExplodeNonAlloc(float force, Vector3 position, float radius, Collider[] results, LayerMask layerMask) => ExplodeNonAlloc(force, position, radius, results, layerMask, default);
        public static void ExplodeNonAlloc(float force, Vector3 position, float radius, Collider[] results, float upwardsModifier) => ExplodeNonAlloc(force, position, radius, results, upwardsModifier, default);
        public static void ExplodeNonAlloc(float force, Vector3 position, float radius, Collider[] results, LayerMask layerMask, float upwardsModifier) => ExplodeNonAlloc(force, position, radius, results, layerMask, upwardsModifier, default);
        public static void ExplodeNonAlloc(float force, Vector3 position, float radius, Collider[] results, float upwardsModifier, ForceMode mode) => ExplodeNonAlloc(force, position, radius, results, Physics.AllLayers, upwardsModifier, mode);
        public static void ExplodeNonAlloc(float force, Vector3 position, float radius, Collider[] results, LayerMask layerMask, float upwardsModifier, ForceMode mode)
        {
            var collidersCount = Physics.OverlapSphereNonAlloc(position, radius, results, layerMask, QueryTriggerInteraction.Ignore);
            var rigidbodies = new HashSet<Rigidbody>();

            for (int i = 0; i < collidersCount; i++)
            {
                var collider = results[i];
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
