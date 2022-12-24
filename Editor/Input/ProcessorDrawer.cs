#nullable enable
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityExtras.Editor;

namespace UnityExtras.InputSystem.Editor
{
#pragma warning disable IDE0065 // Misplaced using directive
    using InputSystem = UnityEngine.InputSystem.InputSystem;
#pragma warning restore IDE0065 // Misplaced using directive

    [CustomPropertyDrawer(typeof(Processor))]
    public class ProcessorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty _inputProcessorAssemblyQualifiedName = property.FindPropertyRelative(nameof(_inputProcessorAssemblyQualifiedName));
            SerializedProperty _inputProcessorValueStrings = property.FindPropertyRelative(nameof(_inputProcessorValueStrings));

            position.height = EditorGUIUtility.singleLineHeight;
            using var stringCheck = new EditorGUI.ChangeCheckScope();
            var processorStrings = InputSystem.ListProcessors().ToArray();
            var processorTypes = Array.ConvertAll(processorStrings, processorString => InputSystem.TryGetProcessor(processorString));
            var index = Array.IndexOf(processorTypes, Type.GetType(_inputProcessorAssemblyQualifiedName.stringValue));

            var typeIndex = Array.IndexOf(processorTypes, Type.GetType(label.text));
            if (typeIndex != -1)
            {
                label.text = processorStrings[typeIndex];
            }

            index = EditorGUI.Popup(position, label, index, processorStrings.Select(processorString => new GUIContent(processorString)).ToArray());
            if (stringCheck.changed)
            {
                var processorType = processorTypes[index];
                _inputProcessorAssemblyQualifiedName.stringValue = processorType?.AssemblyQualifiedName;

                // Reset the processor values.
                for (int i = 0; i < _inputProcessorValueStrings.arraySize; i++)
                {
                    _inputProcessorValueStrings.GetArrayElementAtIndex(i).stringValue = null;
                }

                property.serializedObject.ApplyModifiedProperties();
            }

            var processor = ((PropertyMember)property).GetValue<Processor>();
            property.isExpanded = processor.inputProcessor?.GetType().GetFields().Length > 0
                && EditorGUI.Foldout(position, property.isExpanded, label, true);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                var processorFields = processor.inputProcessor?.GetType().GetFields();
                _inputProcessorValueStrings.arraySize = processorFields?.Length ?? 0;

                for (int i = 0; i < processorFields?.Length; i++)
                {
                    var processorField = processorFields[i];

                    var value = processorField.GetValue(processor.inputProcessor);
                    position.y += EditorGUIUtility.singleLineHeight + 2f;

                    switch (value)
                    {
                        case bool:
                            value = EditorGUI.Toggle(position, processorField.Name, (bool)value);
                            break;
                        case float:
                        case double:
                        case decimal:
                            value = EditorGUI.FloatField(position, processorField.Name, (float)value);
                            break;
                        case byte:
                        case sbyte:
                        case short:
                        case ushort:
                        case int:
                        case uint:
                            value = EditorGUI.IntField(position, processorField.Name, (int)value);
                            break;
                        case long:
                        case ulong:
                            value = EditorGUI.LongField(position, processorField.Name, (long)value);
                            break;
                        case char:
                        case string:
                            value = EditorGUI.TextField(position, processorField.Name, (string)value);
                            break;
                    }

                    _inputProcessorValueStrings.GetArrayElementAtIndex(i).stringValue = value.ToString();
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = base.GetPropertyHeight(property, label);

            if (property.isExpanded)
            {
                height += (EditorGUIUtility.singleLineHeight + 2f) * ((PropertyMember)property).GetValue<Processor>().inputProcessor?.GetType().GetFields().Length ?? 0;
            }

            return height;
        }
    }
}
