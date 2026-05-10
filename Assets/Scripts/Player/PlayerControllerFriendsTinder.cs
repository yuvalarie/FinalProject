using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerFriendsTinder : PlayerControllerBase
    {
        [SerializeField] private Transform leftButton;
        [SerializeField] private Transform rightButton;
        
        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            
        }
    }
}
