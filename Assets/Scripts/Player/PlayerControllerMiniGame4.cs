using System.Net;
using Npc;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerMiniGame4 : PlayerControllerBase
    {
        [SerializeField, Tooltip("Transform where the held object will sit.")]
        private Transform holdSlot;

        [SerializeField, Tooltip("The layer used for grabbable objects.")]
        private LayerMask grabbableLayer;
        
        [SerializeField, Tooltip("How far the player can reach to grab an object.")]
        private float grabRadius = 1f;
        
        [SerializeField, Tooltip("The layer used for valid drop zones.")]
        private LayerMask dropZoneLayer;
        
        private RoamingNpcController _heldItem;
        
        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (_heldItem == null) TryPickUp();
            else DropItem();
        }
        
        private void TryPickUp()
        {
            Debug.Log("Attempting to pick up item...");

            if (IsTrans)
            {
                Debug.Log("can't interact while transparent");
                return;
            }

            RaycastHit2D hit = Physics2D.CircleCast(transform.position, grabRadius, Vector2.zero, 0f, grabbableLayer);

            if (hit.collider != null)
            {
                Debug.Log($"SUCCESS: Found '{hit.collider.name}' on the Grabbable layer!");
                _heldItem = hit.collider.GetComponent<RoamingNpcController>();
                if (_heldItem == null) return;
                Rigidbody2D itemRb = _heldItem.GetComponent<Rigidbody2D>();
                if (itemRb != null)                {
                    itemRb.bodyType = RigidbodyType2D.Kinematic; // Kinematic for 2D
                }
                else                {
                    Debug.LogWarning($"Wait, '{hit.collider.name}' doesn't have a Rigidbody2D attached!");
                }
                // Collider2D npcCollider = _heldItem.GetComponent<Collider2D>();
                // if (npcCollider != null)
                // {
                //     npcCollider.enabled = false;
                // }
                
                _heldItem.transform.position = holdSlot.position;
                _heldItem.transform.SetParent(holdSlot);
                _heldItem.Roaming = false;
            }
            else
            {
                Debug.Log("FAILED: Nothing found on the Grabbable layer within the grab radius.");
            }
        }

        private void DropItem()
        {
            Debug.Log("Attempting to drop item...");
            
            if (IsTrans)
            {
                Debug.Log("can't interact while transparent");
                return;
            }

            Collider2D dropZone = Physics2D.OverlapCircle(transform.position, grabRadius, dropZoneLayer);

            if (dropZone != null)
            {
                Debug.Log($"SUCCESS: Dropping '{_heldItem.name}' in zone '{dropZone.name}'");

                _heldItem.transform.SetParent(null);
        
                // Optional: If you want it to drop exactly where the player is standing
                // instead of floating in the HoldSlot position, uncomment this:
                // _heldItem.transform.position = transform.position; 
                // Collider2D npcCollider = _heldItem.GetComponent<Collider2D>();
                // if (npcCollider != null)
                // {
                //     npcCollider.enabled = true;
                // }
                _heldItem.ClearWaypoints();
                
                Rigidbody2D itemRb = _heldItem.GetComponent<Rigidbody2D>();
                if (itemRb != null)                {
                    itemRb.bodyType = RigidbodyType2D.Dynamic; // Back to Dynamic for 2D
                }
                _heldItem.Roaming = true;
                
                _heldItem = null;
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