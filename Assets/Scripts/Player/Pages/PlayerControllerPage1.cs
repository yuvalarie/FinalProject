using System;
using Managers;
using Transitions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerControllerPage1 : PlayerControllerBase
    {
        [Tooltip("How many seconds after teleporting before the player can teleport again.")]
        [SerializeField] private float transitionCooldown = 0.2f;
        [SerializeField] private Vector3 startPosition;
        private bool _canMove;
        
        protected override void Start()
        {
            base.Start();
        }
        
        protected override void HandleMovement()
        {
            if (!_canMove)
            {
                Rb.linearVelocity = Vector2.zero;
                return;
            }
            base.HandleMovement();
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other.CompareTag("Transition"))
            {
                var rowTransition = other.GetComponent<RowTransition>();
                if (rowTransition == null) return;
                if (rowTransition.destinationSpawn != null) transform.position = rowTransition.destinationSpawn.position;
                SpriteRenderer.sortingOrder = rowTransition.sortingOrder;
                //transform.localScale = new Vector3(rowTransition.targetScale, rowTransition.targetScale, 1f);
                CurrentSize = rowTransition.targetSize;
                SetSize();
                speed = rowTransition.targetSpeed;
            }
        }
        
        public void EnableMovement()
        {
            _canMove = true;
            transform.position = startPosition;
        }
    }
}