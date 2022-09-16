#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityExtras.InputSystem
{
    [Serializable]
    public struct InputReaction : ISerializationCallbackReceiver
    {
        public Input input;
        [SerializeField][Tooltip("The List<InputProcessor> attached as Reprocessors to the InputAction. null if the InputAction is null.")] private List<Processor> _processors;
        [SerializeField][Tooltip("The ReactionMask attached to the Reaction. default if the Reaction is null.")] private ReactionMask _reactionMask;

        /// <summary>The <see cref="List{InputProcessor}">List</see>&lt;<see cref="InputProcessor"/>&gt; attached as Reprocessors to the <see cref="InputAction"/>. <see langword="null"/> if the <see cref="InputAction"/> is <see langword="null"/>.</summary>
        public List<InputProcessor>? inputProcessors
        {
            get => input.action?.Reprocessors();
            set
            {
                var reprocessors = input.action?.Reprocessors();
                if (reprocessors != null)
                {
                    reprocessors.Clear();
                    if (value != null)
                    {
                        reprocessors.AddRange(value);
                    }

                    _processors = reprocessors.Select(reprocessor => (Processor)reprocessor).ToList();
                }
            }
        }

        /// <summary>The <see cref="ReactionMask"/> attached to the <see cref="Reaction"/>. <see langword="default"/> if the <see cref="Reaction"/> is <see langword="null"/>.</summary>
        public ReactionMask reactionMask
        {
            get => reaction?.reactionMask ?? default;
            set
            {
                if (reaction != null)
                {
                    reaction.reactionMask = _reactionMask = value;
                }
            }
        }

        /// <summary>The <see cref="Reaction"/> attached to the <see cref="InputAction"/>. <see langword="null"/> if the <see cref="InputAction"/> is <see langword="null"/>.</summary>
        public Reaction? reaction => input.action?.Reaction();

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            inputProcessors = _processors.Select(processor => (InputProcessor)processor!).ToList();
            reactionMask = _reactionMask;
        }
    }
}
