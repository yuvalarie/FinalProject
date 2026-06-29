using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerMiniGame2Walk : PlayerControllerBase
    {
        private bool _movementLocked = false;
        private bool _interactionLocked = true;
        private Action _interactionAction;

        protected override void Start()
        {
            base.Start();
            SceneLoader.Instance?.PreloadScene(nextSceneName);
        }

        public void SetInteractionAction(Action action)
        {
            _interactionAction = action;
        }
        protected override void HandleMovement()
        {
            if (_movementLocked) return;
            Rb.linearVelocity = new Vector2(MoveInput.x * speed, 0f);
        }
        

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (_interactionLocked || !context.performed) return;
            // for now, not used in this part
            _interactionAction?.Invoke();
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
