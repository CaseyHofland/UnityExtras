#nullable enable
using UnityEngine;
using UnityEditor;

namespace UnityExtras.Editor
{
    public class RigidbodyMenus
    {
        [MenuItem("CONTEXT/Rigidbody/Use 2D Drag")]
        private static void Use2DDrag(MenuCommand command)
        {
            var rigidbody = (Rigidbody)command.context;

            var fixedDrag = Time.fixedDeltaTime * rigidbody.drag;
            var dragFactor2D = 1f + fixedDrag;
            rigidbody.drag /= dragFactor2D;
            rigidbody.angularDrag /= dragFactor2D;
        }
    }
}
