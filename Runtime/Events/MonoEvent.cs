#nullable enable
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace UnityExtras.Events
{
    /// <summary>Helper <see cref="Component"/> for quickly binding <see cref="Scene"/> functionality via <see cref="UnityEvent"/>.</summary>
    [AddComponentMenu("Event/Mono Event")]
    public class MonoEvent : MonoBehaviour
    {
        [field: SerializeField] public MonoEventTrigger eventTrigger { get; set; }
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
                e == MonoEventTrigger.OnTriggerEnter    || 
                e == MonoEventTrigger.OnTriggerStay     ||
                e == MonoEventTrigger.OnTriggerExit     => colliderEvent,
            var e when
                e == MonoEventTrigger.OnTriggerEnter2D  ||
                e == MonoEventTrigger.OnTriggerStay2D   ||
                e == MonoEventTrigger.OnTriggerExit2D   => collider2DEvent,
            var e when
                e == MonoEventTrigger.OnCollisionEnter  ||
                e == MonoEventTrigger.OnCollisionStay   ||
                e == MonoEventTrigger.OnCollisionExit   => collisionEvent,
            var e when
                e == MonoEventTrigger.OnCollisionEnter2D    ||
                e == MonoEventTrigger.OnCollisionStay2D     ||
                e == MonoEventTrigger.OnCollisionExit2D     => collision2DEvent,
            MonoEventTrigger.None => null,
            _ => unityEvent
        };

        public enum MonoEventTrigger
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
            HandleGameEvent(MonoEventTrigger.Awake);
        }

        private void Start()
        {
            HandleGameEvent(MonoEventTrigger.Start);
        }

        private void OnDestroy()
        {
            HandleGameEvent(MonoEventTrigger.OnDestroy);
        }

        private void OnEnable()
        {
            HandleGameEvent(MonoEventTrigger.OnEnable);
        }

        private void OnDisable()
        {
            HandleGameEvent(MonoEventTrigger.OnDisable);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (CheckTag(other.tag))
            {
                HandleGameEvent(MonoEventTrigger.OnTriggerEnter, other);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (CheckTag(other.tag))
            {
                HandleGameEvent(MonoEventTrigger.OnTriggerStay, other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (CheckTag(other.tag))
            {
                HandleGameEvent(MonoEventTrigger.OnTriggerExit, other);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (CheckTag(other.tag))
            {
                HandleGameEvent(MonoEventTrigger.OnTriggerEnter2D, other);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (CheckTag(other.tag))
            {
                HandleGameEvent(MonoEventTrigger.OnTriggerStay2D, other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (CheckTag(other.tag))
            {
                HandleGameEvent(MonoEventTrigger.OnTriggerExit2D, other);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (CheckTag(collision.transform.tag)
                && collision.relativeVelocity.sqrMagnitude >= collisionForce * collisionForce)
            {
                HandleGameEvent(MonoEventTrigger.OnCollisionEnter, collision);
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (CheckTag(collision.transform.tag)
                && collision.relativeVelocity.sqrMagnitude >= collisionForce * collisionForce)
            {
                HandleGameEvent(MonoEventTrigger.OnCollisionStay, collision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (CheckTag(collision.transform.tag)
                && collision.relativeVelocity.sqrMagnitude >= collisionForce * collisionForce)
            {
                HandleGameEvent(MonoEventTrigger.OnCollisionExit, collision);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (CheckTag(collision.transform.tag)
                && collision.relativeVelocity.sqrMagnitude >= collisionForce * collisionForce)
            {
                HandleGameEvent(MonoEventTrigger.OnCollisionEnter2D, collision);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (CheckTag(collision.transform.tag)
                && collision.relativeVelocity.sqrMagnitude >= collisionForce * collisionForce)
            {
                HandleGameEvent(MonoEventTrigger.OnCollisionStay2D, collision);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (CheckTag(collision.transform.tag)
                && collision.relativeVelocity.sqrMagnitude >= collisionForce * collisionForce)
            {
                HandleGameEvent(MonoEventTrigger.OnCollisionExit2D, collision);
            }
        }

        private void OnMouseEnter()
        {
            HandleGameEvent(MonoEventTrigger.MouseEnter);
        }

        private void OnMouseExit()
        {
            HandleGameEvent(MonoEventTrigger.MouseExit);
        }

        private void OnMouseDown()
        {
            HandleGameEvent(MonoEventTrigger.MouseDown);
        }

        private void OnMouseUp()
        {
            HandleGameEvent(MonoEventTrigger.MouseUp);
        }

        private void HandleGameEvent(MonoEventTrigger gameEvent, object? obj = null)
        {
            if (eventTrigger != gameEvent || !this)
            {
                return;
            }

            switch (gameEvent)
            {
                case MonoEventTrigger.OnTriggerEnter:
                case MonoEventTrigger.OnTriggerStay:
                case MonoEventTrigger.OnTriggerExit:
                    colliderEvent.Invoke((obj as Collider)!);
                    break;
                case MonoEventTrigger.OnTriggerEnter2D:
                case MonoEventTrigger.OnTriggerStay2D:
                case MonoEventTrigger.OnTriggerExit2D:
                    collider2DEvent.Invoke((obj as Collider2D)!);
                    break;
                case MonoEventTrigger.OnCollisionEnter:
                case MonoEventTrigger.OnCollisionStay:
                case MonoEventTrigger.OnCollisionExit:
                    collisionEvent.Invoke((obj as Collision)!);
                    break;
                case MonoEventTrigger.OnCollisionEnter2D:
                case MonoEventTrigger.OnCollisionStay2D:
                case MonoEventTrigger.OnCollisionExit2D:
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

            if (triggerOnce && gameEvent != MonoEventTrigger.OnDestroy)
            {
                Destroy(this);
            }
        }

        /// <summary>Removes a <see cref="GameObject"/>, <see cref="Component"/> or Asset.</summary>
        /// <param name="obj"><see cref="Object"/> to destroy.</param>
        public new void Destroy(Object obj)
        {
            Object.Destroy(obj);
        }

        /// <summary>Destroys the <see cref="Object"/> <paramref name="obj"/> immediately. You are strongly recommended to use <see cref="Destroy"/> instead.</summary>
        /// <param name="obj"><see cref="Object"/> to destroy.</param>
        public new void DestroyImmediate(Object obj)
        {
            Object.DestroyImmediate(obj);
        }

        /// <summary>Do not destroy the target <see cref="Object"/> when loading a new <see cref="Scene"/>.</summary>
        /// <param name="target">An <see cref="Object"/> not destroyed on <see cref="Scene"/> change.</param>
        public new void DontDestroyOnLoad(Object target)
        {
            Object.DontDestroyOnLoad( target);
        }
    }
}
