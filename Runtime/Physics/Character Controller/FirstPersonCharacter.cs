#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;

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

        [field: Header("Look")]
        [field: SerializeField] public Transform? lookTransform { get; set; }
        [field: SerializeField] [field: Tooltip("How far in degrees can you move the camera up")] [field: Min(0f)] public float topClamp { get; set; } = 89.9f;
        [field: SerializeField] [field: Tooltip("How far in degrees can you move the camera down")] [field: Min(0f)] public float bottomClamp { get; set; } = 89.9f;

        [field: Header("Input")]
        [field: SerializeField] public bool enableInputOnStart { get; set; } = true;
        [field: SerializeField] public InputActionProperty moveAction { get; set; }
        [field: SerializeField] public InputActionProperty lookAction { get; set; }
        [field: SerializeField] public InputActionProperty sprintAction { get; set; }
        [field: SerializeField] public InputActionProperty jumpAction { get; set; }

        private float _lookPitch;

        private void Start()
        {
            if (enableInputOnStart)
            {
                moveAction.action.Enable();
                lookAction.action.Enable();
                sprintAction.action.Enable();
                jumpAction.action.Enable();
            }
        }

        private void OnEnable()
        {
            moveAction.action.performed += MovePerformed;
            lookAction.action.performed += LookPerformed;
            jumpAction.action.performed += JumpPerformed;
        }

        private void OnDisable()
        {
            moveAction.action.performed -= MovePerformed;
            lookAction.action.performed -= LookPerformed;
            jumpAction.action.performed -= JumpPerformed;
        }

        private void MovePerformed(InputAction.CallbackContext context)
        {
            var direction2D = context.ReadValue<Vector2>();
            var direction = new Vector3(direction2D.x, 0f, direction2D.y);
            characterMover.Move(direction, sprintAction.action.phase.IsInProgress());
        }

        private void LookPerformed(InputAction.CallbackContext context)
        {
            Look(context.ReadValue<Vector2>());
        }

        private void JumpPerformed(InputAction.CallbackContext context)
        {
            characterMover.Jump();
        }

        public void Look(Vector2 movement)
        {
            var turnFactor = movement.x;
            characterMover.Turn(turnFactor);

            var lookFactor = movement.y;
            if (lookTransform != null)
            {
                _lookPitch += lookFactor * characterMover.rotationSpeed * Time.deltaTime;
                _lookPitch = Mathf.Clamp(_lookPitch, -bottomClamp, topClamp);
                lookTransform.rotation = transform.rotation * Quaternion.Euler(_lookPitch, 0f, 0f);
            }
        }
    }
}
