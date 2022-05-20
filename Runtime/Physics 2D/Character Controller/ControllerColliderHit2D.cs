#nullable enable
using UnityEngine;

namespace UnityExtras
{
    public class ControllerColliderHit2D
    {
        /// <summary>The Collider2D that was hit by the CharacterController2D.</summary>
        public Collider2D? collider { get; }

        /// <summary>The CharacterController2D that hit the Collider2D.</summary>
        public CharacterController2D controller { get; }

        /// <summary>The game object that was hit by the CharacterController2D.</summary>
        public GameObject gameObject { get; }

        /// <summary>The direction that the CharacterController2D was moving in when the collision occured.</summary>
        public Vector2 moveDirection { get; }

        /// <summary>How far the CharacterController2D has travelled until it hit the Collider2D.</summary>
        public float moveLength { get; }

        /// <summary>The normal of the surface we collided with in world space.</summary>
        public Vector2 normal { get; }

        /// <summary>The impact point in world space.</summary>
        public Vector2 point { get; }

        /// <summary>The Rigidbody2D that was hit by the CharacterController2D.</summary>
        public Rigidbody2D? rigidbody { get; }

        /// <summary>The transform that was hit by the CharacterController2D.</summary>
        public Transform transform { get; }

        public ControllerColliderHit2D(CharacterController2D characterController2D, RaycastHit2D raycastHit2D)
        {
            collider = raycastHit2D.collider;
            controller = characterController2D;
            gameObject = collider.gameObject;
            moveDirection = characterController2D.velocity.normalized;
            moveLength = raycastHit2D.distance;
            normal = raycastHit2D.normal;
            point = raycastHit2D.point;
            rigidbody = raycastHit2D.rigidbody;
            transform = raycastHit2D.transform;
        }
    }
}
