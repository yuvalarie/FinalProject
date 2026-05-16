using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerPage2 : PlayerControllerBase
    {
        [Header("Size Settings")]
        [Tooltip("The scale factor for the player's size in frame 1-6.")]
        [SerializeField] private float frame1To6Scale = 1f;
        [Tooltip("The scale factor for the player's size in frame 7.")]
        [SerializeField] private float frame7Scale = 1.5f;
        
        [Header("Elevator Settings")]
        [SerializeField] private Collider2D elevatorTrigger;
        [SerializeField] private Transform elevatorTarget;
        
        [Header("Trigger Settings")]
        [SerializeField, Tooltip("The trigger that initiates the transition from frame 6 to 7.")]
        private Collider2D frame6To7Trigger;
        [SerializeField, Tooltip("The trigger that initiates the transition from frame 7 to 6.")]
        private Collider2D frame7To6Trigger;

        private SpriteRenderer _spriteRenderer;
        private float _elevatorOffsetY;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == elevatorTrigger)
            {
                _elevatorOffsetY = elevatorTarget.position.y - transform.position.y;
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
    }
}