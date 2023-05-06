using UnityEngine;
using UnityExtras;

[AddComponentMenu("Physics/Character Slope Slider")]
[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterMover))]
public class CharacterSlopeSlider : MonoBehaviour
{
    [field: SerializeField, HideInInspector] public CharacterMover characterMover { get; private set; }

    [field: SerializeField, Min(0f)] public float slideSpeed { get; set; } = 2f;

    private void InitializeComponents()
    {
        characterMover = GetComponent<CharacterMover>();
    }

    private void Reset()
    {
        InitializeComponents();
    }

    private void Awake()
    {
        InitializeComponents();
    }

    private void Start() { } // Included to make enabled toggle show up in inspector.

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Check the script is enabled.
        if (!isActiveAndEnabled)
        {
            return;
        }

        // Check if we are sliding.
        var hitNormal = hit.normal;
        var angle = Vector3.Angle(Vector3.up, hitNormal);
        var isSliding = (angle > hit.controller.slopeLimit && angle <= 90f);
        if (isSliding)
        {
            // Slide along the slopes surface. This ensures the character stays grounded.
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitNormal);
            var slopeVelocity = slopeRotation * new Vector3(hitNormal.x, 0f, hitNormal.z) * slideSpeed;
            characterMover.targetMotion += slopeVelocity;

            // Prevent jumping if we are sliding, but not if we are walking against a slide.
            var collisionFlags = hit.controller.collisionFlags;
            if (!(collisionFlags.HasFlag(CollisionFlags.Below) && collisionFlags.HasFlag(CollisionFlags.Sides)))
            {
                characterMover.ForcedJump(0f);
            }
        }
    }
}
