#nullable enable
using UnityEditor;
using UnityEngine;
//using UnityEditor.EditorTools;

namespace UnityExtras.Editor
{
    //[EditorTool("Edit Target Joint", typeof(TargetJoint))]
    //public class TargetJointTool : EditorTool
    //{
    //    public override void OnToolGUI(EditorWindow window)
    //    {
    //        var targetJoint = target as TargetJoint;
    //        Undo.RecordObject(targetJoint, "Target Joint Modified");

    //        var anchorPosition = targetJoint.transform.position + targetJoint.transform.rotation * targetJoint.anchor;
    //        var targetPosition = targetJoint.target;

    //        Handles.color = Color.green;
    //        Handles.DrawDottedLine(anchorPosition, targetPosition, 5f);

    //        Handles.color = Color.blue;
    //        EditorGUI.BeginChangeCheck();
    //        anchorPosition = Handles.PositionHandle(anchorPosition, targetJoint.transform.rotation);
    //        if (EditorGUI.EndChangeCheck())
    //        {
    //            targetJoint.anchor = Quaternion.Inverse(targetJoint.transform.rotation) * (anchorPosition - targetJoint.transform.position);
    //        }

    //        EditorGUI.BeginChangeCheck();
    //        targetPosition = Handles.PositionHandle(targetPosition, Quaternion.identity);
    //        if (EditorGUI.EndChangeCheck())
    //        {
    //            targetJoint.target = targetPosition;
    //        }

    //        //base.OnToolGUI(window);
    //    }
    //}

    [CustomEditor(typeof(TargetJoint))]
    [CanEditMultipleObjects]
    public class TargetJointEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            DrawSceneHandles();
        }

        private void DrawSceneHandles()
        {
            var targetJoint = target as TargetJoint;
            var expanded = UnityEditorInternal.InternalEditorUtility.GetIsInspectorExpanded(targetJoint);
            if (targetJoint == null || !expanded)
            {
                return;
            }

            const string undoRecordName = "Target Joint Modified";
            var anchorPosition = targetJoint.transform.position + targetJoint.transform.rotation * targetJoint.anchor;
            var targetPosition = targetJoint.target;

            Handles.color = Color.green;
            Handles.DrawDottedLine(anchorPosition, targetPosition, 5f);

            Handles.color = Color.blue;
            EditorGUI.BeginChangeCheck();
            anchorPosition = Handles.PositionHandle(anchorPosition, targetJoint.transform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetJoint, undoRecordName);
                targetJoint.anchor = Quaternion.Inverse(targetJoint.transform.rotation) * (anchorPosition - targetJoint.transform.position);
            }

            EditorGUI.BeginChangeCheck();
            targetPosition = Handles.PositionHandle(targetPosition, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetJoint, undoRecordName);
                targetJoint.target = targetPosition;
            }
        }
    }
}