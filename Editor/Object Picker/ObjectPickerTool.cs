#nullable enable
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

using Object = UnityEngine.Object;

namespace UnityExtras.Editor
{
    public class ObjectPickerTool : EditorTool
    {
		private const string displayName = "Object Picker";

		private Object? _hovered;

		public Type pickerType = typeof(object);
		public event Action<Object?>? selected;

		public override GUIContent toolbarIcon => new GUIContent(displayName, EditorGUIUtility.IconContent("Grid.PickingTool").image, displayName);

		public override void OnActivated()
		{
			_hovered = null;
			Selection.selectionChanged += SelectionChanged;
		}

		private void SelectionChanged()
		{
			ToolManager.RestorePreviousTool();
		}

		public override void OnWillBeDeactivated()
		{
			_hovered = null;
			Selection.selectionChanged -= SelectionChanged;
		}

		public override void OnToolGUI(EditorWindow window)
		{
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

			if (!(window is SceneView sceneView))
			{
				Debug.LogError("Not a scene view");
				return;
			}

			if (Event.current.type == EventType.MouseMove)
			{
				var filter = typeof(Component).IsAssignableFrom(pickerType)
					? FindObjectsOfType(pickerType).SelectMany(x => ((Component)x).GetComponentsInChildren<Transform>()).Select(x => x.gameObject).ToArray()
					: null;
				if (!TryPickObject(Event.current.mousePosition, true, filter, out _hovered))
				{
					TryPickObject(Event.current.mousePosition, false, filter, out _hovered);
				}
			}

			if (_hovered)
			{
				EditorGUIUtility.AddCursorRect(sceneView.position, MouseCursor.Link);
				Handles.Label(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition - Vector2.down * 30f).GetPoint(0), _hovered?.name);
			}

			if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				if (_hovered)
                {
					selected?.Invoke(_hovered);
                }
				ToolManager.RestorePreviousTool();
			}

			bool TryPickObject(Vector2 position, bool selectPrefabRoot, GameObject[]? filter, out Object? result)
			{
				var pickGO = HandleUtility.PickGameObject(position, selectPrefabRoot, null, filter);
				return result = pickGO == null || !typeof(Component).IsAssignableFrom(pickerType)
					? pickGO
					: pickGO.GetComponentInParent(pickerType);
			}
		}

		public static bool CanPickProperty(SerializedProperty property)
		{
			if (property == null || property.propertyType != SerializedPropertyType.ObjectReference)
            {
				return false;
            }

			var propType = property.GetSerializedPropertyType();
			if (propType != typeof(Object)
				&& !typeof(GameObject).IsAssignableFrom(propType)
				&& !typeof(Component).IsAssignableFrom(propType))
            {
				return false;
            }

			foreach (var target in property.serializedObject.targetObjects)
			{
				if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(target)))
                {
					return false;
                }
			}

			return true;
		}
	}
}
