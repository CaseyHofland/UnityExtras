#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtras
{
    public static class RigidbodyExtensions
    {
        /// <include file='./RigidbodyExtensions.xml' path='docs/GetAttachedColliders/List/*'/>
        private static List<Collider> GetAttachedColliders(this Rigidbody rigidbody)
        {
            rigidbody.GetComponentsInChildren(false, ExtraPhysics.collidersCache);
            ExtraPhysics.collidersCache.RemoveAll(collider => collider.attachedRigidbody != rigidbody);

            return ExtraPhysics.collidersCache;
        }

        /// <include file='./RigidbodyExtensions.xml' path='docs/GetAttachedColliders/*'/>
        public static int GetAttachedColliders(this Rigidbody rigidbody, Collider[] results)
        {
            var attachedColliders = rigidbody.GetAttachedColliders();
            var count = Math.Min(attachedColliders.Count, results.Length);
            
            attachedColliders.CopyTo(0, results, 0, count);
            Array.Clear(results, count, results.Length - count);

            return count;
        }

        /// <include file='./RigidbodyExtensions.xml' path='docs/GetAttachedColliders/*'/>
        public static int GetAttachedColliders(this Rigidbody rigidbody, List<Collider> results)
        {
            results.Clear();
            results.AddRange(rigidbody.GetAttachedColliders());
            return results.Count;
        }
    }
}
