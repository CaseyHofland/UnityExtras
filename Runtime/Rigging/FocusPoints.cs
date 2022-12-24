#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtras.Rigging
{
    [CreateAssetMenu(fileName = nameof(FocusPoints), menuName = nameof(UnityExtras) + "/" + nameof(UnityExtras.Rigging) + "/" + nameof(FocusPoints))]
    public class FocusPoints : ScriptableObject
    {
        [field: SerializeField, Tooltip("How to pick between focus points with duplicate priorities.")] public DuplicatePriorityResolution duplicatePriorityResolution { get; set; }
        public SortedList<int, List<Transform>> focusPointsByPriorities { get; set; } = new(Comparer<int>.Create((x, y) => y.CompareTo(x)));

        public enum DuplicatePriorityResolution
        {
            NewerWins,
            OlderWins,
        }

        public void Add(int priority, Transform transform)
        {
            if (!focusPointsByPriorities.TryGetValue(priority, out var transforms))
            {
                focusPointsByPriorities.Add(priority, transforms = new List<Transform>());
            }

            switch (duplicatePriorityResolution)
            {
                case DuplicatePriorityResolution.OlderWins:
                    transforms.Add(transform);
                    break;
                default:
                    transforms.Insert(0, transform);
                    break;
            }
        }

        public void Remove(int priority, Transform transform)
        {
            if (!focusPointsByPriorities.TryGetValue(priority, out var transforms))
            {
                return;
            }

            if (transforms.Remove(transform) && transforms.Count == 0) 
            {
                focusPointsByPriorities.Remove(priority);
            }
        }
    }
}
