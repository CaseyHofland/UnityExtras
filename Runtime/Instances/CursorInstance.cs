#nullable enable
using UnityEngine;

namespace UnityExtras
{
    [DisallowMultipleComponent]
    public class CursorInstance : Instance<CursorInstance>
    {
        [field: SerializeField] public CursorLockMode lockState { get; set; } = CursorLockMode.Locked;
        [field: SerializeField] public bool visible { get; set; } = true;

        [ContextMenu(nameof(SetState))]
        public override void SetState()
        {
            Cursor.lockState = lockState;
            Cursor.visible = visible;
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus && enabled && current == this)
            {
                SetState();
            }
        }
    }
}
