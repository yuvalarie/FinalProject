using System;
using System.Collections;
using Managers;
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

        [Header("End Sequence")] 
        [SerializeField] private Animator heldaAnimator;
        [SerializeField] private Animator eyeAnimator;
        [SerializeField] private GameObject textBubble;
        
        private GrabbableObject _heldGrabbable;
        private int _numOfPlacedObjects = 0;
        
        private SpriteRenderer _spriteRenderer;
        private SpriteRenderer _rightHandSprite;
        private SpriteRenderer _leftHandSprite;
        private bool _isFacingRight = false;
        
        private bool _isEndSequenceActive = false;
        private int _endSequenceStep = 0;
        private bool _isSequenceWaiting = false;
        
        private static readonly int GrabAnimation = Animator.StringToHash("Grab");
        private static readonly int DropAnimation = Animator.StringToHash("Drop");

        protected override void Start()
        {
            base.Start();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            _rightHandSprite = rightHand.GetComponent<SpriteRenderer>();
            _leftHandSprite = leftHand.GetComponent<SpriteRenderer>();
            
            rightHand.SetActive(true);
            leftHand.SetActive(true);
            
            if (_rightHandSprite != null) _rightHandSprite.enabled = false;
            if (_leftHandSprite != null) _leftHandSprite.enabled = true;
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            if (_isEndSequenceActive)
            {
                AdvanceEndSequence();
                return; 
            }

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
            
            if (_rightHandSprite != null) _rightHandSprite.enabled = _isFacingRight;
            if (_leftHandSprite != null) _leftHandSprite.enabled = !_isFacingRight;
            
            if (_heldGrabbable != null)
            {
                Transform newHand = GetActiveHand();
                _heldGrabbable.transform.SetParent(newHand);
                _heldGrabbable.transform.localPosition = Vector3.zero;
            }
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
            float percentage = ((float)_numOfPlacedObjects / totalObjectsToPlace) * 100f;
            Debug.Log($"placed {_numOfPlacedObjects} objects");

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
                    if (!_isEndSequenceActive)
                    {
                        _isEndSequenceActive = true;
                        AdvanceEndSequence();
                    }
                    break;
            }
        }

        private void AdvanceEndSequence()
        {
            if (_isSequenceWaiting) return;

            if (_endSequenceStep == 0)
            {
                heldaAnimator.SetTrigger("Enter");
                _endSequenceStep++;
            }
            else if (_endSequenceStep == 1)
            {
                StartCoroutine(EyesAndBubbleRoutine());
            }
            else if (_endSequenceStep == 2)
            {
                SceneLoader.Instance.ActivatePreloadedScene();
            }
        }
        
        private IEnumerator EyesAndBubbleRoutine()
        {
            _isSequenceWaiting = true;
            
            eyeAnimator.SetTrigger("EyesRoll");
            yield return new WaitForSeconds(1.5f);
            textBubble.SetActive(true);
            
            _endSequenceStep++;
            _isSequenceWaiting = false;
        }

        private void TryPickUp()
        {
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
                if (closestItem.isInstantPlacement)
                {
                    closestItem.currentState = GrabbableObject.ObjectState.Placed;
                    closestItem.SwitchState();
                    _numOfPlacedObjects++;
                    return;
                }
                
                // Trigger Grab on BOTH hands at the exact same time so they sync perfectly!
                rightHandAnimator?.SetTrigger(GrabAnimation);
                leftHandAnimator?.SetTrigger(GrabAnimation);
                
                _heldGrabbable = closestItem;
                
                _heldGrabbable.currentState = GrabbableObject.ObjectState.Held;
                _heldGrabbable.SwitchState();
                
                _heldGrabbable.CenterChildren();

                _heldGrabbable.transform.SetParent(GetActiveHand());
                _heldGrabbable.transform.localPosition = Vector3.zero;
                _heldGrabbable.transform.localRotation = Quaternion.identity;
            }
        }

        private void DropItem()
        {
            Vector2 grabOrigin = GetGrabOrigin();
            DropZone validZone = null;

            Collider2D[] dropZones = Physics2D.OverlapCircleAll(grabOrigin, dropRadius, dropZoneLayer);

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
                _heldGrabbable.ResetPosition();
                _heldGrabbable.currentState = GrabbableObject.ObjectState.Start;
                _heldGrabbable.SwitchState();
                _heldGrabbable = null;
            }
            
            // Trigger Drop on BOTH hands at the same time
            rightHandAnimator?.SetTrigger(DropAnimation);
            leftHandAnimator?.SetTrigger(DropAnimation);
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