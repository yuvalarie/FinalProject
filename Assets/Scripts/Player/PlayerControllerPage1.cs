using System;
using Transitions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerControllerPage1 : PlayerControllerBase
    {
        [SerializeField] private Sprite page1Sprite;
        private SpriteRenderer _spriteRenderer;
        public bool canMove;
 
        private void Start()
        {
                _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        protected override void HandleMovement()
        {
            if (!canMove)
            {
                Rb.linearVelocity = Vector2.zero;
                return;
            }
            base.HandleMovement();
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Transition"))
            {
                var rowTransition = other.GetComponent<RowTransition>();
                if (rowTransition == null) return;
                transform.position = rowTransition.destinationSpawn.position;
                _spriteRenderer.sortingOrder = rowTransition.sortingOrder;
                transform.localScale = new Vector3(rowTransition.targetScale, rowTransition.targetScale, 1f);
            }
        }
    }
}