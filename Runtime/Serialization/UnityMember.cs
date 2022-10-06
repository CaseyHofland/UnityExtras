#nullable enable
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityExtras
{
    [Serializable]
    public class UnityMember : ISerializationCallbackReceiver
    {
        [SerializeField] private Object? _target;
        [SerializeField] private string? _moduleName;
        [SerializeField] private int _metadataToken;
        [SerializeField] private MemberTypes _memberTypes = MemberTypes.All;
        [SerializeField] private BindingFlags _bindingFlags;

        public Predicate<MemberInfo>? displayCheck { get; private set; }

        public UnityMember() { }

        public UnityMember(BindingFlags bindingFlags)
        {
            _bindingFlags = bindingFlags;
        }

        public UnityMember(Predicate<MemberInfo> displayCheck)
        {
            this.displayCheck = displayCheck;
        }

        public UnityMember(MemberTypes memberTypes, BindingFlags bindingFlags) : this(bindingFlags)
        {
            _memberTypes = memberTypes;
        }

        public UnityMember(BindingFlags bindingFlags, Predicate<MemberInfo> displayCheck) : this(bindingFlags)
        {
            this.displayCheck = displayCheck;
        }

        public UnityMember(MemberTypes memberTypes, BindingFlags bindingFlags, Predicate<MemberInfo> displayCheck) : this(memberTypes, bindingFlags)
        {
            this.displayCheck = displayCheck;
        }

        public object? target { get; private set; }
        public MemberInfo? memberInfo { get; private set; }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _target = (Object?)target;
            _moduleName = memberInfo?.Module.FullyQualifiedName;
            _metadataToken = memberInfo?.MetadataToken ?? default;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            target = _target;
            var members = _bindingFlags == BindingFlags.Default ? target?.GetType().GetMembers() : target?.GetType().GetMembers(_bindingFlags);
            memberInfo = members?.Length switch
            {
                default(int) => null,
                1 => members[0],
                _ => members.FirstOrDefault(member => member.MetadataToken == _metadataToken && member.Module.FullyQualifiedName == _moduleName),
            };
        }
    }

    public class DisplayMemberTypesAttribute : PropertyAttribute { }
    public class DisplayBindingsFlagsAttribute : PropertyAttribute { }
}