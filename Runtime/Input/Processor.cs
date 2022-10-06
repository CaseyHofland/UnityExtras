#nullable enable
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityExtras.InputSystem
{
    [Serializable]
    public struct Processor
    {
        [SerializeField] private string? _inputProcessorAssemblyQualifiedName;
        [SerializeField] private string[]? _inputProcessorValueStrings;

        public InputProcessor? inputProcessor
        {
            get
            {
                Type? processorType;
                if (string.IsNullOrEmpty(_inputProcessorAssemblyQualifiedName) || (processorType = Type.GetType(_inputProcessorAssemblyQualifiedName)) == null)
                {
                    return null;
                }

                var inputProcessor = (InputProcessor)Activator.CreateInstance(processorType);

                var processorFields = processorType.GetFields();
                for (int i = 0; i < processorFields.Length && i < _inputProcessorValueStrings?.Length; i++)
                {
                    var processorField = processorFields[i];
                    var processorValueString = _inputProcessorValueStrings[i];

                    try
                    {
                        var processorValue = Convert.ChangeType(processorValueString, processorField.FieldType);
                        processorField.SetValue(inputProcessor, processorValue);
                    }
                    catch (Exception) { }
                }

                return inputProcessor;
            }
            set
            {
                var processorType = value?.GetType();
                _inputProcessorAssemblyQualifiedName = processorType?.AssemblyQualifiedName;

                var processorFields = processorType?.GetFields();
                _inputProcessorValueStrings = new string[processorFields?.Length ?? 0];
                for (int i = 0; i < processorFields?.Length; i++)
                {
                    var processorField = processorFields[i];
                    _inputProcessorValueStrings[i] = processorField.GetValue(value).ToString();
                }
            }
        }

        public static implicit operator InputProcessor?(Processor processor) => processor.inputProcessor;
        public static implicit operator Processor(InputProcessor? inputProcessor) => new() { inputProcessor = inputProcessor };
    }
}
