#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;
using Input = UnityExtras.InputSystem.Input;

namespace UnityExtras.Events
{
    public class InputSwitch : Switch
    {
        [field: SerializeField] public Input input { get; set; }

        protected override void OnEnable()
        {
            base.OnEnable();

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
