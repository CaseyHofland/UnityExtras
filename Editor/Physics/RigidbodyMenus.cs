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

            Undo.RecordObject(rigidbody, "Use 2D Drag");

            var fixedDrag = Time.fixedDeltaTime * rigidbody.drag;
            var dragFactor2D = 1f + fixedDrag;
            rigidbody.drag /= dragFactor2D;

            var fixedAngularDrag = Time.fixedDeltaTime * rigidbody.angularDrag;
            var angularDragFactor2D = 1f + fixedAngularDrag;
            rigidbody.angularDrag /= angularDragFactor2D;
        }
    }
}
