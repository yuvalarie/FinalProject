using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerMiniGame2Walk : PlayerControllerBase
    {
        [Header("Hover Bob")]
        [SerializeField] private float bobRange = 0.17f;
        [SerializeField] private float bobCycleDuration = 2f;

        private bool _movementLocked = false;
        private bool _interactionLocked = true;
        private Action _interactionAction;
        private float _baseY;
        public bool HasReachedEndTrigger { get; private set; }

        protected override void Start()
        {
            base.Start();
            SceneLoader.Instance?.PreloadScene(nextSceneName);
            _baseY = Rb.position.y;
        }

        public void SetInteractionAction(Action action)
        {
            _interactionAction = action;
        }
        protected override void HandleMovement()
        {
            ApplyHoverBob();
            if (_movementLocked) return;
            Rb.linearVelocity = new Vector2(MoveInput.x * speed, 0f);
        }

        private void ApplyHoverBob()
        {
            float y = _baseY + bobRange * 0.5f * (1f - Mathf.Cos(2f * Mathf.PI * Time.fixedTime / bobCycleDuration));
            Rb.position = new Vector2(Rb.position.x, y);
        }
        

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (_interactionLocked || !context.performed) return;
            // for now, not used in this part
            _interactionAction?.Invoke();
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("End"))
            {
                HasReachedEndTrigger = true;
            }

            base.OnTriggerEnter2D(other);
        }
        
        public void EnableMovement()
        {
            _movementLocked = false;
        }
        
        public void DisableMovement()
        {
            Rb.linearVelocity = Vector2.zero;
            _movementLocked = true;
        }
        
        public void EnableInteraction()
        {
            _interactionLocked = false;
        }
        
        public void DisableInteraction()
        {
            _interactionLocked = true;
        }
    }
}
