#nullable enable
using UnityEngine;

namespace UnityExtras
{
    /// <summary>A <see cref="CharacterController2D"/> allows you to easily do 2D movement constrained by collisions without having to deal with a <see cref="Rigidbody2D"/>.</summary>
    [AddComponentMenu("Physics 2D/Character Controller 2D")]
    [DisallowMultipleComponent]
    public class CharacterController2D : MonoBehaviour, IAuthor
    {
        [SerializeField][HideInInspector] private RequiredComponent<CapsuleCollider2D> _capsuleCollider2D;
        public CapsuleCollider2D capsuleCollider2D => _capsuleCollider2D.GetComponent(gameObject, HideFlags.HideInInspector);

        public static implicit operator CapsuleCollider2D(CharacterController2D characterController2D) => characterController2D.capsuleCollider2D;

        [SerializeField][Tooltip("The character controller2Ds slope limit in degrees.")] private float _slopeLimit = 45f;
        /// <summary>The character controller2Ds slope limit in degrees.</summary>
        public float slopeLimit
        {
            get => _slopeLimit;
            set => _slopeLimit = Mathf.Clamp(value, 0f, 180f);
        }

        [field: SerializeField][field: Tooltip("The character controller2Ds step offset in meters.")][field: Min(0f)] public float stepOffset { get; set; } = 0.3f;
        [field: SerializeField][field: Tooltip("The character's collision skin width.")][field: Min(0.0001f)] public float skinWidth { get; set; } = 0.08f;
        [field: SerializeField][field: Tooltip("Gets or sets the minimum move distance of the character controller2D.")][field: Min(0f)] public float minMoveDistance { get; set; } = 0.001f;
        
        [SerializeField][Tooltip("The center of the character's capsule relative to the transform's position.")] private Vector2 _center;
        /// <summary>The center of the character's capsule relative to the transform's position.</summary>
        public Vector2 center
        {
            get => capsuleCollider2D.offset;
            set => capsuleCollider2D.offset = _center = value;
        }
        
        [SerializeField][Tooltip("The radius of the character's capsule.")][Min(0f)] private float _radius = 0.5f;
        /// <summary>The radius of the character's capsule.</summary>
        public float radius
        {
            get => capsuleCollider2D.size.x * 0.5f;
            set => capsuleCollider2D.size = new Vector2((_radius = value) * 2f, capsuleCollider2D.size.y);
        }

        [SerializeField][Tooltip("The height of the character's capsule.")][Min(0f)] private float _height = 2f;
        /// <summary>The height of the character's capsule.</summary>
        public float height
        {
            get => capsuleCollider2D.size.y;
            set => capsuleCollider2D.size = new Vector2(capsuleCollider2D.size.x, _height = value);
        }

        /// <summary>What part(s) of the capsule collided with the environment during the last <see cref="Move"/> call.</summary>
        public CollisionFlags collisionFlags { get; private set; }

        private bool _detectCollisions = true;
        /// <summary>Determines whether other <see cref="Rigidbody2D"/> or <see cref="CharacterController2D"/> collide with this <see cref="CharacterController2D"/> (by default this is always enabled).</summary>
        public bool detectCollisions
        {
            get => _detectCollisions;
            set => capsuleCollider2D.enabled = (_detectCollisions = value) && enabled;
        }

        /// <summary>Was the <see cref="CharacterController2D"/> touching the ground during the last move?</summary>
        public bool isGrounded => collisionFlags.HasFlag(CollisionFlags.Below);

        /// <summary>The current relative velocity of the character.</summary>
        public Vector2 velocity { get; private set; }

        // public bool enableOverlapRecovery { get; set; } = true;
        private bool _enableOverlapRecovery = true;
        /// <summary>Enables or disables overlap recovery. Used to depenetrate <see cref="CharacterController2D"/> from static objects when an overlap is detected.</summary>
        public bool enableOverlapRecovery
        {
            get => _enableOverlapRecovery;
            set
            {
                _enableOverlapRecovery = value;
                Debug.LogWarning($"Horizontal {nameof(enableOverlapRecovery)} is not yet implemented and currently disabled. Only vertical {nameof(enableOverlapRecovery)} is available.", this);
            }
        }

        private RaycastHit2D[] _results = new RaycastHit2D[1];
        private RaycastHit2D hit => _results[0];

        public const float contactOffset = 0.01f;
        public float contactOffsetCompensation => Physics2D.defaultContactOffset - contactOffset;

        #region IAuthor Methods
        [field: SerializeField][field: HideInInspector] bool IAuthor.isDeserializing { get; set; }

        void IAuthor.Serialize()
        {
            _ = detectCollisions
                ? enabled = capsuleCollider2D.enabled
                : capsuleCollider2D.enabled = false;
            capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
            center = center;
            radius = radius;
            height = height;
        }

        void IAuthor.Deserialize()
        {
            capsuleCollider2D.enabled = detectCollisions && enabled;
            capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
            center = _center;
            radius = _radius;
            height = _height;
        }

        void IAuthor.DestroyAuthor()
        {
            ExtraObject.DestroyImmediateSafe(_capsuleCollider2D);
        }
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            capsuleCollider2D.enabled = detectCollisions;
        }

        private void OnDisable()
        {
            if (_capsuleCollider2D.component != null)
            {
                _capsuleCollider2D.component.enabled = false;
            }
        }

        private void Reset()
        {
            ((IAuthor)this).ResetAuthor();
        }

        private void OnDestroy()
        {
            ((IAuthor)this).DestroyAuthor();
        }

        private void OnValidate()
        {
            Debug.LogWarning($"Standardize hit results searching, taking IgnoreCollision into account.");
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Physics2D.alwaysShowColliders)
            {
                OnDrawGizmosSelected();
            }
        }

        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = detectCollisions ? Physics2D.colliderAwakeColor : Physics2D.colliderAsleepColor;

            // Compute direction vectors for our drawing positions.
            Vector3 horizontalOffset = radius * (Vector2)transform.right;
            var effectiveRadius = horizontalOffset.magnitude;
            Vector3 verticalOffset = height * 0.5f * (Vector2)transform.up - effectiveRadius * ((Vector2)transform.up).normalized;
            if ((Vector2)verticalOffset.normalized != ((Vector2)transform.up).normalized)
            {
                verticalOffset = Vector3.zero;
            }
            var forward = new Vector3(0f, 0f, transform.forward.z).normalized;
            horizontalOffset = effectiveRadius * (Quaternion.LookRotation(forward, verticalOffset) * Vector3.right);

            // Declare ease-of-use positions.
            var center = capsuleCollider2D.bounds.center;
            var head = center + verticalOffset;
            var feet = center - verticalOffset;

            // Draw Capsule
            UnityEditor.Handles.DrawWireArc(head, forward, horizontalOffset, 180f, effectiveRadius);
            UnityEditor.Handles.DrawWireArc(feet, forward, -horizontalOffset, 180f, effectiveRadius);
            UnityEditor.Handles.DrawAAPolyLine(head + horizontalOffset, feet + horizontalOffset);
            UnityEditor.Handles.DrawAAPolyLine(head - horizontalOffset, feet - horizontalOffset);

            // Draw AABB
            if (Physics2D.showColliderAABB)
            {
                Gizmos.color = Physics2D.colliderAABBColor;
                var AA = capsuleCollider2D.bounds.min;
                var BB = capsuleCollider2D.bounds.max;
                var AB = new Vector3(AA.x, BB.y, AA.z);
                var BA = new Vector3(BB.x, AA.y, AA.z);

                Gizmos.DrawLine(AA, AB);
                Gizmos.DrawLine(AB, BB);
                Gizmos.DrawLine(BB, BA);
                Gizmos.DrawLine(BA, AA);
            }
        }
#endif
        #endregion

        #region Controller Methods
        public bool Cast(Vector2 direction, RaycastHit2D[] results, float distance) => Cast(capsuleCollider2D.bounds.center, direction, results, distance);
        private bool Cast(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance)
        {
            capsuleCollider2D.enabled = false;
            var hasHit = Physics2D.CapsuleCastNonAlloc(origin, capsuleCollider2D.size - (contactOffsetCompensation * 2f) * Vector2.one, capsuleCollider2D.direction, transform.eulerAngles.z, direction, results, distance, Physics2D.GetLayerCollisionMask(gameObject.layer)) > 0;
            capsuleCollider2D.enabled = detectCollisions;
            return hasHit;
        }


        /// <summary>Supplies the movement of a <see cref="GameObject"/> with an attached <see cref="CharacterController2D"/> component.</summary>
        public CollisionFlags Move(Vector2 motion) => Move(motion, Vector2.right, Vector2.up, Space.World);
        /// <summary>Supplies the movement of a <see cref="GameObject"/> with an attached <see cref="CharacterController2D"/> component relative to its orientation.</summary>
        public CollisionFlags RelativeMove(Vector2 motion) => Move(motion, ((Vector2)transform.right).normalized, ((Vector2)transform.up).normalized, Space.Self);

        private CollisionFlags Move(Vector2 motion, Vector2 right, Vector2 up, Space translationSpace)
        {
            var newCollisionFlags = CollisionFlags.None;
            var deltaTime = Time.deltaTime;
            var deltaMinMoveDistance = minMoveDistance * deltaTime;

            // Move our character and check if we are walking over a step or slope while doing it.
            RaycastHit2D? horizontalHit = null;
            var stepHeight = Mathf.Abs(motion.x) > deltaMinMoveDistance ? Step() : 0f;

            // If we are walking over a step or slope, use a special algorithm to calculate our vertical motion. Otherwise, simply check for collision.
            RaycastHit2D? verticalHit = null;
            if (stepHeight != 0f)
            {
                var stepMotion = new Vector2(motion.x, stepHeight);
                var sign = (float)System.Math.Sign(motion.y);
                var collisionMotion = CollideFrom((Vector2)capsuleCollider2D.bounds.center + stepMotion, motion.y + stepMotion.y * sign, up, CollisionFlags.Above, CollisionFlags.Below, out verticalHit);

                // Only apply the step height if it is higher than what we're traveling already.
                stepHeight = stepMotion.y - collisionMotion * sign;

                var stepSign = Mathf.Sign(stepMotion.y);
                if (stepHeight * stepSign > motion.y * stepSign)
                {
                    motion.y = stepHeight;
                }
            }
            else
            {
                motion.y = Mathf.Abs(motion.y) > deltaMinMoveDistance ? Collide(motion.y, up, CollisionFlags.Above, CollisionFlags.Below, out verticalHit) : 0f;
            }

            // Send collision messages. WARNING: ORDER DEPENDENT!!!
            if (verticalHit != null && (newCollisionFlags.HasFlag(CollisionFlags.Below) || newCollisionFlags.HasFlag(CollisionFlags.Above)))
            {
                SendMessage($"On{nameof(ControllerColliderHit2D)}", new ControllerColliderHit2D(this, (RaycastHit2D)verticalHit), SendMessageOptions.DontRequireReceiver);
            }
            if (horizontalHit != null && newCollisionFlags.HasFlag(CollisionFlags.Sides))
            {
                SendMessage($"On{nameof(ControllerColliderHit2D)}", new ControllerColliderHit2D(this, (RaycastHit2D)horizontalHit), SendMessageOptions.DontRequireReceiver);
            }

            // Apply motion.
            transform.Translate(motion, translationSpace);
            velocity = deltaTime > 0f ? motion / deltaTime : default;

            return collisionFlags = newCollisionFlags;

            float Step()
            {
                // If we don't collide with anything (overlap doesn't count) take an early out.
                var horizontalMotion = motion.x;

                var tmp = enableOverlapRecovery;
                enableOverlapRecovery = false;
                motion.x = Collide(horizontalMotion, right, CollisionFlags.Sides, CollisionFlags.Sides, out horizontalHit);
                enableOverlapRecovery = tmp;
                if (!newCollisionFlags.HasFlag(CollisionFlags.Sides) || hit.distance == 0f)
                {
                    return 0f;
                }


                // Define the cast distance and the cast origin taking ceilings into account. If we don't hit something or are overlapping take an early out.
                var centroid = hit.centroid;
                var castDistance = Cast(up, _results, stepOffset) ? Mathf.Max(hit.distance - skinWidth, 0f) : stepOffset;
                var castOrigin = centroid + horizontalMotion * right + castDistance * up;
                if (!Cast(castOrigin, -up, _results, castDistance) || hit.distance == 0f)
                {
                    return 0f;
                }

                // Calculate the resulting motion of climbing the step / slope.
                var stepMotion = ExtraMath.Rotate2D(hit.centroid - centroid, -ExtraMath.Angle(right)) + skinWidth * up;

                // Perform a slope check using a separate ray for complete angle accurracy (casting with a capsule may produce incorrect normals). If the slope is too steep to climb take an early out.
                castOrigin += radius * Mathf.Sign(horizontalMotion) * right;
                capsuleCollider2D.enabled = false;
                var hasHit = Physics2D.RaycastNonAlloc(castOrigin, -up, _results, castDistance + height * 0.5f - skinWidth - contactOffsetCompensation, Physics2D.GetLayerCollisionMask(gameObject.layer)) > 0;
                capsuleCollider2D.enabled = detectCollisions;

                if (hasHit && Vector2.Angle(up, hit.normal) > slopeLimit + Vector2.kEpsilon)
                {
                    return 0f;
                }

                // Apply the horizontal motion and return the vertical motion for climbing the step / slope.
                newCollisionFlags &= ~CollisionFlags.Sides;
                motion.x = stepMotion.x;
                return stepMotion.y;
            }

            float Collide(float motion, Vector2 direction, in CollisionFlags positiveCollisionFlag, in CollisionFlags negativeCollisionFlag, out RaycastHit2D? raycastHit) => CollideFrom(capsuleCollider2D.bounds.center, motion, direction, positiveCollisionFlag, negativeCollisionFlag, out raycastHit);
            float CollideFrom(Vector2 origin, float motion, Vector2 direction, in CollisionFlags positiveCollisionFlag, in CollisionFlags negativeCollisionFlag, out RaycastHit2D? raycastHit)
            {
                raycastHit = null;

                // Check if we are colliding with anything.
                var sign = (float)System.Math.Sign(motion);
                if (sign == 0f || !Cast(origin, direction *= sign, _results, skinWidth + Mathf.Abs(motion)) || Vector2.Dot(hit.normal, direction) >= 0f)
                {
                    return motion;
                }

                // Check if we are overlapping with anything (and if we should recover from it).
                if (hit.distance == 0f)
                {
                    if (enableOverlapRecovery)
                    {
                        // Magic formula to find the contact offset-independent point of collision.
                        var point = hit.point + (contactOffset * 0.5f + 0.001f + contactOffsetCompensation) * direction;

                        // Resolve the overlap.
                        var pointDistance = (hit.centroid - point).magnitude;
                        if (pointDistance >= height * 0.25f - Vector2.kEpsilon)
                        {
                            var overlapDistance = (height * 0.5f - pointDistance) * 2f;
                            motion = (overlapDistance + skinWidth) * -sign;
                            newCollisionFlags |= sign == 1 ? positiveCollisionFlag : negativeCollisionFlag;
                        }
                    }

                    return motion;
                }

                // If we are colliding and not overlapping, send out a hit message, add the collision to the collisionFlags and return a motion that doesn't penetrate the collider.
                raycastHit = hit;
                newCollisionFlags |= sign == 1 ? positiveCollisionFlag : negativeCollisionFlag;
                return Mathf.Max(hit.distance - skinWidth, 0f) * sign;
            }
        }

        /// <summary>Moves the character with speed.</summary>
        public bool SimpleMove(Vector2 speed) => Move((speed + Physics2D.gravity) * Time.deltaTime).HasFlag(CollisionFlags.Below);
        #endregion
    }
}
