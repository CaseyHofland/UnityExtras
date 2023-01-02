#nullable enable
using UnityEditor;
using UnityEngine;

namespace UnityExtras.Editor
{
    public static partial class Menus
    {
		[InitializeOnLoadMethod]
		private static void InitializeContextualMenus()
		{
			EditorApplication.contextualPropertyMenu += ContextResetMenu;
		}

		private static void ContextResetMenu(GenericMenu menu, SerializedProperty property)
		{
			var propertyCopy = property.Copy();

			menu.AddItem(new GUIContent("Reset"), false, () =>
			{
				switch (propertyCopy.propertyType)
                {
					case SerializedPropertyType.AnimationCurve:
						propertyCopy.animationCurveValue = default;
						break;
					case SerializedPropertyType.ArraySize:
						propertyCopy.arraySize = default;
						break;
					case SerializedPropertyType.Boolean:
						propertyCopy.boolValue = default;
						break;
					case SerializedPropertyType.Bounds:
						propertyCopy.boundsValue = default;
						break;
					case SerializedPropertyType.BoundsInt:
						propertyCopy.boundsIntValue = default;
						break;
					case SerializedPropertyType.Character:
						break;
					case SerializedPropertyType.Color:
						propertyCopy.colorValue = default;
						break;
					case SerializedPropertyType.Enum:
						propertyCopy.enumValueIndex = propertyCopy.enumValueFlag = default;
						break;
					case SerializedPropertyType.ExposedReference:
						propertyCopy.exposedReferenceValue = default;
						break;
					case SerializedPropertyType.FixedBufferSize:
						break;
					case SerializedPropertyType.Float:
						propertyCopy.floatValue = default;
						propertyCopy.doubleValue = default;
						break;
					case SerializedPropertyType.Generic:
						break;
					case SerializedPropertyType.Gradient:
						break;
					case SerializedPropertyType.Hash128:
						propertyCopy.hash128Value = default;
						break;
					case SerializedPropertyType.Integer:
					case SerializedPropertyType.LayerMask:
						propertyCopy.intValue = default;
						propertyCopy.longValue = default;
						break;
					case SerializedPropertyType.ManagedReference:
						propertyCopy.managedReferenceValue = default;
						break;
					case SerializedPropertyType.ObjectReference:
						propertyCopy.objectReferenceValue = default;
						break;
					case SerializedPropertyType.Quaternion:
						propertyCopy.quaternionValue = Quaternion.identity;
						break;
					case SerializedPropertyType.Rect:
						propertyCopy.rectValue = default;
						break;
					case SerializedPropertyType.RectInt:
						propertyCopy.rectIntValue = default;
						break;
					case SerializedPropertyType.String:
						propertyCopy.stringValue = default;
						break;
					case SerializedPropertyType.Vector2:
						propertyCopy.vector2Value = default;
						break;
					case SerializedPropertyType.Vector2Int:
						propertyCopy.vector2IntValue = default;
						break;
					case SerializedPropertyType.Vector3:
						propertyCopy.vector3Value = default;
						break;
					case SerializedPropertyType.Vector3Int:
						propertyCopy.vector3IntValue = default;
						break;
					case SerializedPropertyType.Vector4:
						propertyCopy.vector4Value = default;
						break;
                }

				propertyCopy.serializedObject.ApplyModifiedProperties();
			});
		}
	}
}
