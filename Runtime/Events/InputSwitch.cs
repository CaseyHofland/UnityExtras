#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;
using Input = UnityExtras.InputSystem.Input;

namespace UnityExtras.Events
{
    public class InputSwitch : Switch
    {
        [field: SerializeField] public Input input { get; set; }

        protected virtual void OnEnable()
        {
            if (input.action != null)
            {
                input.action.performed += Performed;
            }
        }

        protected virtual void OnDisable()
        {
            if (input.action != null)
            {
                input.action.performed -= Performed;
            }
        }

        private void Performed(InputAction.CallbackContext context)
        {
            isOn ^= true;
        }
    }
}
