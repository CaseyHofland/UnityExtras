#nullable enable
using UnityEngine;

namespace UnityExtras
{
    public static class Rigidbody2DExtensions
    {
        /// <include file='Rigidbody2DExtensions.xml' path='docs/AddExplosionForce'/>
        public static void AddExplosionForce(this Rigidbody2D rigidbody2D, float explosionForce, Vector2 explosionPosition, float explosionRadius) => AddExplosionForce(rigidbody2D, explosionForce, explosionPosition, explosionRadius, 0f);
        /// <include file='Rigidbody2DExtensions.xml' path='docs/AddExplosionForce'/>
        public static void AddExplosionForce(this Rigidbody2D rigidbody2D, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier) => AddExplosionForce(rigidbody2D, explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode2D.Force);
        /// <include file='Rigidbody2DExtensions.xml' path='docs/AddExplosionForce'/>
        public static void AddExplosionForce(this Rigidbody2D rigidbody2D, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier, ForceMode2D mode)
        {
            // Prepare values.
            explosionRadius = Mathf.Approximately(explosionRadius, 0f)
                ? float.PositiveInfinity
                : Mathf.Abs(explosionRadius);
            var forceWearoff = 1f;  // A value from 0 to 1 that is lower based on the explosions' distance from the rigidbody.
            bool isOverlapping;     // Is the rigidbody overlapping with the explosion or not.

            // Get bounds. We will be using this to get the same behaviour as Rigidbody.AddExplosionForce.
            Bounds bounds;
            var attachedColliderCount = rigidbody2D.GetAttachedColliders(ExtraPhysics2D.collidersCache);
            if (attachedColliderCount == 0)
            {
                bounds = new Bounds(rigidbody2D.worldCenterOfMass, Vector3.zero);
            }
            else
            {
                bounds = ExtraPhysics2D.collidersCache[0].bounds;
                for (int i = 1; i < attachedColliderCount; i++)
                {
                    var collider = ExtraPhysics2D.collidersCache[i];
                    bounds.Encapsulate(collider.bounds);
                }
            }

            // If the explosionRadius is infinity we can skip calculating the force wearoff.
            if (!float.IsPositiveInfinity(explosionRadius))
            {
                // Get the closest point on the rigidbody2D for calculating the explosion distance.
                Vector2 closestPoint = bounds.ClosestPoint(explosionPosition);
                isOverlapping = attachedColliderCount > 0 && closestPoint == explosionPosition;

                // Get the explosion distance.
                var explosionDistance = (closestPoint - explosionPosition).magnitude;

                // If the explosion distance is further than the explosion radius, don't add an explosion force.
                if (explosionDistance > explosionRadius)
                {
                    return;
                }

                forceWearoff -= explosionDistance / explosionRadius;
            }
            else
            {
                isOverlapping = rigidbody2D.OverlapPoint(explosionPosition);
            }

            // Get the force position.
            var upwardsExplosionPosition = explosionPosition + Vector2.down * upwardsModifier;
            var forcePosition = (Vector2)bounds.ClosestPoint(upwardsExplosionPosition);
            if (forcePosition == upwardsExplosionPosition)
            {
                forcePosition = rigidbody2D.worldCenterOfMass;
            }
            
            // Get the force direction.
            Vector2 forceDirection = forcePosition - upwardsExplosionPosition;
            if (forceDirection.sqrMagnitude <= float.Epsilon)
            {
                forceDirection = Vector2.up;    // Default the force direction to up.
            }
            else if (!isOverlapping)
            {
                forceDirection.Normalize();     // Only normalize the force direction if we aren't overlapping with the explosion.
            }

            // Apply the force at the force position.
            var force = explosionForce * forceWearoff * forceDirection;
            rigidbody2D.AddForceAtPosition(force, forcePosition, mode);
        }

        /// <include file='Rigidbody2DExtensions.xml' path='docs/ClosestPointOnBounds'/>
        public static Vector2 ClosestPointOnBounds(this Rigidbody2D rigidbody2D, Vector2 position)
        {
            var count = rigidbody2D.GetAttachedColliders(ExtraPhysics2D.collidersCache);
            if (count == 0)
            {
                return position;
            }

            var bounds = ExtraPhysics2D.collidersCache[0].bounds;
            for (int i = 1; i < count; i++)
            {
                var collider = ExtraPhysics2D.collidersCache[i];
                bounds.Encapsulate(collider.bounds);
            }

            return bounds.ClosestPoint(position);
        }
    }
}
