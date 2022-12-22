#nullable enable
using System;
using System.Reflection;

namespace UnityExtras
{
    [Serializable]
    public class ValueRef<T> : UnityMember
    {
        private static readonly ArgumentException _invalidMemberInfo = new($"{nameof(memberInfo)} was invalid. Make sure it is not null and of type {nameof(PropertyInfo)} or {nameof(FieldInfo)}.");

        public ValueRef() : this(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly) { }
        public ValueRef(BindingFlags bindingFlags) : this(MemberTypes.Field | MemberTypes.Property, bindingFlags, DisplayCheck) { }
        protected ValueRef(MemberTypes memberTypes, BindingFlags bindingFlags, Predicate<MemberInfo> displayCheck) : base(memberTypes, bindingFlags, displayCheck) { }

        private static bool DisplayCheck(MemberInfo member) => member switch
        {
            PropertyInfo property => property.CanRead && property.CanWrite && property.PropertyType is T,
            FieldInfo field => field.FieldType is T,
            _ => false,
        };

        public T value
        {
            get => (T)(memberInfo switch
            {
                PropertyInfo property => property.GetValue(target),
                FieldInfo field => field.GetValue(target),
                _ => throw _invalidMemberInfo
            });
            set
            {
                switch (memberInfo)
                {
                    case PropertyInfo property:
                        property.SetValue(target, value);
                        break;
                    case FieldInfo field:
                        field.SetValue(target, value);
                        break;
                    default:
                        throw _invalidMemberInfo;
                }
            }
        }

        public Type valueType => memberInfo switch
        {
            PropertyInfo property => property.PropertyType,
            FieldInfo field => field.FieldType,
            _ => throw _invalidMemberInfo
        };
    }

    [Serializable]
    public class ValueRef : ValueRef<object> 
    {
        public ValueRef() : base() { }
        public ValueRef(BindingFlags bindingFlags) : base(bindingFlags) { }
    }
}
