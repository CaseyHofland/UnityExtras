#nullable enable
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityExtras.Editor
{
    [CustomPropertyDrawer(typeof(UnityMember), true)]
    public class UnityMemberDrawer : PropertyDrawer
    {
        private const string noFunction = "No Function";

        public readonly ReorderableList reorderableList = new(new Object[1], typeof(Object), false, true, false, false);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            reorderableList.ClearSelection();
            reorderableList.drawHeaderCallback = (rect) => GUI.Label(rect, label);
            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) => DrawAction(rect, property);
            reorderableList.elementHeightCallback = (index) => fieldInfo.GetCustomAttribute<DisplayMemberTypesAttribute>(true) == null && fieldInfo.GetCustomAttribute<DisplayBindingsFlagsAttribute>(true) == null
                ? EditorGUIUtility.singleLineHeight
                : EditorGUIUtility.singleLineHeight * 2 + 2f;
            reorderableList.DoList(position);

            EditorGUI.EndProperty();
        }

        protected virtual void DrawAction(Rect position, SerializedProperty property)
        {
            SerializedProperty _target = property.FindPropertyRelative(nameof(_target));
            SerializedProperty _moduleName = property.FindPropertyRelative(nameof(_moduleName));
            SerializedProperty _metadataToken = property.FindPropertyRelative(nameof(_metadataToken));

            SerializedProperty _memberTypes = property.FindPropertyRelative(nameof(_memberTypes));
            SerializedProperty _bindingFlags = property.FindPropertyRelative(nameof(_bindingFlags));

            position.width /= 3;
            position.height = EditorGUIUtility.singleLineHeight;
            using (var targetChangeCheck = new EditorGUI.ChangeCheckScope())
            {
                EditorGUI.PropertyField(position, _target, GUIContent.none, true);
                if (targetChangeCheck.changed)
                {
                    Unselected();
                }
            }

            if (fieldInfo.GetCustomAttribute<DisplayMemberTypesAttribute>(true) != null)
            {
                position.y += EditorGUIUtility.singleLineHeight + 2f;
                EditorGUI.PropertyField(position, _memberTypes, GUIContent.none, true);
                position.y -= EditorGUIUtility.singleLineHeight + 2f;
            }

            position.x += position.width + 2f;
            position.width *= 2f;
            position.width -= 2f;
            using (var dropdownDisabledGroup = new EditorGUI.DisabledGroupScope(_target.objectReferenceValue == null))
            {
                var unityMember = property.GetValue<UnityMember>();

                var dropdownContent = _target.objectReferenceValue != null && unityMember.memberInfo != null
                    ? new GUIContent($"{_target.objectReferenceValue.GetType().Name}.{unityMember.memberInfo.Name}")
                    : new GUIContent(noFunction);
                if (EditorGUI.DropdownButton(position, dropdownContent, FocusType.Passive, EditorStyles.popup))
                {
                    BuildPopulList(_target.objectReferenceValue!, string.IsNullOrEmpty(_moduleName.stringValue) && _metadataToken.intValue == default, Unselected, (MemberTypes)_memberTypes.enumValueFlag, (BindingFlags)_bindingFlags.enumValueFlag, ValidMember, On, Selected).DropDown(position);

                    bool ValidMember(MemberInfo member)
                    {
                        var valid = UnityMemberDrawer.ValidMember(member);
                        return valid && (unityMember?.displayCheck?.Invoke(member) ?? true);
                    }
                }
            }

            if (fieldInfo.GetCustomAttribute<DisplayBindingsFlagsAttribute>(true) != null)
            {
                position.y += EditorGUIUtility.singleLineHeight + 2f;
                EditorGUI.PropertyField(position, _bindingFlags, GUIContent.none, true);
                position.y -= EditorGUIUtility.singleLineHeight + 2f;
            }

            bool On(MemberInfo memberInfo) => memberInfo.Module.FullyQualifiedName == _moduleName.stringValue && memberInfo.MetadataToken == _metadataToken.intValue;
            
            void Unselected()
            {
                _moduleName.stringValue = default;
                _metadataToken.intValue = default;
            }

            void Selected(object target, MemberInfo memberInfo)
            {
                _target.objectReferenceValue = (Object)target;
                _moduleName.stringValue = memberInfo.Module.FullyQualifiedName;
                _metadataToken.intValue = memberInfo.MetadataToken;
                _target.serializedObject.ApplyModifiedProperties();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => reorderableList.GetHeight() - reorderableList.footerHeight;

        private static GenericMenu BuildPopulList(object target, bool off, Action unselected, MemberTypes memberTypes, BindingFlags bindingFlags, Predicate<MemberInfo> valid, Predicate<MemberInfo> on, Action<object, MemberInfo> selected)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent(noFunction), off, () => unselected());

            if (target == null)
            {
                return menu;
            }

            menu.AddSeparator(string.Empty);

            if (target is Component component)
            {
                target = component.gameObject;
            }

            GenerateMembersPopup(menu, target, memberTypes, bindingFlags, valid, on, selected, target.GetType().Name);
            if (target is GameObject gameObject)
            {
                var components = gameObject.GetComponents<Component>().AsSpan();
                for (int i = 0; i < components.Length; i++)
                {
                    component = components[i];
                    var componentType = component.GetType();

                    int componentCount = 0;
                    var componentsSlice = components[..i];
                    for (int j = 0; j < componentsSlice.Length; j++)
                    {
                        if (componentsSlice[j].GetType() == componentType)
                        {
                            componentCount++;
                        }
                    }

                    var subMenu = componentType.Name;
                    if (componentCount > 0)
                    {
                        subMenu += $" ({componentCount})";
                    }
                    GenerateMembersPopup(menu, component, memberTypes, bindingFlags, valid, on, selected, subMenu);
                }
            }

            return menu;
        }

        private static bool ValidMember(MemberInfo member)
        {
            return !(member is MethodBase methodBase && methodBase.IsSpecialName
                || member is FieldInfo field && field.IsSpecialName
                || member is PropertyInfo property && property.IsSpecialName
                || member is EventInfo @event && @event.IsSpecialName
                || member.GetCustomAttribute<ObsoleteAttribute>(true) != null);
        }

        private static void GenerateMembersPopup(GenericMenu menu, object target, MemberTypes memberTypes, BindingFlags bindingFlags, Predicate<MemberInfo> valid, Predicate<MemberInfo> on, Action<object, MemberInfo> selected, string subMenu)
        {
            if (subMenu != string.Empty)
            {
                subMenu += '/';
            }

            var targetType = target.GetType();
            var members = bindingFlags == BindingFlags.Default ? targetType.GetMembers() : targetType.GetMembers(bindingFlags);
            members = Array.FindAll(members, member => memberTypes.HasFlag(member.MemberType));

            MemberTypes currentMemberType = 0;
            foreach (var member in members)
            {
                {
                    if (member is MethodBase methodBase && methodBase.IsSpecialName
                        || member is FieldInfo field && field.IsSpecialName
                        || member is PropertyInfo property && property.IsSpecialName
                        || member is EventInfo @event && @event.IsSpecialName
                        || member.GetCustomAttribute<ObsoleteAttribute>(true) != null
                        || !valid(member))
                    {
                        continue;
                    }
                }

                if (member.MemberType != currentMemberType)
                {
                    if (currentMemberType != 0)
                    {
                        menu.AddItem(new GUIContent(subMenu + " "), false, null);
                    }
                    currentMemberType = member.MemberType;
                    menu.AddDisabledItem(new GUIContent(subMenu + $"{currentMemberType} Members"), false);
                }

                using CSharpCodeProvider cSharpCodeProvider = new();
                string GetTypeOutput(Type type)
                {
                    var typeOutput = cSharpCodeProvider.GetTypeOutput(new CodeTypeReference(type));
                    return Regex.Replace(typeOutput, @"[^<]*?[?=\.]", string.Empty);
                }

                var text = new StringBuilder(member.Name);
                switch (member)
                {
                    case MethodBase methodBase:
                        var parameters = methodBase.GetParameters();
                        var parameterTypeNames = Array.ConvertAll(parameters, parameter => GetTypeOutput(parameter.ParameterType));

                        text.Append('(');
                        text.AppendJoin(", ", parameterTypeNames);
                        text.Append(')');

                        if (methodBase is MethodInfo method)
                        {
                            text.Insert(0, ' ');
                            text.Insert(0, GetTypeOutput(method.ReturnType));
                        }
                        break;
                    case FieldInfo field:
                        text.Insert(0, ' ');
                        text.Insert(0, GetTypeOutput(field.FieldType));
                        break;
                    case PropertyInfo property:
                        text.Insert(0, ' ');
                        text.Insert(0, GetTypeOutput(property.PropertyType));
                        break;
                    case EventInfo @event:
                        text.Insert(0, ' ');
                        text.Insert(0, GetTypeOutput(@event.EventHandlerType));
                        break;
                    case Type type:
                        text.Insert(0, ' ');
                        text.Insert(0, GetTypeOutput(type.BaseType));
                        break;
                }
                text.Insert(0, subMenu);

                menu.AddItem(
                    new GUIContent(text.ToString()),
                    on(member),
                    (userData) =>
                    {
                        var (target, member) = ((object, MemberInfo))userData;
                        selected(target, member);
                    },
                    (target, member)
                );
            }
        }
    }
}