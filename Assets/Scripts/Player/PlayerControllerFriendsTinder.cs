using Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public enum TinderButton
    {
        Left, // like tinder, swipe left 
        Right
    }
    
    public class PlayerControllerFriendsTinder : PlayerControllerBase
    {
        [SerializeField] private Transform leftButton;
        [SerializeField] private Transform rightButton;
        [SerializeField] private GameObject hand;
        [SerializeField] private FriendSpawner friendSpawner;
        private TinderButton _currentButton = TinderButton.Left;
        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if(_currentButton == TinderButton.Left) friendSpawner.ShowNextFriend();
            else friendSpawner.SpawnFriend();
        }

        protected override void HandleMovement()
        {
            if (MoveInput.x > 0.1f)
            {
                _currentButton = TinderButton.Right;
                hand.transform.position = rightButton.position;
            }
            else if (MoveInput.x < -0.1f)
            {
                _currentButton = TinderButton.Left;
                hand.transform.position = leftButton.position;
            }
        }
    }
}
