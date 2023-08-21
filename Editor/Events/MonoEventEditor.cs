#nullable disable
using UnityEditor;
using UnityExtras.Editor;

namespace UnityExtras.Events.Editor
{
    [CustomEditor(typeof(MonoEvent))]
    [CanEditMultipleObjects]
    public class MonoEventEditor : UnityEditor.Editor
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

        private SerializedProperty currentEvent;

        private bool showTagFields = false;
        private bool showCollisionFields = false;

        private void OnEnable()
        {
            var monoEvent = target as MonoEvent;

            _eventTrigger = serializedObject.FindAutoProperty(nameof(monoEvent.eventTrigger));
            _checkTag = serializedObject.FindAutoProperty(nameof(monoEvent.checkTag));
            _collisionTag = serializedObject.FindAutoProperty(nameof(monoEvent.collisionTag));
            _collisionForce = serializedObject.FindAutoProperty(nameof(monoEvent.collisionForce));
            _triggerOnce = serializedObject.FindAutoProperty(nameof(monoEvent.triggerOnce));

            _unityEvent = serializedObject.FindAutoProperty(nameof(monoEvent.unityEvent));
            _colliderEvent = serializedObject.FindAutoProperty(nameof(monoEvent.colliderEvent));
            _collider2DEvent = serializedObject.FindAutoProperty(nameof(monoEvent.collider2DEvent));
            _collisionEvent = serializedObject.FindAutoProperty(nameof(monoEvent.collisionEvent));
            _collision2DEvent = serializedObject.FindAutoProperty(nameof(monoEvent.collision2DEvent));

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
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateVisibility()
        {
            switch (_eventTrigger.enumValueIndex)
            {
                case (int)MonoEvent.MonoEventTrigger.OnTriggerEnter:
                case (int)MonoEvent.MonoEventTrigger.OnTriggerStay:
                case (int)MonoEvent.MonoEventTrigger.OnTriggerExit:
                    showTagFields = true;
                    showCollisionFields = false;
                    currentEvent = _colliderEvent;
                    break;
                case (int)MonoEvent.MonoEventTrigger.OnTriggerEnter2D:
                case (int)MonoEvent.MonoEventTrigger.OnTriggerStay2D:
                case (int)MonoEvent.MonoEventTrigger.OnTriggerExit2D:
                    showTagFields = true;
                    showCollisionFields = false;
                    currentEvent = _collider2DEvent;
                    break;
                case (int)MonoEvent.MonoEventTrigger.OnCollisionEnter:
                case (int)MonoEvent.MonoEventTrigger.OnCollisionStay:
                case (int)MonoEvent.MonoEventTrigger.OnCollisionExit:
                    showTagFields = true;
                    showCollisionFields = true;
                    currentEvent = _collisionEvent;
                    break;
                case (int)MonoEvent.MonoEventTrigger.OnCollisionEnter2D:
                case (int)MonoEvent.MonoEventTrigger.OnCollisionStay2D:
                case (int)MonoEvent.MonoEventTrigger.OnCollisionExit2D:
                    showTagFields = true;
                    showCollisionFields = true;
                    currentEvent = _collision2DEvent;
                    break;
                case (int)MonoEvent.MonoEventTrigger.None:
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
