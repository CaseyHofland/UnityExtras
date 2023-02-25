#nullable enable
using System;
using System.Reflection;
using UnityEditor;

namespace UnityExtras.Editor
{
    public class PropertyMember
    {
        public readonly PropertyMember? parent;

        public readonly MemberInfo memberInfo;
        public readonly object target;
        public readonly int index = -1;

        private ArgumentException invalidMemberInfoException => new($"{memberInfo} was invalid. Should be of type {typeof(PropertyInfo)} or {typeof(FieldInfo)}, but was of type {memberInfo?.GetType().ToString() ?? "NULL"}.");

        public PropertyMember(MemberInfo memberInfo, object target)
        {
            this.memberInfo = memberInfo;
            this.target = target;
        }
        public PropertyMember(PropertyMember? parent, MemberInfo memberInfo, object target) : this(memberInfo, target) => this.parent = parent;
        public PropertyMember(MemberInfo memberInfo, object target, int index) : this(memberInfo, target) => this.index = index;
        public PropertyMember(PropertyMember? parent, MemberInfo memberInfo, object target, int index) : this(memberInfo, target, index) => this.parent = parent;

        public object GetValue() => memberInfo switch
        {
            PropertyInfo propertyInfo => (index == -1 ? propertyInfo.GetValue(target) : propertyInfo.GetValue(target, new object[] { index })),
            FieldInfo fieldInfo => fieldInfo.GetValue(target),
            _ => throw invalidMemberInfoException,
        };

        public T GetValue<T>() => (T)GetValue();

        public void SetValue<T>(T value)
        {
            switch (memberInfo)
            {
                case PropertyInfo propertyInfo:
                    if (index == -1)
                    {
                        propertyInfo.SetValue(target, value);
                    }
                    else
                    {
                        propertyInfo.SetValue(target, value, new object[] { index });
                    }
                    break;
                case FieldInfo fieldInfo:
                    fieldInfo.SetValue(target, value);
                    break;
                default:
                    throw invalidMemberInfoException;
            }

            parent?.SetValue(target);
        }

        public bool TryGetPropertyInfo(out PropertyInfo? propertyInfo) => (propertyInfo = memberInfo is PropertyInfo tmp ? tmp : null) != null;
        public bool TryGetFieldInfo(out FieldInfo? fieldInfo) => (fieldInfo = memberInfo is FieldInfo tmp ? tmp : null) != null;
        public Type GetMemberType() => memberInfo switch
        {
            PropertyInfo propertyInfo => propertyInfo.PropertyType,
            FieldInfo fieldInfo => fieldInfo.FieldType,
            _ => throw invalidMemberInfoException,
        };

        public static explicit operator PropertyMember(SerializedProperty property) => property.GetPropertyMember();
    }
}
