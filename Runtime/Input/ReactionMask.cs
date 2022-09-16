#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityExtras.InputSystem
{
    [Serializable]
    public struct ReactionMask : ISerializationCallbackReceiver, IEquatable<ReactionMask>
    {
        [field: SerializeField] public ReactionMethod defaultReaction { get; set; }

        [SerializeField] private string[] _interactionTypes;
        [SerializeField] private ReactionMethod[] _interactionReactions;

        private Dictionary<Type, ReactionMethod> _interactionTypeReactions { get; set; }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (_interactionTypeReactions != null)
            {
                _interactionTypes = _interactionTypeReactions.Keys.Select(interactionType => interactionType.AssemblyQualifiedName).ToArray();
                _interactionReactions = _interactionTypeReactions.Values.ToArray();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _interactionTypeReactions = new Dictionary<Type, ReactionMethod>();
            for (int i = 0; i < _interactionTypes.Length; i++)
            {
                _interactionTypeReactions.Add(Type.GetType(_interactionTypes[i]), _interactionReactions[i]);
            }
        }

        public ReactionMethod GetReaction<T>() where T : IInputInteraction => GetReaction(typeof(T));
        public ReactionMethod GetReaction(Type inputInteractionType)
        {
            if (!typeof(IInputInteraction).IsAssignableFrom(inputInteractionType))
            {
                throw new Exception();
            }

            return _interactionTypeReactions.TryGetValue(inputInteractionType, out var inputInteractionTrigger) 
                ? inputInteractionTrigger 
                : defaultReaction;
        }

        public void SetReaction<T>(ReactionMethod reaction) where T : IInputInteraction => SetReaction(reaction, typeof(T));
        public void SetReaction(ReactionMethod reaction, Type inputInteractionType)
        {
            if (!typeof(IInputInteraction).IsAssignableFrom(inputInteractionType))
            {
                throw new Exception();
            }

            _interactionTypeReactions[inputInteractionType] = reaction;
        }

        public void RemoveReaction<T>() where T : IInputInteraction => RemoveReaction(typeof(T));
        public void RemoveReaction(Type inputInteractionType)
        {
            if (!typeof(IInputInteraction).IsAssignableFrom(inputInteractionType))
            {
                throw new Exception();
            }

            _interactionTypeReactions.Remove(inputInteractionType);
        }

        public override bool Equals(object? obj) => obj is ReactionMask other && Equals(other);
        public bool Equals(ReactionMask other) => defaultReaction == other.defaultReaction 
            && _interactionTypeReactions.Count == other._interactionTypeReactions.Count 
            && !_interactionTypeReactions.Except(other._interactionTypeReactions).Any();
        public override int GetHashCode() => HashCode.Combine(defaultReaction, _interactionTypeReactions);
        public static bool operator ==(ReactionMask lhs, ReactionMask rhs) => lhs.Equals(rhs);
        public static bool operator !=(ReactionMask lhs, ReactionMask rhs) => !(lhs == rhs);
    }
}
