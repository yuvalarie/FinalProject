using Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerMiniGame1 : PlayerControllerBase
    {
        [SerializeField, Tooltip("Transform where the held object will sit.")]
        private Transform holdSlot;

        [SerializeField, Tooltip("The layer used for grabbable objects.")]
        private LayerMask grabbableLayer;
        
        [SerializeField, Tooltip("How far the player can reach to grab an object.")]
        private float grabRadius = 1f;
        
        [SerializeField, Tooltip("The layer used for valid drop zones.")]
        private LayerMask dropZoneLayer;
        
        private GameObject _heldItem;
        private GrabbableObject _heldGrabbable;
        
        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (_heldItem == null) TryPickUp();
            else DropItem();
        }
        
        private void TryPickUp()
        {
            Debug.Log("Attempting to pick up item...");

            // if (IsTrans)
            // {
            //     Debug.Log("can't interact while transparent");
            //     return;
            // }

            RaycastHit2D hit = Physics2D.CircleCast(transform.position, grabRadius, Vector2.zero, 0f, grabbableLayer);

            if (hit.collider != null)
            {
                Debug.Log($"SUCCESS: Found '{hit.collider.name}' on the Grabbable layer!");
                _heldItem = hit.collider.gameObject;
                // try get GrabbableObject component for debug purposes, but it's not required to pick up
                _heldGrabbable = _heldItem.GetComponent<GrabbableObject>();
                if (_heldGrabbable == null || _heldGrabbable.currentState != GrabbableObject.ObjectState.Start)
                {
                    Debug.LogWarning($"Wait, '{hit.collider.name}' is already placed? Are you sure it's on the right layer?");
                    _heldItem = null;
                    return;
                }
                _heldGrabbable.currentState = GrabbableObject.ObjectState.Held;
                _heldGrabbable.SwitchState();
                
                Rigidbody2D itemRb = _heldItem.GetComponent<Rigidbody2D>();
                if (itemRb != null) 
                {
                    itemRb.bodyType = RigidbodyType2D.Kinematic; // Kinematic for 2D
                }
                else
                {
                    Debug.LogWarning($"Wait, '{hit.collider.name}' doesn't have a Rigidbody2D attached!");
                }

                _heldItem.transform.position = holdSlot.position;
                _heldItem.transform.SetParent(holdSlot);
            }
            else
            {
                Debug.Log("FAILED: Nothing found on the Grabbable layer within the grab radius.");
            }
        }

        private void DropItem()
        {
            Debug.Log("Attempting to drop item...");
            
            // if (IsTrans)
            // {
            //     Debug.Log("can't interact while transparent");
            //     return;
            // }

            Collider2D dropZone = Physics2D.OverlapCircle(transform.position, grabRadius, dropZoneLayer);

            if (dropZone != null)
            {
                if (dropZone != _heldGrabbable.targetDropSpot) return;
                Debug.Log($"SUCCESS: Dropping '{_heldItem.name}' in zone '{dropZone.name}'");

                _heldItem.transform.SetParent(null);
                _heldGrabbable.currentState = GrabbableObject.ObjectState.Placed;
                _heldGrabbable.SwitchState();
        
                // Optional: If you want it to drop exactly where the player is standing
                // instead of floating in the HoldSlot position, uncomment this:
                // _heldItem.transform.position = transform.position; 

                _heldItem = null;
                _heldGrabbable = null;
            }
            else
            {
                Debug.Log("FAILED: Cannot drop here. You must be in a drop zone!");
            }
        }
        
        private void OnDrawGizmos()
        {
            // Draws a yellow circle around the player in the Scene view to show exactly where they can grab
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, grabRadius);
        }
    }
}