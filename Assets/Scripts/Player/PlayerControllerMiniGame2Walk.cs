using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerMiniGame2Walk : PlayerControllerBase
    {
        protected override void HandleMovement()
        {
            Rb.linearVelocity = new Vector2(MoveInput.x * speed, 0f);
        }
        

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            // for now, not used in this part
        }
        
        public void EnableMovement()
        {
            enabled = true;
        }
        
        public void DisableMovement()
        {
            Rb.linearVelocity = Vector2.zero;
            enabled = false;
        }
    }
}
