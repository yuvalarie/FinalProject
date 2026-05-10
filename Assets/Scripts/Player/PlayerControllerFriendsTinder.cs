using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerFriendsTinder : MonoBehaviour
    {
        [SerializeField] private Transform leftButton;
        [SerializeField] private Transform rightButton;
        
        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            
        }
    }
}
