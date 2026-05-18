using System;
using Objects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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

        [Header("Table Status Settings")] 
        [SerializeField] private int totalObjectsToPlace;
        [SerializeField] private int secondStatePercentage;
        [SerializeField] private Sprite secondStateSprite;
        [SerializeField] private int thirdStatePercentage;
        [SerializeField] private Sprite thirdStateSprite;
        [SerializeField] private int fourthStatePercentage;
        [SerializeField] private Sprite fourthStateSprite;
        [SerializeField] private int fifthStatePercentage;
        [SerializeField] private Sprite fifthStateSprite;
        [SerializeField] private int sixthStatePercentage;
        [SerializeField] private Sprite sixthStateSprite;
        [SerializeField] private SpriteRenderer tableSpriteRenderer;  
        
        private GrabbableObject _heldGrabbable;
        private int _numOfPlacedObjects = 0;
        
        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (_heldGrabbable == null) TryPickUp();
            else DropItem();
        }

        private void Update()
        {
            UpdateTableStatus();
        }

        private void UpdateTableStatus()
        {
            tableSpriteRenderer.sprite = (_numOfPlacedObjects / totalObjectsToPlace * 100) switch
            {
                var n when n >= fifthStatePercentage && fifthStateSprite != null => fifthStateSprite,
                var n when n >= fourthStatePercentage && fourthStateSprite != null => fourthStateSprite,
                var n when n >= thirdStatePercentage && thirdStateSprite != null => thirdStateSprite,
                var n when n >= secondStatePercentage && secondStateSprite != null => secondStateSprite,
                _ => tableSpriteRenderer.sprite
            };
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
            DropZone validZone = null;

            Collider2D[] dropZones = Physics2D.OverlapCircleAll(transform.position, grabRadius, dropZoneLayer);
            bool foundCorrectZone = false;

            foreach (Collider2D zone in dropZones)
            {
                if (_heldGrabbable.targetDropSpot != null && zone == _heldGrabbable.targetDropSpot)
                {
                    validZone = zone.GetComponent<DropZone>();
                    break;
                }
                if(_heldGrabbable.validDropSpots != null && _heldGrabbable.validDropSpots.Length > 0)
                {
                    foreach (Collider2D validSpot in _heldGrabbable.validDropSpots)
                    {
                        var dropZoneComponent = zone.GetComponent<DropZone>();
                        if (zone == validSpot && dropZoneComponent != null && !dropZoneComponent.isOccupied)
                        {
                            validZone = dropZoneComponent;
                            break;
                        }
                    }
                }
                if (validZone != null) break;
            }

            if (validZone != null)
            {
                Debug.Log($"SUCCESS: Dropping '{_heldGrabbable.gameObject.name}' in its correct zone!");
                
                validZone.isOccupied = true;
                _heldGrabbable.transform.SetParent(validZone.transform);
                _heldGrabbable.transform.localPosition = Vector3.zero; 
                
                _heldGrabbable.currentState = GrabbableObject.ObjectState.Placed;
                _heldGrabbable.SwitchState();
                
                _heldGrabbable = null;
                _numOfPlacedObjects++;
            }
            else
            {
                Debug.Log("FAILED: Returning item to its original location.");
                _heldGrabbable.transform.SetParent(null);
                _heldGrabbable.ResetPosition();
                _heldGrabbable.currentState = GrabbableObject.ObjectState.Start;
                _heldGrabbable.SwitchState();
                _heldGrabbable = null;
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, grabRadius);
        }
    }
}