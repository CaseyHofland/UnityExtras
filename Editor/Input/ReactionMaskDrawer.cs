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

    [CustomPropertyDrawer(typeof(ReactionMask))]
    public class ReactionMaskDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw Button
            var defaultReaction = property.FindAutoPropertyRelative(nameof(ReactionMask.defaultReaction));

            var reactionDisplayNames = defaultReaction.enumDisplayNames;
            var reactionIndex = defaultReaction.enumValueIndex;

            position = EditorGUI.PrefixLabel(position, label);
            var buttonStyle = new GUIStyle("DropDownButton")
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(4, 4, 0, 0)
            };
            if (GUI.Button(position, reactionDisplayNames[reactionIndex], buttonStyle))
            {
                // Populate Enum Data.
                var menu = new GenericMenu();

                for (int i = 0; i < reactionDisplayNames.Length; i++)
                {
                    var triggerString = reactionDisplayNames[i];
                    menu.AddItem(new GUIContent(triggerString), i == reactionIndex, (object userData) =>
                    {
                        defaultReaction.enumValueIndex = (int)userData;
                        property.serializedObject.ApplyModifiedProperties();
                    }, i);
                }

                // Populate Mask Data.
                menu.AddSeparator(string.Empty);

                SerializedProperty _interactionTypes = property.FindPropertyRelative(nameof(_interactionTypes));
                SerializedProperty _interactionReactions = property.FindPropertyRelative(nameof(_interactionReactions));

                var interactionNames = InputSystem.ListInteractions().ToArray();
                var interactionTypes = Array.ConvertAll(interactionNames, interactionName => InputSystem.TryGetInteraction(interactionName));

                var addedInteractionTypes = _interactionTypes.GetPropertyMember().GetValue<string[]>();

                for (int i = 0; i < interactionNames.Length; i++)
                {
                    var interactionName = interactionNames[i];
                    var interactionType = interactionTypes[i].AssemblyQualifiedName;

                    var elementIndex = addedInteractionTypes != null ? Array.IndexOf(addedInteractionTypes, interactionType) : -1;

                    for (int j = 0; j < reactionDisplayNames.Length; j++)
                    {
                        var reactionDisplayName = reactionDisplayNames[j];

                        var path = interactionName + "/" + reactionDisplayName;

                        menu.AddItem(new GUIContent(path), elementIndex != -1 && _interactionReactions.GetArrayElementAtIndex(elementIndex).enumValueIndex == j, (object userData) =>
                        {
                            var userDataArray = (object[])userData;
                            var i = (int)userDataArray[0];
                            var interactionType = interactionTypes[i].AssemblyQualifiedName;
                            var reactionIndex = (int)userDataArray[1];
                            var elementIndex = (int)userDataArray[2];

                            if (elementIndex == -1)
                            {
                                _interactionTypes.InsertArrayElementAtIndex(0);
                                _interactionReactions.InsertArrayElementAtIndex(0);

                                _interactionTypes.GetArrayElementAtIndex(0).stringValue = interactionType;
                                _interactionReactions.GetArrayElementAtIndex(0).enumValueIndex = reactionIndex;
                            }
                            else if (_interactionReactions.GetArrayElementAtIndex(elementIndex).enumValueIndex == reactionIndex)
                            {
                                _interactionTypes.DeleteArrayElementAtIndex(elementIndex);
                                _interactionReactions.DeleteArrayElementAtIndex(elementIndex);
                            }
                            else
                            {
                                _interactionTypes.GetArrayElementAtIndex(elementIndex).stringValue = interactionType;
                                _interactionReactions.GetArrayElementAtIndex(elementIndex).enumValueIndex = reactionIndex;
                            }

                            property.serializedObject.ApplyModifiedProperties();
                        }, new object[] { i, j, elementIndex });
                    }
                }

                menu.DropDown(position);
            }

            EditorGUI.EndProperty();
        }
    }
}
