#nullable disable
using UnityEditor;

namespace UnityExtras.Editor
{
    [CustomEditor(typeof(OnEvent))]
    [CanEditMultipleObjects]
    public class OnEventEditor : UnityEditor.Editor
    {
        private SerializedProperty _eventTrigger;
        private SerializedProperty _checkTag;
        private SerializedProperty _collisionTag;
        private SerializedProperty _collisionForce;
        private SerializedProperty _triggerOnce;

        private SerializedProperty _unityEvent;
        private SerializedProperty _colliderEvent;
        private SerializedProperty _collider2DEvent;
        private SerializedProperty _collisionEvent;
        private SerializedProperty _collision2DEvent;

        private SerializedProperty _sendDebugMessage;

        private SerializedProperty currentEvent;

        private bool showTagFields = false;
        private bool showCollisionFields = false;

        private void OnEnable()
        {
            var onEvent = target as OnEvent;
            _eventTrigger = serializedObject.FindAutoProperty(nameof(onEvent.eventTrigger));
            _checkTag = serializedObject.FindAutoProperty(nameof(onEvent.checkTag));
            _collisionTag = serializedObject.FindAutoProperty(nameof(onEvent.collisionTag));
            _collisionForce = serializedObject.FindAutoProperty(nameof(onEvent.collisionForce));
            _triggerOnce = serializedObject.FindAutoProperty(nameof(onEvent.triggerOnce));

            _unityEvent = serializedObject.FindAutoProperty(nameof(onEvent.unityEvent));
            _colliderEvent = serializedObject.FindAutoProperty(nameof(onEvent.colliderEvent));
            _collider2DEvent = serializedObject.FindAutoProperty(nameof(onEvent.collider2DEvent));
            _collisionEvent = serializedObject.FindAutoProperty(nameof(onEvent.collisionEvent));
            _collision2DEvent = serializedObject.FindAutoProperty(nameof(onEvent.collision2DEvent));

            _sendDebugMessage = serializedObject.FindAutoProperty(nameof(onEvent.sendDebugMessage));

            UpdateVisibility();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_eventTrigger);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateVisibility();
            }

            if (currentEvent != null)
            {
                if (showTagFields)
                {
                    EditorGUILayout.PropertyField(_checkTag);
                    if (_checkTag.boolValue)
                    {
                        _collisionTag.stringValue = EditorGUILayout.TagField(_collisionTag.displayName, _collisionTag.stringValue);
                    }
                }

                if (showCollisionFields)
                {
                    EditorGUILayout.PropertyField(_collisionForce);
                }

                EditorGUILayout.PropertyField(_triggerOnce);
                EditorGUILayout.PropertyField(currentEvent);
                EditorGUILayout.PropertyField(_sendDebugMessage);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateVisibility()
        {
            switch (_eventTrigger.enumValueIndex)
            {
                case (int)OnEvent.EventTrigger.OnTriggerEnter:
                case (int)OnEvent.EventTrigger.OnTriggerStay:
                case (int)OnEvent.EventTrigger.OnTriggerExit:
                    showTagFields = true;
                    showCollisionFields = false;
                    currentEvent = _colliderEvent;
                    break;
                case (int)OnEvent.EventTrigger.OnTriggerEnter2D:
                case (int)OnEvent.EventTrigger.OnTriggerStay2D:
                case (int)OnEvent.EventTrigger.OnTriggerExit2D:
                    showTagFields = true;
                    showCollisionFields = false;
                    currentEvent = _collider2DEvent;
                    break;
                case (int)OnEvent.EventTrigger.OnCollisionEnter:
                case (int)OnEvent.EventTrigger.OnCollisionStay:
                case (int)OnEvent.EventTrigger.OnCollisionExit:
                    showTagFields = true;
                    showCollisionFields = true;
                    currentEvent = _collisionEvent;
                    break;
                case (int)OnEvent.EventTrigger.OnCollisionEnter2D:
                case (int)OnEvent.EventTrigger.OnCollisionStay2D:
                case (int)OnEvent.EventTrigger.OnCollisionExit2D:
                    showTagFields = true;
                    showCollisionFields = true;
                    currentEvent = _collision2DEvent;
                    break;
                case (int)OnEvent.EventTrigger.None:
                    showTagFields = false;
                    showCollisionFields = false;
                    currentEvent = null;
                    break;
                default:
                    showTagFields = false;
                    showCollisionFields = false;
                    currentEvent = _unityEvent;
                    break;
            }
        }
    }
}
