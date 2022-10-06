#nullable enable
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(Object), true)]
    public class ObjectDrawer : PropertyDrawer
    {
		private ObjectPickerTool objectPicker = ScriptableObject.CreateInstance<ObjectPickerTool>();

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float baseHeight = EditorGUIUtility.singleLineHeight;

			ScriptableObject? data = 
				property != null
				&& property.isExpanded
				&& property.propertyType == SerializedPropertyType.ObjectReference
				&& !property.hasMultipleDifferentValues
				? property.objectReferenceValue as ScriptableObject
				: null;

			if (data)
			{
				baseHeight += EditorGUIUtility.standardVerticalSpacing * 2;

				SerializedObject so = new SerializedObject(data);
				SerializedProperty iterator = so.GetIterator();
				iterator.NextVisible(true);

				var foldoutHeight = 0f;
				while (iterator.NextVisible(false))
				{
					var current = iterator.Copy();
					foldoutHeight += EditorGUI.GetPropertyHeight(current, label, true) + EditorGUIUtility.standardVerticalSpacing;
				}
				baseHeight += Mathf.Max(EditorGUIUtility.singleLineHeight, foldoutHeight);
			}
			return baseHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
			EditorGUI.BeginProperty(position, label, property);

            PropertyField(position, property, label);

            EditorGUI.EndProperty();
        }

        public void PropertyField(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect propertyFieldRect = PropertyFieldRight(position, property);
            EditorGUI.PropertyField(propertyFieldRect, property, label, true);
        }

		private Rect PropertyFieldRight(Rect position, SerializedProperty property)
		{
			int singleButtonWidth = 20;
			Rect propertyFieldRect = position;

			// Draw "Picker tool"
			if (ObjectPickerTool.CanPickProperty(property))
			{
				Rect buttonRect = new Rect(propertyFieldRect.xMax - singleButtonWidth, position.y, singleButtonWidth, EditorGUIUtility.singleLineHeight);

				var isPicking = ToolManager.IsActiveTool(objectPicker);
				EditorGUI.BeginChangeCheck();
				isPicking = ImageToggle(buttonRect, objectPicker.toolbarIcon, isPicking);
				if (EditorGUI.EndChangeCheck())
                {
					if (isPicking)
                    {
						var serializedPropertyType = property.GetSerializedPropertyType();
						if (serializedPropertyType != null)
                        {
							objectPicker.pickerType = serializedPropertyType;
							ToolManager.SetActiveTool(objectPicker);
							objectPicker.selected += obj =>
							{
								property.objectReferenceValue = obj;
								property.serializedObject.ApplyModifiedProperties();
							};
                        }
					}
					else
                    {
						objectPicker.selected -= obj =>
						{
							property.objectReferenceValue = obj;
							property.serializedObject.ApplyModifiedProperties();
						};
						ToolManager.RestorePreviousTool();
					}
				}
				propertyFieldRect.xMax -= singleButtonWidth;
			}

			return propertyFieldRect;
		}

        private static bool ImageToggle(Rect position, GUIContent label, bool value)
        {
			var result = GUI.Toggle(position, value, new GUIContent("", label.tooltip), "Button");

			var imageSize = new Vector2(label.image.width, label.image.height);
			var imageRect = new Rect(position.center - imageSize / 2f, imageSize);

			Color prevColor = GUI.color;
			GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.gray;
			GUI.DrawTexture(imageRect, label.image);
			GUI.color = prevColor;

			return result;
        }
    }
}
