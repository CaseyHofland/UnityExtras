#nullable enable
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityExtras.Editor
{
    [CustomEditor(typeof(CharacterMover), true)]
    [CanEditMultipleObjects]
    public class CharacterMoverEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            if (targets?.Length <= 1)
            {
                var characterMover = (CharacterMover)target;

                // Warn for a non-zero min move distance because it can make things like gravity, speeding up, slowing down jittery and incorrect.
                {
                    var minMoveHelpBox = new HelpBox($"It is recommended to set the {nameof(CharacterController)}'s {nameof(CharacterController.minMoveDistance)} to 0 to ensure all movement is accurately registered.", HelpBoxMessageType.Info);
                    root.Add(minMoveHelpBox);

                    CheckIfShouldDisplay();
                    root.TrackSerializedObjectValue(new SerializedObject(characterMover.characterController), _ => CheckIfShouldDisplay());

                    void CheckIfShouldDisplay() => minMoveHelpBox.style.display = characterMover.characterController.minMoveDistance != 0f ? DisplayStyle.Flex : DisplayStyle.None;
                }

                // Warn that trigger- and collision events will not fire correctly without a Rigidbody attached.
                if (!characterMover.TryGetComponent(out Rigidbody rigidbody))
                {
                    var rigidbodyHelpBox = new HelpBox($"Is is strongly recommended to add a kinematic {nameof(Rigidbody)}.Trigger- and collision events will not fire correctly without a {nameof(Rigidbody)} attached.", HelpBoxMessageType.Info);
                    root.Add(rigidbodyHelpBox);
                }
            }

            return root;
        }
    }
}
