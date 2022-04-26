#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtras
{
    public class ExtraPhysics2D
    {
        internal static List<Collider2D> collidersCache = new List<Collider2D>();
        internal static List<Collider2D> collidersComparisonCache = new List<Collider2D>();

        public static void IgnoreCollision(Rigidbody2D rigidbody, Collider2D collider) => IgnoreCollision(rigidbody, collider, true);
        public static void IgnoreCollision(Rigidbody2D rigidbody, Collider2D collider, bool ignore)
        {
            var collidersCount = rigidbody.GetAttachedColliders(collidersCache);

            for (int i = 0; i < collidersCount; i++)
            {
                var attachedCollider = collidersCache[i];

                Physics2D.IgnoreCollision(attachedCollider, collider, ignore);
            }
        }

        public static void IgnoreCollision(Rigidbody2D rigidbody1, Rigidbody2D rigidbody2) => IgnoreCollision(rigidbody1, rigidbody2, true);
        public static void IgnoreCollision(Rigidbody2D rigidbody1, Rigidbody2D rigidbody2, bool ignore)
        {
            var collidersCount = rigidbody1.GetAttachedColliders(collidersCache);
            var collidersComparisonCount = rigidbody2.GetAttachedColliders(collidersComparisonCache);

            for (int i1 = 0; i1 < collidersCount; i1++)
            {
                var collider1 = collidersCache[i1];

                for (int i2 = 0; i2 < collidersComparisonCount; i2++)
                {
                    var collider2 = collidersComparisonCache[i2];

                    Physics2D.IgnoreCollision(collider1, collider2, ignore);
                }
            }
        }
    }
}
