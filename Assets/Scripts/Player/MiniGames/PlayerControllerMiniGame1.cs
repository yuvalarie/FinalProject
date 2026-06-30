using System;
using System.Collections;
using Audio;
using Audio.AudioEmitters;
using Managers;
using Objects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerControllerMiniGame1 : PlayerControllerBase
    {
        [SerializeField] private GameObject blackScreen;
        [SerializeField] private float blackScreenDuration;
        
        [Header("Hold Settings")]
        [SerializeField, Tooltip("Transform where the held object will sit when facing Right.")]
        private Transform rightHoldSlot;
        [SerializeField, Tooltip("Transform where the held object will sit when facing Left.")]
        private Transform leftHoldSlot;
        
        [SerializeField, Tooltip("The Animator component that handles the lift/drop animations.")]
        private Animator interactionAnimator;

        [SerializeField, Tooltip("Optional offset for the grab radius relative to the active hold slot.")]
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
        [SerializeField] private GameObject endColliders;
        [SerializeField] private GameObject sideColliders;
        [SerializeField] private float enterAnimationDuration;
        
        [Header("Audio")]
        [SerializeField] private AudioEmitterBase misplacedAudioEmitter;
        
        private GrabbableObject _heldGrabbable;
        private int _numOfPlacedObjects = 0;
        
        private bool _isFacingRight = true; // Assuming the player starts facing right (flipX = false)
        
        private bool _isEndSequenceActive = false;
        private int _endSequenceStep = 0;
        private bool _isSequenceWaiting = false;

        private int _musicStage = 0;
        
        private static readonly int GrabAnimation = Animator.StringToHash("Grab");
        private static readonly int DropAnimation = Animator.StringToHash("Drop");

        protected override void Start()
        {
            base.Start();
            StartCoroutine(TurnOffBlackScreenCoroutine());
        }

        private IEnumerator TurnOffBlackScreenCoroutine()
        {
            yield return new WaitForSeconds(blackScreenDuration);
            if(blackScreen != null) blackScreen.SetActive(false);
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
            // Update our internal facing state based on movement input
            if (MoveInput.x > 0 && !_isFacingRight)
            {
                _isFacingRight = true;
                ShiftObjectToActiveSlot();
            }
            else if (MoveInput.x < 0 && _isFacingRight)
            {
                _isFacingRight = false;
                ShiftObjectToActiveSlot();
            }
        }

        private void ShiftObjectToActiveSlot()
        {
            // If we are holding an object, instantly pop it to the new active hold slot
            if (_heldGrabbable != null)
            {
                Transform activeSlot = GetActiveSlot();
                _heldGrabbable.transform.SetParent(activeSlot);
                _heldGrabbable.transform.localPosition = Vector3.zero;
            }
        }
        
        private Transform GetActiveSlot()
        {
            return _isFacingRight ? rightHoldSlot : leftHoldSlot;
        }

        private Vector2 GetGrabOrigin()
        {
            Transform activeSlot = GetActiveSlot();
            // Optional: invert the offset if facing left so it stays in front of the player
            Vector3 currentOffset = _isFacingRight ? grabOriginOffset : new Vector3(-grabOriginOffset.x, grabOriginOffset.y, grabOriginOffset.z);
            
            // If the activeSlot is null (e.g. forgot to assign in inspector), fallback to player center
            if (activeSlot == null) return (Vector2)transform.position + (Vector2)currentOffset;
            
            return (Vector2)activeSlot.position + (Vector2)currentOffset;
        }

        private void UpdateTableStatus()
        {
            float percentage = ((float)_numOfPlacedObjects / totalObjectsToPlace) * 100f;
            int expectedMusicStage = 0;
            
            switch (percentage)
            {
                case >= 16 and < 32:
                    firstStateSprite.SetActive(false);
                    break;
                case >= 32 and < 48:
                    secondStateSprite.SetActive(false);
                    secondStateSprite2.SetActive(false);
                    expectedMusicStage = 1;
                    break;
                case >= 48 and < 64:
                    thirdStateSprite.SetActive(false);
                    thirdStateSprite2.SetActive(false);
                    expectedMusicStage = 2;
                    break;
                case >= 64 and < 80:
                    fourthStateSprite.SetActive(false);
                    SceneLoader.Instance?.PreloadScene(nextSceneName);
                    expectedMusicStage = 3;
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

            if (_musicStage < expectedMusicStage)
            {
                _musicStage++;
                MusicManager.Instance.AdvanceProgress();
            }
        }

        private void AdvanceEndSequence()
        {
            if (_isSequenceWaiting) return;

            if (_endSequenceStep == 0)
            {
                heldaAnimator.SetTrigger("Enter");
                StartCoroutine(EyesAndBubbleRoutine());
            }
            else if (_endSequenceStep == 1)
            {
                textBubble.SetActive(false);
                heldaAnimator.SetTrigger("Exit");
            }
        }
        
        private IEnumerator EyesAndBubbleRoutine()
        {
            _isSequenceWaiting = true;
            yield return new WaitForSeconds(enterAnimationDuration);
            
            eyeAnimator.SetTrigger("EyesRoll");
            textBubble.SetActive(true);
            
            _endSequenceStep++;
            _isSequenceWaiting = false;
            endColliders.SetActive(true);
            sideColliders.SetActive(false);
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
                
                interactionAnimator?.SetTrigger(GrabAnimation);
                
                _heldGrabbable = closestItem;
                
                _heldGrabbable.currentState = GrabbableObject.ObjectState.Held;
                _heldGrabbable.SwitchState();
                _heldGrabbable.CenterChildren();

                // Parent to the currently active slot based on facing direction
                _heldGrabbable.transform.SetParent(GetActiveSlot());
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
                misplacedAudioEmitter?.PlayAudioOnce();
                _heldGrabbable = null;
            }
            
            interactionAnimator?.SetTrigger(DropAnimation);
        }
        
        private void OnDrawGizmos()
        {
            Vector2 grabOrigin = GetGrabOrigin();
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(grabOrigin, pickupRadius);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(grabOrigin, dropRadius);
        }
    }
}