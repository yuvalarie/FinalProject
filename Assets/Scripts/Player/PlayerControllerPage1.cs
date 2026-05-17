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
        private SpriteRenderer _spriteRenderer;
        public bool canMove;
        private float _lastTransitionTime = -1f;
        
        [SerializeField, Tooltip("The next scene's name")] private string nextSceneName;
 
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
                if (Time.time < _lastTransitionTime + transitionCooldown) return;
                var rowTransition = other.GetComponent<RowTransition>();
                if (rowTransition == null) return;
                transform.position = rowTransition.destinationSpawn.position;
                _spriteRenderer.sortingOrder = rowTransition.sortingOrder;
                transform.localScale = new Vector3(rowTransition.targetScale, rowTransition.targetScale, 1f);
                _lastTransitionTime = Time.time;
            }

            if (other.CompareTag("End"))
            {
                SceneLoader.Instance.LoadScene(nextSceneName);
            }
        }
    }
}