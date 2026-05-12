using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerPage2 : PlayerControllerBase
    {
        [Header("Size Settings")]
        [Tooltip("The scale factor for the player's size in frame 1.")]
        [SerializeField] private float frame1Scale = 0.5f;
        [Tooltip("The scale factor for the player's size in frame 2-6.")]
        [SerializeField] private float frame2To6Scale = 1f;
        [Tooltip("The scale factor for the player's size in frame 7.")]
        [SerializeField] private float frame7Scale = 1.5f;

        [Header("Transition Settings")]
        [Tooltip("How long the player is frozen in the dark between frames.")]
        [SerializeField] private float freezeDuration = 2f;
        [Tooltip("How far backward (to the left) the player is moved before entering the new frame.")]
        [SerializeField] private float backwardDistance = 1.5f;
        
        [Header("Trigger Settings")]
        [Tooltip("Trigger to transition to frame 2.")]
        [SerializeField] private Collider2D frame1To2Trigger;
        [Tooltip("Trigger to transition to frame 3.")]
        [SerializeField] private Collider2D frame2To3Trigger;
        [Tooltip("Trigger to transition to frame 5.")]
        [SerializeField] private Collider2D frame4To5Trigger;
        [Tooltip("Trigger to transition to frame 6.")]
        [SerializeField] private Collider2D frame5To6Trigger;
        [Tooltip("Trigger to transition to frame 7.")]
        [SerializeField] private Collider2D frame6To7Trigger;
        
        [Header("Mask Settings")]
        [SerializeField] private SpriteMask frame1Mask;
        [SerializeField] private SpriteMask frame2Mask;
        [SerializeField] private SpriteMask frame3Mask;
        [SerializeField] private SpriteMask frame4Mask;
        [SerializeField] private SpriteMask frame5Mask;
        [SerializeField] private SpriteMask frame6Mask;
        [SerializeField] private SpriteMask frame7Mask;
        
        [Header("Elevator Settings")]
        [SerializeField] private Collider2D elevatorTrigger;
        [SerializeField] private Transform elevatorTarget;

        private bool _isTransitioning;
        private float _elevatorOffsetY;

        private void Start()
        {
            frame2Mask.gameObject.SetActive(false);
            frame3Mask.gameObject.SetActive(false);
            frame4Mask.gameObject.SetActive(false);
            frame5Mask.gameObject.SetActive(false);
            frame6Mask.gameObject.SetActive(false);
            frame7Mask.gameObject.SetActive(false);
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
        }
        
        protected override void HandleMovement()
        {
            // If the player is in the dark gutter, ignore their inputs completely!
            if (_isTransitioning) return;
            
            // Otherwise, let the Base script apply the velocity as normal
            base.HandleMovement();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == elevatorTrigger)
            {
                _elevatorOffsetY = elevatorTarget.position.y - transform.position.y;
                return;
            }
            if (_isTransitioning) return;
            float moveDir = MoveInput.x != 0 ? MoveInput.x : Rb.linearVelocity.x;
            if (moveDir == 0) return;
            float directionSign = Mathf.Sign(moveDir);

            if (other == frame1To2Trigger)
            {
                StartCoroutine(directionSign > 0
                    ? MoveToNextFrame(frame1Mask, frame2Mask, frame2To6Scale, directionSign)
                    : MoveToNextFrame(frame2Mask, frame1Mask, frame1Scale, directionSign));
            }
            else if (other == frame2To3Trigger)
            {
                StartCoroutine(directionSign > 0
                    ? MoveToNextFrame(frame2Mask, frame3Mask, frame2To6Scale, directionSign)
                    : MoveToNextFrame(frame3Mask, frame2Mask, frame2To6Scale, directionSign));
                frame4Mask.gameObject.SetActive(directionSign > 0);
            }
            else if (other == frame4To5Trigger)
            {
                frame3Mask.gameObject.SetActive((directionSign > 0));
                StartCoroutine(directionSign > 0
                    ? MoveToNextFrame(frame5Mask, frame4Mask, frame2To6Scale, directionSign)
                    : MoveToNextFrame(frame4Mask, frame5Mask, frame2To6Scale, directionSign));
            }
            else if (other == frame5To6Trigger)
            {
                StartCoroutine(directionSign > 0
                    ? MoveToNextFrame(frame6Mask, frame5Mask, frame2To6Scale, directionSign)
                    : MoveToNextFrame(frame5Mask, frame6Mask, frame2To6Scale, directionSign));
            }
            else if (other == frame6To7Trigger)
            {
                StartCoroutine(directionSign > 0
                    ? MoveToNextFrame(frame7Mask, frame6Mask, frame2To6Scale, directionSign)
                    : MoveToNextFrame(frame6Mask, frame7Mask, frame7Scale, directionSign));
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other == elevatorTrigger && MoveInput.y != 0f)
            {
                elevatorTarget.position = new Vector3(
                    elevatorTarget.position.x, 
                    transform.position.y + _elevatorOffsetY, 
                    elevatorTarget.position.z
                );
            }
        }

        private IEnumerator MoveToNextFrame(SpriteMask currentMask, SpriteMask nextMask, float targetScale, float directionSign)
        {
            _isTransitioning = true;
            currentMask.gameObject.SetActive(false);
            //InputActions.Game.Disable();
            Rb.linearVelocity = Vector2.zero;
            Rb.bodyType = RigidbodyType2D.Kinematic;
            yield return new WaitForSeconds(freezeDuration);
            //transform.position -= new Vector3(backwardDistance * directionSign, 0f, 0f);
            transform.localScale = Vector3.one * targetScale;
            nextMask.gameObject.SetActive(true);
            Rb.bodyType = RigidbodyType2D.Dynamic;
            //InputActions.Game.Enable();
            _isTransitioning = false;
        }
    }
}