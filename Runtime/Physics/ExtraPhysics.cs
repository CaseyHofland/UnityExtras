#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtras
{
    public class ExtraPhysics
    {
        internal static List<Collider> collidersCache = new List<Collider>();
        internal static List<Collider> collidersComparisonCache = new List<Collider>();

        public static int GetLayerCollisionMask(int layer)
        {
            if (layer < 0 || layer > 31)
            {
                throw new ArgumentOutOfRangeException($"{nameof(layer)} is out of range. Layer numbers must be in the range 0 to 31.");
            }

            int layerCollisionMask = 0;
            for (int i = 0; i < 32; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(layer, i))
                {
                    layerCollisionMask |= 1 << i;
                }
            }
            return layerCollisionMask;
        }

        public static void SetLayerCollisionMask(int layer, int layerMask)
        {
            if (layer < 0 || layer > 31)
            {
                throw new ArgumentOutOfRangeException($"{nameof(layer)} is out of range. Layer numbers must be in the range 0 to 31.");
            }

            for (int i = 0; i < 32; i++)
            {
                Physics.IgnoreLayerCollision(layer, i, (layerMask & (1 << i)) == 0);
            }
        }
    }
}
