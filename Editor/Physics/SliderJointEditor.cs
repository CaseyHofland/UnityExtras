#nullable enable
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace UnityExtras.Editor
{
    [EditorTool(displayName, typeof(SliderJoint))]
    public class SliderJointAnchorEditorTool : EditorTool
    {
        private const string displayName = "Edit Slider Joint Anchors";

        public override GUIContent toolbarIcon => new GUIContent(displayName, EditorGUIUtility.IconContent("AvatarPivot").image, displayName);

        public override void OnToolGUI(EditorWindow window)
        {
            var sliderJoint = target as SliderJoint;
            if (sliderJoint == null)
            {
                return;
            }

            Undo.RecordObject(sliderJoint, "Slider Joint Modified");

            var anchorPosition = sliderJoint.transform.TransformPoint(sliderJoint.anchor);
            var connectedAnchorPosition = sliderJoint.connectedBody?.transform.TransformPoint(sliderJoint.connectedAnchor) ?? sliderJoint.connectedAnchor;

            EditorGUI.BeginChangeCheck();
            anchorPosition = Handles.PositionHandle(anchorPosition, sliderJoint.transform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                sliderJoint.anchor = sliderJoint.transform.InverseTransformPoint(anchorPosition);
            }

            EditorGUI.BeginChangeCheck();
            connectedAnchorPosition = Handles.PositionHandle(connectedAnchorPosition, sliderJoint.connectedBody?.rotation ?? Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                sliderJoint.connectedAnchor = sliderJoint.connectedBody?.transform.InverseTransformPoint(connectedAnchorPosition) ?? connectedAnchorPosition;
            }
        }
    }

    [EditorTool(displayName, typeof(SliderJoint))]
    public class SliderJointAngleEditorTool : EditorTool
    {
        private const string displayName = "Edit Slider Joint Angle";

        public override GUIContent toolbarIcon => new GUIContent(displayName, EditorGUIUtility.IconContent("RotateTool").image, displayName);

        public override void OnToolGUI(EditorWindow window)
        {
            var sliderJoint = target as SliderJoint;
            if (sliderJoint == null)
            {
                return;
            }

            Undo.RecordObject(sliderJoint, "Slider Joint Modified");

            var anchorPosition = sliderJoint.transform.TransformPoint(sliderJoint.anchor);
            var angle = sliderJoint.transform.rotation * sliderJoint.angle;

            EditorGUI.BeginChangeCheck();
            angle = Handles.RotationHandle(angle, anchorPosition);
            if (EditorGUI.EndChangeCheck())
            {
                sliderJoint.angle = Quaternion.Inverse(sliderJoint.transform.rotation) * angle;
            }
        }
    }

    [EditorTool(displayName, typeof(SliderJoint))]
    public class SliderJointLimitsEditorTool : EditorTool
    {
        private const string displayName = "Edit Slider Joint Limits";

        public override GUIContent toolbarIcon => new GUIContent(displayName, EditorGUIUtility.IconContent("CreateAddNew").image, displayName);

        public override void OnToolGUI(EditorWindow window)
        {
            var sliderJoint = target as SliderJoint;
            if (sliderJoint == null)
            {
                return;
            }

            Undo.RecordObject(sliderJoint, "Slider Joint Modified");

            var anchorPosition = sliderJoint.transform.TransformPoint(sliderJoint.anchor);
            var axis = sliderJoint.transform.rotation * sliderJoint.angle * Vector3.right;

            var minPosition = anchorPosition + sliderJoint.minDistance * axis;
            var maxPosition = anchorPosition + sliderJoint.maxDistance * axis;

            Handles.color = Color.green;
            EditorGUI.BeginChangeCheck();
            minPosition = Handles.Slider(minPosition, -axis);
            if (EditorGUI.EndChangeCheck())
            {
                var diff = minPosition - anchorPosition;
                sliderJoint.minDistance = Mathf.Sign(Vector3.Dot(diff, axis)) * diff.magnitude;
            }

            EditorGUI.BeginChangeCheck();
            maxPosition = Handles.Slider(maxPosition, axis);
            if (EditorGUI.EndChangeCheck())
            {
                var diff = maxPosition - anchorPosition;
                sliderJoint.maxDistance = Mathf.Sign(Vector3.Dot(diff, axis)) * diff.magnitude;
            }
        }
    }

    [CustomEditor(typeof(SliderJoint))]
    [CanEditMultipleObjects]
    public class SliderJointEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            DrawSceneHandles();
        }

        private void DrawSceneHandles()
        {
            var sliderJoint = target as SliderJoint;
            var expanded = UnityEditorInternal.InternalEditorUtility.GetIsInspectorExpanded(sliderJoint);
            if (sliderJoint == null || !expanded)
            {
                return;
            }

            var anchorPosition = sliderJoint.transform.TransformPoint(sliderJoint.anchor);
            var connectedAnchorPosition = sliderJoint.connectedBody != null ? sliderJoint.connectedBody.transform.TransformPoint(sliderJoint.connectedAnchor) : sliderJoint.connectedAnchor;

            Handles.color = Color.blue;
            var size = HandleUtility.GetHandleSize(anchorPosition) * 0.1f + 0.01f;
            Handles.FreeMoveHandle(anchorPosition, sliderJoint.transform.rotation, size, default, Handles.SphereHandleCap);
            Handles.FreeMoveHandle(connectedAnchorPosition, sliderJoint.connectedBody?.rotation ?? Quaternion.identity, size, default, Handles.SphereHandleCap);

            Handles.color = Color.green;
            Handles.DrawAAPolyLine(anchorPosition, connectedAnchorPosition);

            var sliderDirection = sliderJoint.transform.rotation * sliderJoint.angle * Vector3.right;
            var perpendicularDirection = sliderJoint.transform.rotation * sliderJoint.angle * Vector3.forward;
            if (!sliderJoint.useLimits)
            {
                var min = anchorPosition - size * 2f * sliderDirection;
                var max = anchorPosition + size * 2f * sliderDirection;
                Handles.DrawAAPolyLine(min, max);
            }
            else
            {
                var min = anchorPosition + sliderJoint.minDistance * sliderDirection;
                var max = anchorPosition + sliderJoint.maxDistance * sliderDirection;
                Handles.DrawAAPolyLine(min, max);

                var edge = size * (sliderJoint.transform.rotation * sliderJoint.angle * Vector3.up);
                Handles.DrawAAPolyLine(min - edge, min + edge);
                Handles.DrawAAPolyLine(max - edge, max + edge);
            }

            Handles.color = Color.blue;
            var perpendicularMin = anchorPosition - size * 2f * perpendicularDirection;
            var perpendicularMax = anchorPosition + size * 2f * perpendicularDirection;
            Handles.DrawAAPolyLine(perpendicularMin, perpendicularMax);
        }
    }
}
