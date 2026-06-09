using System;
using System.Collections;
using Objects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerControllerPage5 : PlayerControllerBase
    {
        [Header("Helda Settings")]
        [SerializeField] private Sprite heldaFrame1Sprite;
        [SerializeField] private Vector3 heldaFrame1Scale;
        [SerializeField] private Sprite heldaFrame2Sprite;
        [SerializeField] private Vector3 heldaFrame2Scale;
        [SerializeField] private Sprite heldaFrame4Sprite;
        [SerializeField] private Vector3 heldaFrame4Scale;
        [SerializeField] private SpriteRenderer heldaSpriteRenderer;
        [SerializeField] private GameObject heldaObject;
        [SerializeField] private HeldaAnimatorPage5 heldaMovement;
        
        [Header("Text Settings")]
        [SerializeField] private GameObject textBubble1;
        [SerializeField] private GameObject textBubble2;
        [SerializeField] private GameObject textBubble3;
        [SerializeField] private GameObject textBubble4;
        [SerializeField] private GameObject textBubble5;
        [SerializeField] private GameObject closedLetterObject;
        [SerializeField] private GameObject openLetterObject;
        
        [Header("Interaction Settings")]
        [SerializeField] private MagnifierFollowObject magnifierObject;
        [SerializeField] private Collider2D frame2Collider;
        
        [Header("Sprite Settings")]
        [SerializeField] private Sprite frame1Sprite;
        [SerializeField] private Sprite frame2Sprite;
        [SerializeField] private Sprite frame4Sprite;
        [SerializeField] private Sprite frame6Sprite;
        [SerializeField] private Vector3 frame1Scale;
        [SerializeField] private Vector3 frame2Scale;
        [SerializeField] private Vector3 frame4Scale;
        [SerializeField] private Vector3 frame6Scale;
        
        [Header("Trigger Settings")]
        [SerializeField] private Collider2D frame1toframe2Trigger;
        [SerializeField] private Collider2D frame2toframe1Trigger;
        [SerializeField] private Collider2D frame2toframe4Trigger;
        [SerializeField] private Collider2D frame4toframe2Trigger;
        [SerializeField] private Collider2D frame4toframe6Trigger;
        [SerializeField] private Collider2D frame6toframe4Trigger;
        [SerializeField] private Collider2D letterTrigger;

        private bool _canMove;
        private bool textBubble1Shown;
        private bool textBubble2Shown;
        private bool textBubble4Shown;
        private bool textBubble5Shown;
        private bool textBubble5Cleared; // Added to prevent re-triggering animation 5
        private bool letterShown;
        private bool letterOpened;
        private bool letterCleared;
        
        // NEW STATE TRACKERS FOR THE LETTER LOGIC
        private bool _isPlayerInLetterCollider;
        private bool _wasInLetterCollider;
        private bool _isAnimation5Complete;
        
        private SpriteRenderer _spriteRenderer;
        
        public static event Action OnSequenceComplete;
        public static event Action OnBackSequenceComplete;
        
        protected override void Start()
        {
            //base.Start();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _canMove = true;
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            OnSequenceComplete += StartTextSequence;
            OnBackSequenceComplete += StartExitAnimation;
        }
        
        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (textBubble1Shown && !textBubble2Shown)
            {
                StartMagnifierSequence();
            }
            if(textBubble1Shown && textBubble2Shown && !textBubble4Shown)
            {
                StartCoroutine(StartTextBubble4Coroutine());
            }
            if (textBubble1Shown && textBubble2Shown && textBubble4Shown && !textBubble5Shown)
            {
                magnifierObject.BackTransition();
            }
            
            // Clear Bubble 5 and start final movement
            if(textBubble1Shown && textBubble2Shown && textBubble4Shown && textBubble5Shown && !textBubble5Cleared)
            {
                textBubble5.SetActive(false);
                textBubble5Cleared = true;
                heldaMovement.PlayMovement5();
            }
            
            // Open the letter ONLY if player is in the collider
            if (letterShown && !letterOpened && _isPlayerInLetterCollider)
            {
                closedLetterObject.SetActive(false);
                openLetterObject.SetActive(true);
                letterOpened = true;
            }
            else if (letterShown && letterOpened)
            {
                openLetterObject.SetActive(false);
                letterCleared = true;
                _canMove = true;
            }
        }

        protected override void HandleMovement()
        {
            if (_canMove) base.HandleMovement();
        }
        
        /* --- Frame 1 Sequence --- */
        public void OnAnimation1Complete()
        {
            if (heldaSpriteRenderer != null && heldaFrame2Sprite != null)
            {
                heldaSpriteRenderer.sprite = heldaFrame2Sprite;
                heldaObject.transform.localScale = heldaFrame2Scale;
            }
            heldaMovement.PlayMovement2();
        }
        
        /* --- Frame 2 Sequence --- */
        public void StartTextBubble1Sequence()
        {
            textBubble1.SetActive(true);
            textBubble1Shown = true;
        }
        
        private void StartMagnifierSequence()
        {
            textBubble1.SetActive(false);
            magnifierObject.FrameTransition();
        }
        
        public static void TriggerSequenceComplete()
        {
            OnSequenceComplete?.Invoke();
        }
        
        private void StartTextSequence()
        {
            StartCoroutine(StartTextCoroutine());
        }
        
        private IEnumerator StartTextCoroutine()
        {
            textBubble1.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            if (textBubble3 != null && textBubble2 != null)
            {
                textBubble2.SetActive(true);
                textBubble2Shown = true;
                yield return new WaitForSeconds(0.1f);
                textBubble3.SetActive(true);
            }
        }
        
        private IEnumerator StartTextBubble4Coroutine()
        {
            textBubble2.SetActive(false);
            textBubble3.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            if (textBubble4 != null)
            {
                textBubble4.SetActive(true);
                textBubble4Shown = true;
            }
        }
        
        public static void TriggerBackSequenceComplete()
        {
            OnBackSequenceComplete?.Invoke();
        }
        
        private void StartExitAnimation()
        {
            textBubble4.SetActive(false);
            _canMove = true;
            heldaMovement.PlayMovement3();
        }
        
        public void ChangeCanMoveState()
        {
            _canMove = true;
            frame2Collider.enabled = false;
        }
        
        /* --- Frame 4 Sequence --- */
        public void StartAnimation4()
        {
            heldaSpriteRenderer.sprite = heldaFrame4Sprite;
            heldaObject.transform.localScale = heldaFrame4Scale;
            heldaMovement.PlayMovement4();
        }
        
        public void ShowTextBubble5()
        {
            if (textBubble5 != null)
            {
                textBubble5.SetActive(true);
                textBubble5Shown = true;
            }
        }
        
        /* --- Frame 5 Sequence (Letter) --- */
        
        // This will be called by HeldaMovementPage5 when her final jump finishes
        public void OnAnimation5Complete()
        {
            _isAnimation5Complete = true;
            
            // If the player is ALREADY standing in the trigger zone, show the letter immediately
            if (_wasInLetterCollider && !letterShown)
            {
                closedLetterObject.SetActive(true);
                letterShown = true;
            }
        }
        
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);

            if (other == frame2Collider)

            {
                _canMove = false;

                Rigidbody2D rb = GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                }

            }

            if (other == frame1toframe2Trigger)
            {
                _spriteRenderer.sprite = frame2Sprite;
                transform.localScale = frame2Scale;

            }
            if (other == frame2toframe1Trigger)
            {
                _spriteRenderer.sprite = frame1Sprite;
                transform.localScale = frame1Scale;
            }
            if (other == frame2toframe4Trigger)
            {
                _spriteRenderer.sprite = frame4Sprite;
                transform.localScale = frame4Scale;
            }
            if (other == frame4toframe2Trigger)
            {
                _spriteRenderer.sprite = frame2Sprite;
                transform.localScale = frame2Scale;
            }
            if (other == frame4toframe6Trigger)
            {
                _spriteRenderer.sprite = frame6Sprite;
                transform.localScale = frame6Scale;
            }
            if (other == frame6toframe4Trigger)
            {
                _spriteRenderer.sprite = frame4Sprite;
                transform.localScale = frame4Scale;
            }
            
            if (other == letterTrigger)
            {
                _isPlayerInLetterCollider = true;
                _wasInLetterCollider = true;
                _canMove = false;
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                }
                // Show the letter only if animation 5 is complete
                if (_isAnimation5Complete && !letterShown)
                {
                    closedLetterObject.SetActive(true);
                    letterShown = true;
                }
            }
        }

        // Add OnTriggerExit2D so we know when the player walks away!
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other == letterTrigger)
            {
                _isPlayerInLetterCollider = false;
            }
        }
    }
}