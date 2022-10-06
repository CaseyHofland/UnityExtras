#nullable enable
using System;
using System.Reflection;
using UnityEngine;

namespace UnityExtras
{
    [Serializable]
    public class ValueRef<T>
    {
        [field: SerializeField] public UnityMember member { get; private set; } = new UnityMember(MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly, DisplayCheck);

        private static bool DisplayCheck(MemberInfo member) => member switch
        {
            PropertyInfo property => property.CanRead && property.CanWrite && property.PropertyType == typeof(T),
            FieldInfo field => field.FieldType == typeof(T),
            _ => false,
        };

        public T value
        {
            get => (T)(member.memberInfo switch
            {
                PropertyInfo property => property.GetValue(member.target),
                FieldInfo field => field.GetValue(member.target),
                _ => throw new ArgumentException($"{member.memberInfo} was invalid. Make sure it is not null and of type PropertyInfo or FieldInfo."),
            });
            set
            {
                switch(member.memberInfo)
                {
                    case PropertyInfo property:
                        property.SetValue(member.target, value);
                        break;
                    case FieldInfo field:
                        field.SetValue(member.target, value);
                        break;
                    default:
                        throw new ArgumentException($"{member.memberInfo} was invalid. Make sure it is not null and of type PropertyInfo or FieldInfo.");
                }
            }
        }
    }
}
