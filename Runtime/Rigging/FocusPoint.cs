#nullable enable
using UnityEngine;

namespace UnityExtras.Rigging
{
    public class FocusPoint : MonoBehaviour
    {
        [field: SerializeField, Tooltip("The focus points this focus point will alter.")] public FocusPoints? focusPoints { get; set; }
        [field: SerializeField, Tooltip("The priority of this focus point. Higher = More Interesting.")] public int priority { get; set; }
        [field: SerializeField, Header("Trigger Settings"), Tooltip("Should the collider's tag be checked when it enters the trigger?")] public bool checkTag { get; set; }
        [field: SerializeField, Tag, Tooltip("The collision tag the trigger checks for.")] public string? collisionTag { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (checkTag && other.CompareTag(collisionTag) && focusPoints != null)
            {
                focusPoints.Add(priority, transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (checkTag && other.CompareTag(collisionTag) && focusPoints != null)
            {
                focusPoints.Remove(priority, transform);
            }
        }
    }
}
