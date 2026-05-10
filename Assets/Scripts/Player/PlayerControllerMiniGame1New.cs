using Objects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerControllerMiniGame1New : PlayerControllerBase
    {
        [Header("Elevation Settings")]
        [SerializeField, Tooltip("How fast the player flies up when holding the interact button.")]
        private float liftSpeed = 8f;
        
        [FormerlySerializedAs("HoldSlot")]
        [Header("Holding Settings")]
        [Tooltip("An empty GameObject child of the player where items will attach.")]
        public Transform holdSlot;
        
        public GrabbableObject CurrentHeldItem { get; set; }
        private bool _isElevating;

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _isElevating = true;
            }
            else if (context.canceled)
            {
                _isElevating = false;
            }
        }

        protected override void HandleMovement()
        {
            float targetX = MoveInput.x * speed;
            float targetY = _isElevating ? liftSpeed : Rb.linearVelocity.y;
            Rb.linearVelocity = new Vector2(targetX, targetY);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // LOGIC 1: Are we empty-handed and bumping into a grabbable object?
            if (CurrentHeldItem == null)
            {
                GrabbableObject itemWeHit = other.GetComponent<GrabbableObject>();
                
                // If it is an item, and it hasn't been placed yet, pick it up!
                if (itemWeHit != null && itemWeHit.currentState == GrabbableObject.ObjectState.Start)
                {
                    PickUpItem(itemWeHit);
                    return; // Exit early so we don't accidentally do drop logic
                }
            }

            // LOGIC 2: Are we holding something, and bumping into its specific drop spot?
            if (CurrentHeldItem != null && other == CurrentHeldItem.targetDropSpot)
            {
                Debug.Log($"Collided with drop spot for {CurrentHeldItem.gameObject.name}!");
                DropItem();
            }
        }

        private void PickUpItem(GrabbableObject item)
        {
            CurrentHeldItem = item;
            CurrentHeldItem.currentState = GrabbableObject.ObjectState.Held;

            // Snap to player's hand
            CurrentHeldItem.transform.SetParent(holdSlot);
            CurrentHeldItem.transform.localPosition = Vector3.zero;
            
            Debug.Log($"Picked up: {CurrentHeldItem.gameObject.name}");
        }

        private void DropItem()
        {
            Debug.Log($"Successfully placed: {CurrentHeldItem.gameObject.name}");
            
            CurrentHeldItem.currentState = GrabbableObject.ObjectState.Placed;

            // Snap to the drop zone
            CurrentHeldItem.transform.SetParent(CurrentHeldItem.targetDropSpot.transform);
            CurrentHeldItem.transform.localPosition = Vector3.zero;

            // Empty the player's hands
            CurrentHeldItem = null;
        }
    }
}