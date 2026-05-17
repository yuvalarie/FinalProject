using System;
using System.Collections;
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
        
        [Header("Trigger Settings")]
        [SerializeField, Tooltip("The trigger that initiates the transition from frame 6 to 7.")]
        private Collider2D frame6To7Trigger;
        [SerializeField, Tooltip("The trigger that initiates the transition from frame 7 to 6.")]
        private Collider2D frame7To6Trigger;
        
        [Header("Helmet interaction settings")]
        [SerializeField, Tooltip("The placement for the helmet.")]
        private Vector3 helmetPlacement;
        [SerializeField] private GameObject helmetObject;
        [SerializeField] private Animator leftDoorAnimator;
        [SerializeField] private Animator rightDoorAnimator;

        [Header("Last frame interaction settings")] 
        [SerializeField] private Collider2D lastFrameTrigger;
        [SerializeField] private GameObject textBubble1;
        [SerializeField] private GameObject textBubble2;

        private SpriteRenderer _spriteRenderer;
        private float _elevatorOffsetY;
        private bool _hasHelmet = false;
        private bool hasActivatedLastFrameSequence = false;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (_hasHelmet) return;
            helmetObject.transform.SetParent(gameObject.transform);
            helmetObject.transform.localPosition = helmetPlacement;
            _hasHelmet = true;
            leftDoorAnimator.SetTrigger(Open);
            rightDoorAnimator.SetTrigger(Open);
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
            
            if (other == lastFrameTrigger && !hasActivatedLastFrameSequence)
            {
                StartCoroutine(LastFrameSequenceCoroutine());
                hasActivatedLastFrameSequence = true;
            }
            
            if(other.CompareTag("End")) 
            {
                SceneLoader.Instance?.LoadScene(nextSceneName);
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

        private IEnumerator LastFrameSequenceCoroutine()
        {
            textBubble1.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            textBubble2.SetActive(true);
        }
    }
}