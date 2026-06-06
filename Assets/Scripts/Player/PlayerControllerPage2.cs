using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerPage2 : PlayerControllerBase
    {
        private static readonly int Open = Animator.StringToHash("Open");
        
        [SerializeField,Tooltip("The next scene's name")] private string nextSceneName;

        [Header("Size Settings")]
        [Tooltip("The scale factor for the player's size in frame 1-6.")]
        [SerializeField] private float frame1To6Scale = 1f;
        [Tooltip("The scale factor for the player's size in frame 7.")]
        [SerializeField] private float frame7Scale = 1.5f;
        
        [Header("Elevator Settings")]
        [SerializeField] private Collider2D elevatorTrigger;
        [SerializeField] private Transform elevatorTarget;
        [SerializeField] private Transform elevatorPlacement;
        [SerializeField] private Transform elevatorTargetPlacement;
        
        [Header("Trigger Settings")]
        [SerializeField, Tooltip("The trigger that initiates the transition from frame 6 to 7.")]
        private Collider2D frame6To7Trigger;
        [SerializeField, Tooltip("The trigger that initiates the transition from frame 7 to 6.")]
        private Collider2D frame7To6Trigger;
        
        [Header("Helmet interaction settings")]
        [SerializeField, Tooltip("The placement for the helmet.")]
        private Vector3 helmetPlacement;
        [SerializeField] private List<GameObject> availableHelmets;
        [SerializeField, Tooltip("How much higher each additional helmet should sit (e.g., Y = 0.2)")]
        private Vector3 helmetStackOffset = new Vector3(0f, 0.5f, 0f);
        [SerializeField] private Animator leftDoorAnimator;
        [SerializeField] private Animator rightDoorAnimator;
        [SerializeField] private Collider2D leftDoorCollider;

        [Header("Last frame interaction settings")] 
        [SerializeField] private Collider2D lastFrameTrigger;
        [SerializeField] private GameObject textBubble1;
        [SerializeField] private GameObject textBubble2;

        private SpriteRenderer _spriteRenderer;
        private float _elevatorOffsetY;
        private int _equippedHelmetCount = 0;
        private bool _hasActivatedLastFrameSequence = false;
        private Vector3 _elevatorStartPosition;
        private int _sortingOrderAtStart;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _sortingOrderAtStart = _spriteRenderer.sortingOrder;
            _elevatorStartPosition = elevatorTarget.position;
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (availableHelmets.Count == 0) return;
            int topHelmetIndex = availableHelmets.Count - 1;
            GameObject helmetObject = availableHelmets[topHelmetIndex];
            availableHelmets.RemoveAt(topHelmetIndex);
            helmetObject.transform.SetParent(gameObject.transform);
            helmetObject.transform.localPosition = helmetPlacement + (helmetStackOffset * _equippedHelmetCount);
            var spriteRenderer = helmetObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)            {
                spriteRenderer.sortingOrder = _sortingOrderAtStart + 1 + _equippedHelmetCount;;
            }
            _equippedHelmetCount++;
            if (_equippedHelmetCount == 1)
            {
                leftDoorAnimator.SetTrigger(Open);
                rightDoorAnimator.SetTrigger(Open);
                leftDoorCollider.enabled = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == elevatorTrigger)
            {
                var newPosition = new Vector3(elevatorPlacement.position.x, elevatorPlacement.position.y, transform.position.z);
                _elevatorOffsetY = elevatorTarget.position.y - newPosition.y;
            }
            if (other == frame6To7Trigger)
            {
                transform.localScale = new Vector3(frame7Scale, frame7Scale, 1f);
                _spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                
            }
            else if (other == frame7To6Trigger)
            {
                transform.localScale = new Vector3(frame1To6Scale, frame1To6Scale, 1f);
                _spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
            }
            
            if (other == lastFrameTrigger && !_hasActivatedLastFrameSequence)
            {
                StartCoroutine(LastFrameSequenceCoroutine());
                _hasActivatedLastFrameSequence = true;
            }
            
            if(other.CompareTag("End")) 
            {
                SceneLoader.Instance?.ActivatePreloadedScene();
            }
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other == elevatorTrigger)
            {
                bool isAwayFromTop = Vector3.Distance(elevatorTarget.position, elevatorTargetPlacement.position) > 0.05f;
                bool isAwayFromBottom = Vector3.Distance(elevatorTarget.position, _elevatorStartPosition) > 0.05f;

                if (isAwayFromTop && isAwayFromBottom)
                {
                    transform.position = new Vector3(elevatorPlacement.position.x, transform.position.y, transform.position.z);
                }
                
                if (MoveInput.y != 0f)
                {
                    // if (MoveInput.y < 0 && !isAwayFromBottom) return;
                    // if (MoveInput.y > 0 && !isAwayFromTop) return;
                    float desiredY = transform.position.y + _elevatorOffsetY;
                    float minY = _elevatorStartPosition.y;
                    float maxY = elevatorTargetPlacement.position.y;
                    float clampedY = Mathf.Clamp(desiredY, minY, maxY);
                    // if (Mathf.Abs(desiredY - clampedY) > 0.001f)
                    // {
                    //     _elevatorOffsetY = clampedY - transform.position.y;
                    // }
                    elevatorTarget.position = new Vector3(
                        elevatorTarget.position.x, 
                        //transform.position.y + _elevatorOffsetY, 
                        clampedY,
                        elevatorTarget.position.z
                    );
                    if (desiredY != clampedY)
                    {
                        transform.position = new Vector3(
                            transform.position.x, 
                            clampedY - _elevatorOffsetY, 
                            transform.position.z
                        );
                    }
                }
            }
        }

        private IEnumerator LastFrameSequenceCoroutine()
        {
            textBubble1.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            textBubble2.SetActive(true);
        }
    }
}