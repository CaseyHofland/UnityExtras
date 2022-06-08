#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
    [Serializable]
    public struct Layer
    {
        [SerializeField] private int _value;
        public int value
        {
            get => _value;
            set => _value = Mathf.Clamp(0, sizeof(int) * 8 - 1, value);
        }

        public static implicit operator int(Layer layer) => layer.value;
        public static implicit operator Layer(int value) => new() { value = value };

        public static string LayerToName(int layer) => LayerMask.LayerToName(layer);
        public static int NameToLayer(string layerName) => LayerMask.NameToLayer(layerName);
    }
}
