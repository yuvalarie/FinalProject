using System;
using Objects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerControllerMiniGame1 : PlayerControllerBase
    {
        [Header("Hand Settings")]
        [SerializeField, Tooltip("The child GameObject for the Right Hand.")]
        private GameObject rightHand;
        [SerializeField, Tooltip("The Animator component on the Right Hand.")]
        private Animator rightHandAnimator;
        
        [SerializeField, Tooltip("The child GameObject for the Left Hand.")]
        private GameObject leftHand;
        [SerializeField, Tooltip("The Animator component on the Left Hand.")]
        private Animator leftHandAnimator;

        [SerializeField, Tooltip("Optional offset for the grab radius relative to the active hand.")]
        private Vector3 grabOriginOffset = Vector3.zero;

        [SerializeField, Tooltip("The layer used for grabbable objects.")]
        private LayerMask grabbableLayer;
        
        [SerializeField, Tooltip("How far the player can reach to pick up an object.")]
        private float pickupRadius = 1f;
        
        [SerializeField, Tooltip("How far the player can reach to drop an object into a zone.")]
        private float dropRadius = 1.5f;
        
        [SerializeField, Tooltip("The layer used for valid drop zones.")]
        private LayerMask dropZoneLayer;

        [Header("Table Status Settings")] 
        [SerializeField] private int totalObjectsToPlace;
        [SerializeField] private GameObject firstStateSprite;
        [SerializeField] private GameObject secondStateSprite;
        [SerializeField] private GameObject secondStateSprite2;
        [SerializeField] private GameObject thirdStateSprite;
        [SerializeField] private GameObject thirdStateSprite2;
        [SerializeField] private GameObject fourthStateSprite;
        [SerializeField] private GameObject fifthStateSprite;
        [SerializeField] private GameObject sixthStateSprite;
        
        private GrabbableObject _heldGrabbable;
        private int _numOfPlacedObjects = 0;
        
        private SpriteRenderer _spriteRenderer;
        private bool _isFacingRight = false;
        
        private static readonly int GrabAnimation = Animator.StringToHash("Grab");
        private static readonly int DropAnimation = Animator.StringToHash("Drop");

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            leftHand.SetActive(true);
            rightHand.SetActive(false);
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (_heldGrabbable == null) TryPickUp();
            else DropItem();
        }

        private void Update()
        {
            UpdateTableStatus();
            HandleFacingDirection();
        }
        
        private void HandleFacingDirection()
        {
            if (MoveInput.x > 0 && !_isFacingRight)
            {
                Flip();
            }
            else if (MoveInput.x < 0 && _isFacingRight)
            {
                Flip();
            }
        }
        
        private void Flip()
        {
            _isFacingRight = !_isFacingRight;
            
            if (_spriteRenderer != null)
            {
                _spriteRenderer.flipX = _isFacingRight;
            }
            
            if (_heldGrabbable != null)
            {
                Transform newHand = GetActiveHand();
                _heldGrabbable.transform.SetParent(newHand);
                _heldGrabbable.transform.localPosition = Vector3.zero;
            }

            // Toggle the active hands
            rightHand.SetActive(_isFacingRight);
            leftHand.SetActive(!_isFacingRight);
        }
        
        private Transform GetActiveHand()
        {
            return _isFacingRight ? rightHand.transform : leftHand.transform;
        }

        private Animator GetActiveAnimator()
        {
            return _isFacingRight ? rightHandAnimator : leftHandAnimator;
        }

        private Vector2 GetGrabOrigin()
        {
            Transform activeHand = GetActiveHand();
            Vector3 currentOffset = _isFacingRight ? grabOriginOffset : new Vector3(-grabOriginOffset.x, grabOriginOffset.y, grabOriginOffset.z);
            return (Vector2)activeHand.position + (Vector2)currentOffset;
        }

        private void UpdateTableStatus()
        {
            // Force float division by casting the numerator to (float)
            float percentage = ((float)_numOfPlacedObjects / totalObjectsToPlace) * 100f;

            Debug.Log($"Updating table status: {_numOfPlacedObjects}/{totalObjectsToPlace} objects placed, percentage: {percentage}%");

            switch (percentage)
            {
                case >= 16 and < 32:
                    firstStateSprite.SetActive(false);
                    break;
                case >= 32 and < 48:
                    secondStateSprite.SetActive(false);
                    secondStateSprite2.SetActive(false);
                    break;
                case >= 48 and < 64:
                    thirdStateSprite.SetActive(false);
                    thirdStateSprite2.SetActive(false);
                    break;
                case >= 64 and < 80:
                    fourthStateSprite.SetActive(false);
                    break;
                case >= 80 and < 100:
                    fifthStateSprite.SetActive(false);
                    break;
                case >= 100:
                    sixthStateSprite.SetActive(false);
                    break;
            }
        }

        private void TryPickUp()
        {
            Debug.Log("Attempting to pick up item...");

            Vector2 grabOrigin = GetGrabOrigin();
            Collider2D[] hits = Physics2D.OverlapCircleAll(grabOrigin, pickupRadius, grabbableLayer);
            
            GrabbableObject closestItem = null;
            float closestDistance = float.MaxValue;

            foreach (Collider2D hit in hits)
            {
                GrabbableObject grabbable = hit.GetComponentInParent<GrabbableObject>();
                
                if (grabbable != null && grabbable.currentState == GrabbableObject.ObjectState.Start)
                {
                    float distance = Vector2.Distance(grabOrigin, grabbable.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestItem = grabbable;
                    }
                }
            }

            if (closestItem != null)
            {
                GetActiveAnimator()?.SetTrigger(GrabAnimation);
                Debug.Log($"SUCCESS: Picking up '{closestItem.gameObject.name}'!");
                _heldGrabbable = closestItem;
                
                _heldGrabbable.currentState = GrabbableObject.ObjectState.Held;
                _heldGrabbable.SwitchState();
                
                _heldGrabbable.CenterChildren();

                _heldGrabbable.transform.SetParent(GetActiveHand());
                _heldGrabbable.transform.localPosition = Vector3.zero;
                _heldGrabbable.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.Log("FAILED: Nothing found on the Grabbable layer within the grab radius.");
            }
        }

        private void DropItem()
        {
            Debug.Log("Attempting to drop item...");
            Vector2 grabOrigin = GetGrabOrigin();
            DropZone validZone = null;

            Collider2D[] dropZones = Physics2D.OverlapCircleAll(grabOrigin, dropRadius, dropZoneLayer);
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
                _heldGrabbable.transform.localRotation = Quaternion.identity;
                
                _heldGrabbable.currentState = GrabbableObject.ObjectState.Placed;
                _heldGrabbable.SwitchState();
                
                _heldGrabbable = null;
                _numOfPlacedObjects++;
            }
            else
            {
                Debug.Log("FAILED: Returning item to its original location.");
                _heldGrabbable.ResetPosition();
                _heldGrabbable.currentState = GrabbableObject.ObjectState.Start;
                _heldGrabbable.SwitchState();
                _heldGrabbable = null;
            }
            GetActiveAnimator()?.SetTrigger(DropAnimation);
        }
        
        private void OnDrawGizmos()
        {
            if (rightHand == null || leftHand == null) return; 
            Vector2 grabOrigin = GetGrabOrigin();
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(grabOrigin, pickupRadius);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(grabOrigin, dropRadius);
        }
    }
}