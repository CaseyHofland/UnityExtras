#nullable enable
using UnityEngine;

namespace UnityExtras
{
    [AddComponentMenu("Physics/First Person Character")]
    [RequireComponent(typeof(CharacterMover))]
    [DisallowMultipleComponent]
    public class FirstPersonCharacter : MonoBehaviour
    {
        private CharacterMover? _characterMover;
        public CharacterMover characterMover => _characterMover ? _characterMover! : (_characterMover = GetComponent<CharacterMover>());

        public static implicit operator CharacterMover(FirstPersonCharacter firstPersonCharacter) => firstPersonCharacter.characterMover;

        private const float lookThreshold = 0.1f;

        [field: Header("Looking")]
        [field: SerializeField] public Transform? lookTransformOverride { get; set; }
        [field: SerializeField][field: Tooltip("How far in degrees can you move the camera up")] [field: Min(0f)] public float topClamp { get; set; } = 90.0f;
        [field: SerializeField] [field: Tooltip("How far in degrees can you move the camera down")][field: Min(0f)] public float bottomClamp { get; set; } = 90.0f;

        private float _lookPitch;

        public void Look(Vector2 movement)
        {
            var turnFactor = movement.x;
            if (turnFactor >= lookThreshold || turnFactor <= -lookThreshold)
            {
                characterMover.Turn(turnFactor);
            }

            var lookFactor = movement.y;
            var lookTransform = lookTransformOverride ? lookTransformOverride! : Camera.main.transform;
            if (lookFactor >= lookThreshold || lookFactor <= -lookThreshold && lookTransform != null)
            {
                _lookPitch += lookFactor * characterMover.rotationSpeed * Time.deltaTime;
                _lookPitch = Mathf.Clamp(_lookPitch, -bottomClamp, topClamp);
                lookTransform.rotation = transform.rotation * Quaternion.Euler(_lookPitch, 0f, 0f);
            }
        }
    }
}
