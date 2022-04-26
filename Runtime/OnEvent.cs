#nullable enable
using System;
using UnityEngine;
using UnityEngine.Events;

using Object = UnityEngine.Object;

namespace UnityExtras
{
    [AddComponentMenu("Event/On Event")]
    public class OnEvent : MonoBehaviour
    {
        [field: SerializeField] public EventTrigger eventTrigger { get; set; }
        [field: SerializeField] public bool checkTag { get; set; }
        [field: SerializeField] public string? collisionTag { get; set; } = "Untagged";
        [field: SerializeField][field: Min(0f)] public float collisionForce { get; set; }
        [field: SerializeField] public bool triggerOnce { get; set; }

        [field: SerializeField] public UnityEvent unityEvent { get; private set; } = new UnityEvent();
        [field: SerializeField] public UnityEvent<Collider> colliderEvent { get; private set; } = new UnityEvent<Collider>();
        [field: SerializeField] public UnityEvent<Collider2D> collider2DEvent { get; private set; } = new UnityEvent<Collider2D>();
        [field: SerializeField] public UnityEvent<Collision> collisionEvent { get; private set; } = new UnityEvent<Collision>();
        [field: SerializeField] public UnityEvent<Collision2D> collision2DEvent { get; private set; } = new UnityEvent<Collision2D>();

        [field: SerializeField] public bool sendDebugMessage { get; set; }

        public UnityEventBase? currentEvent => eventTrigger switch
        {
            var e when
                e == EventTrigger.OnTriggerEnter    || 
                e == EventTrigger.OnTriggerStay     ||
                e == EventTrigger.OnTriggerExit     => colliderEvent,
            var e when
                e == EventTrigger.OnTriggerEnter2D  ||
                e == EventTrigger.OnTriggerStay2D   ||
                e == EventTrigger.OnTriggerExit2D   => collider2DEvent,
            var e when
                e == EventTrigger.OnCollisionEnter  ||
                e == EventTrigger.OnCollisionStay   ||
                e == EventTrigger.OnCollisionExit   => collisionEvent,
            var e when
                e == EventTrigger.OnCollisionEnter2D    ||
                e == EventTrigger.OnCollisionStay2D     ||
                e == EventTrigger.OnCollisionExit2D     => collision2DEvent,
            EventTrigger.None => null,
            _ => unityEvent
        };

        public enum EventTrigger
        {
            None,
            Awake,
            Start,
            OnDestroy,
            OnEnable,
            OnDisable,
            OnTriggerEnter,
            OnTriggerStay,
            OnTriggerExit,
            OnTriggerEnter2D,
            OnTriggerStay2D,
            OnTriggerExit2D,
            OnCollisionEnter,
            OnCollisionStay,
            OnCollisionExit,
            OnCollisionEnter2D,
            OnCollisionStay2D,
            OnCollisionExit2D,
            MouseEnter,
            MouseExit,
            MouseDown,
            MouseUp,
        }

        private bool CheckTag(string? tag) => !checkTag || collisionTag == tag;

        private void Awake()
        {
            HandleGameEvent(EventTrigger.Awake);
        }

        private void Start()
        {
            HandleGameEvent(EventTrigger.Start);
        }

        private void OnDestroy()
        {
            HandleGameEvent(EventTrigger.OnDestroy);
        }

        private void OnEnable()
        {
            HandleGameEvent(EventTrigger.OnEnable);
        }

        private void OnDisable()
        {
            HandleGameEvent(EventTrigger.OnDisable);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (CheckTag(other.tag))
            {
                HandleGameEvent(EventTrigger.OnTriggerEnter, other);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (CheckTag(other.tag))
            {
                HandleGameEvent(EventTrigger.OnTriggerStay, other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (CheckTag(other.tag))
            {
                HandleGameEvent(EventTrigger.OnTriggerExit, other);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (CheckTag(other.tag))
            {
                HandleGameEvent(EventTrigger.OnTriggerEnter2D, other);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (CheckTag(other.tag))
            {
                HandleGameEvent(EventTrigger.OnTriggerStay2D, other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (CheckTag(other.tag))
            {
                HandleGameEvent(EventTrigger.OnTriggerExit2D, other);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (CheckTag(collision.transform.tag)
                && collision.relativeVelocity.sqrMagnitude >= collisionForce * collisionForce)
            {
                HandleGameEvent(EventTrigger.OnCollisionEnter, collision);
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (CheckTag(collision.transform.tag)
                && collision.relativeVelocity.sqrMagnitude >= collisionForce * collisionForce)
            {
                HandleGameEvent(EventTrigger.OnCollisionStay, collision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (CheckTag(collision.transform.tag)
                && collision.relativeVelocity.sqrMagnitude >= collisionForce * collisionForce)
            {
                HandleGameEvent(EventTrigger.OnCollisionExit, collision);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (CheckTag(collision.transform.tag)
                && collision.relativeVelocity.sqrMagnitude >= collisionForce * collisionForce)
            {
                HandleGameEvent(EventTrigger.OnCollisionEnter2D, collision);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (CheckTag(collision.transform.tag)
                && collision.relativeVelocity.sqrMagnitude >= collisionForce * collisionForce)
            {
                HandleGameEvent(EventTrigger.OnCollisionStay2D, collision);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (CheckTag(collision.transform.tag)
                && collision.relativeVelocity.sqrMagnitude >= collisionForce * collisionForce)
            {
                HandleGameEvent(EventTrigger.OnCollisionExit2D, collision);
            }
        }

        private void OnMouseEnter()
        {
            HandleGameEvent(EventTrigger.MouseEnter);
        }

        private void OnMouseExit()
        {
            HandleGameEvent(EventTrigger.MouseExit);
        }

        private void OnMouseDown()
        {
            HandleGameEvent(EventTrigger.MouseDown);
        }

        private void OnMouseUp()
        {
            HandleGameEvent(EventTrigger.MouseUp);
        }

        private void HandleGameEvent(EventTrigger gameEvent, object? obj = null)
        {
            if (eventTrigger != gameEvent || !this)
            {
                return;
            }

            switch (gameEvent)
            {
                case EventTrigger.OnTriggerEnter:
                case EventTrigger.OnTriggerStay:
                case EventTrigger.OnTriggerExit:
                    colliderEvent.Invoke((obj as Collider)!);
                    break;
                case EventTrigger.OnTriggerEnter2D:
                case EventTrigger.OnTriggerStay2D:
                case EventTrigger.OnTriggerExit2D:
                    collider2DEvent.Invoke((obj as Collider2D)!);
                    break;
                case EventTrigger.OnCollisionEnter:
                case EventTrigger.OnCollisionStay:
                case EventTrigger.OnCollisionExit:
                    collisionEvent.Invoke((obj as Collision)!);
                    break;
                case EventTrigger.OnCollisionEnter2D:
                case EventTrigger.OnCollisionStay2D:
                case EventTrigger.OnCollisionExit2D:
                    collision2DEvent.Invoke((obj as Collision2D)!);
                    break;
                default:
                    unityEvent.Invoke();
                    break;
            }

            if (sendDebugMessage)
            {
                Debug.Log($"{gameEvent} event invoked on {this}.");
            }

            if (triggerOnce && gameEvent != EventTrigger.OnDestroy)
            {
                Destroy(this);
            }
        }

        public new void Destroy(Object obj)
        {
            Object.Destroy(obj);
        }

        public new void DestroyImmediate(Object obj)
        {
            Object.DestroyImmediate(obj);
        }

        public new void DontDestroyOnLoad(Object target)
        {
            Object.DontDestroyOnLoad(target);
        }
    }
}
