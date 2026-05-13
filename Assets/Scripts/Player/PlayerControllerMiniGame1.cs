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
        
        private GrabbableObject _heldGrabbable;
        
        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (_heldGrabbable == null) TryPickUp();
            else DropItem();
        }
        
        private void TryPickUp()
        {
            Debug.Log("Attempting to pick up item...");

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, grabRadius, grabbableLayer);
            
            GrabbableObject closestItem = null;
            float closestDistance = float.MaxValue;

            foreach (Collider2D hit in hits)
            {
                GrabbableObject grabbable = hit.GetComponentInParent<GrabbableObject>();
                
                if (grabbable != null && grabbable.currentState == GrabbableObject.ObjectState.Start)
                {
                    float distance = Vector2.Distance(transform.position, grabbable.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestItem = grabbable;
                    }
                }
            }

            if (closestItem != null)
            {
                Debug.Log($"SUCCESS: Picking up '{closestItem.gameObject.name}'!");
                _heldGrabbable = closestItem;
                
                _heldGrabbable.currentState = GrabbableObject.ObjectState.Held;
                _heldGrabbable.SwitchState();
                
                _heldGrabbable.CenterChildren();

                _heldGrabbable.transform.SetParent(holdSlot);
                _heldGrabbable.transform.localPosition = Vector3.zero;
            }
            else
            {
                Debug.Log("FAILED: Nothing found on the Grabbable layer within the grab radius.");
            }
        }

        private void DropItem()
        {
            Debug.Log("Attempting to drop item...");

            Collider2D[] dropZones = Physics2D.OverlapCircleAll(transform.position, grabRadius, dropZoneLayer);
            bool foundCorrectZone = false;

            foreach (Collider2D zone in dropZones)
            {
                if (zone == _heldGrabbable.targetDropSpot)
                {
                    foundCorrectZone = true;
                    break;
                }
            }

            if (foundCorrectZone)
            {
                Debug.Log($"SUCCESS: Dropping '{_heldGrabbable.gameObject.name}' in its correct zone!");

                _heldGrabbable.transform.SetParent(_heldGrabbable.targetDropSpot.transform);
                _heldGrabbable.transform.localPosition = Vector3.zero; 
                
                _heldGrabbable.currentState = GrabbableObject.ObjectState.Placed;
                _heldGrabbable.SwitchState();
                
                _heldGrabbable = null;
            }
            else if (dropZones.Length > 0)
            {
                Debug.LogWarning("FAILED: You are inside a drop zone, but it is the WRONG zone for this specific item!");
            }
            else
            {
                Debug.Log("FAILED: Cannot drop here. You must be in a drop zone!");
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, grabRadius);
        }
    }
}