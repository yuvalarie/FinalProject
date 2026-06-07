using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerMiniGame2Walk : PlayerControllerBase
    {
        private bool _movementLocked = false;
        protected override void HandleMovement()
        {
            if (_movementLocked) return;
            Rb.linearVelocity = new Vector2(MoveInput.x * speed, 0f);
        }
        

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            // for now, not used in this part
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
    }
}
