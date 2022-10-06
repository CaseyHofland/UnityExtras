#nullable enable
using System;
using UnityEngine;

namespace UnityExtras
{
    public enum StoreMethod
    {
        None,
        Store,
        Override,
    }

    [Serializable]
    public class ValueStore<T>
    {
        [field: SerializeField][field: Tooltip("The value that should be returned on " + nameof(StoreValue) + " based on the " + nameof(StoreMethod) + ".")] public T? value { get; set; }
        [field: SerializeField][field: Tooltip("How " + nameof(value) + " will be stored inside of " + nameof(storedValue) + ".\n" + nameof(StoreMethod.None) + " doesn't store the value at all.\n" + nameof(StoreMethod.Store) + " stores the value.\n" + nameof(StoreMethod.Override) + " overrides the value.")] public StoreMethod storeMethod { get; set; }
        public T? storedValue { get; private set; }

        public ValueStore() { } 

        public ValueStore(T value) : this() 
        {
            this.value = this.storedValue = value;
        }

        public ValueStore(T value, StoreMethod storeMethod) : this(value)
        {
            this.storeMethod = storeMethod;
        }

        public T? StoreValue(T value)
        {
            switch (storeMethod)
            {
                case StoreMethod.Store:
                    this.storedValue = value;
                    return this.value;
                case StoreMethod.Override:
                    this.storedValue = this.value;
                    return this.value;
                default:
                    this.storedValue = value;
                    return value;
            }
        }
    }
}
