using System;
using System.Collections;
using Objects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerControllerPage6 : PlayerControllerBase
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
        [SerializeField] private HeldaAnimatorPage6 heldaMovement;
        
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
        [SerializeField] private Transform letterLocation;
        [SerializeField] private Animator ricePotAnimator;

        [Header("Sprite Settings")]
        [SerializeField] private SizeSettings frame1Size;
        [SerializeField] private SizeSettings frame2Size;
        [SerializeField] private SizeSettings frame4Size;
        [SerializeField] private SizeSettings frame6Size;
        
        [Header("Trigger Settings")]
        [SerializeField] private Collider2D frame1toframe2Trigger;
        [SerializeField] private Collider2D frame2toframe1Trigger;
        [SerializeField] private Collider2D frame2toframe4Trigger;
        [SerializeField] private Collider2D frame4toframe2Trigger;
        [SerializeField] private Collider2D frame4toframe6Trigger;
        [SerializeField] private Collider2D frame6toframe4Trigger;
        [SerializeField] private Collider2D letterTrigger;
        [SerializeField] private Collider2D ricePotTrigger;

        // State Machine Variables
        private int _interactionCount = 0;
        private bool _isSequenceActive = false;
        private bool _isAnimating = false;
        private bool _axisInUse = false;
        private bool _isWaitingForHelda = false; // New flag to freeze the player!
        private Coroutine _currentSequenceRoutine;

        // Sequence 1 (Frame 2) Readiness
        private bool _isHeldaReadyForSeq1 = false;
        private bool _isPlayerAtSeq1 = false;
        private bool _seq1Done = false;

        // Sequence 2 (Letter) Readiness
        private bool _isHeldaReadyForSeq2 = false;
        private bool _isPlayerAtSeq2 = false;
        private bool _seq2Done = false;
        
        // RicePot
        private bool _atRicePot;

        protected override void Start()
        {
            base.Start();
        }
        
        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if(_atRicePot) ricePotAnimator.SetTrigger("Play");
        }

        protected override void HandleMovement()
        {
            // Lock the player in place if a sequence is running OR if they are waiting for Helda
            if (_isSequenceActive || _isWaitingForHelda)
            {
                if (Rb != null) Rb.linearVelocity = Vector2.zero;
                return;
            }
            
            base.HandleMovement();
        }

        private void Update()
        {
            if (!_isSequenceActive) return;

            if (Mathf.Abs(MoveInput.x) < 0.1f)
            {
                _axisInUse = false;
            }

            if (_isAnimating || _axisInUse) return;

            // Move Forward
            if (MoveInput.x > 0.5f)
            {
                // Prevent overstepping boundaries
                if ((!_seq1Done && _interactionCount >= 4) || (_seq1Done && _interactionCount >= 8)) return;
                
                _axisInUse = true;
                _interactionCount++;
                if (_currentSequenceRoutine != null) StopCoroutine(_currentSequenceRoutine);
                _currentSequenceRoutine = StartCoroutine(TransitionToState(_interactionCount, true));
            }
            // Move Backward
            else if (MoveInput.x < -0.5f)
            {
                // Prevent walking back before the start of the current sequence
                if ((!_seq1Done && _interactionCount <= 1) || (_seq1Done && _interactionCount <= 5)) return;
                
                _axisInUse = true;
                _interactionCount--;
                if (_currentSequenceRoutine != null) StopCoroutine(_currentSequenceRoutine);
                _currentSequenceRoutine = StartCoroutine(TransitionToState(_interactionCount, false));
            }
        }

        private IEnumerator TransitionToState(int targetState, bool isMovingForward)
        {
            _isAnimating = true;

            // Clear all UI elements first to avoid overlaps
            textBubble1.SetActive(false);
            textBubble2.SetActive(false);
            textBubble3.SetActive(false);
            textBubble4.SetActive(false);
            textBubble5.SetActive(false);
            closedLetterObject.SetActive(false);
            openLetterObject.SetActive(false);

            switch (targetState)
            {
                /* --- SEQUENCE 1 (Frame 2 text and magnifier) --- */
                case 1:
                    if (!isMovingForward) 
                    {
                        magnifierObject.BackTransition(); // Player pressed back from the magnifier
                        yield return new WaitForSeconds(magnifierObject.AnimationDuration);
                    }
                    textBubble1.SetActive(true);
                    break;
                
                case 2:
                    if (isMovingForward)
                    {
                        magnifierObject.FrameTransition();
                        yield return new WaitForSeconds(magnifierObject.AnimationDuration);
                    }
                    textBubble2.SetActive(true);
                    yield return new WaitForSeconds(1f);
                    textBubble3.SetActive(true);
                    break;
                
                case 3:
                    yield return new WaitForSeconds(0.1f);
                    textBubble4.SetActive(true);
                    break;
                
                case 4: // End of Sequence 1
                    if (isMovingForward)
                    {
                        magnifierObject.BackTransition();
                        // Wait for the magnifier to fully close before sending Helda away and unfreezing the player
                        yield return new WaitForSeconds(magnifierObject.AnimationDuration); 
                        heldaMovement.PlayMovement3(); 
                    }
                    
                    _isSequenceActive = false;
                    _seq1Done = true;
                    frame2Collider.enabled = false;
                    break;

                /* --- SEQUENCE 2 (Frame 5 text and letter) --- */
                case 5:
                    textBubble5.SetActive(true);
                    break;
                
                case 6:
                    closedLetterObject.transform.parent = transform;
                    closedLetterObject.transform.localPosition = letterLocation.position;
                    closedLetterObject.SetActive(true);
                    break;
                
                case 7:
                    openLetterObject.SetActive(true);
                    break;
                
                case 8: // End of Sequence 2
                    _isSequenceActive = false;
                    _seq2Done = true;
                    letterTrigger.enabled = false;
                    
                    if (isMovingForward)
                    {
                        heldaMovement.PlayMovement5();
                    }
                    break;
            }

            _isAnimating = false;
        }

        private void TryStartSequence1()
        {
            if (_isHeldaReadyForSeq1 && _isPlayerAtSeq1 && !_seq1Done)
            {
                _isWaitingForHelda = false; // Unfreeze the waiting state
                _isSequenceActive = true;
                _interactionCount = 1;
                
                // Fix for the first bubble skipping: Mark the axis as in use immediately!
                if (Mathf.Abs(MoveInput.x) > 0.1f) _axisInUse = true; 
                
                if (_currentSequenceRoutine != null) StopCoroutine(_currentSequenceRoutine);
                _currentSequenceRoutine = StartCoroutine(TransitionToState(_interactionCount, true));
            }
        }

        private void TryStartSequence2()
        {
            if (_isHeldaReadyForSeq2 && _isPlayerAtSeq2 && !_seq2Done)
            {
                _isWaitingForHelda = false; // Unfreeze the waiting state
                _isSequenceActive = true;
                _interactionCount = 5;
                
                // Fix for skipping: Mark the axis as in use immediately!
                if (Mathf.Abs(MoveInput.x) > 0.1f) _axisInUse = true;
                
                if (_currentSequenceRoutine != null) StopCoroutine(_currentSequenceRoutine);
                _currentSequenceRoutine = StartCoroutine(TransitionToState(_interactionCount, true));
            }
        }

        /* --- Helda Animation Callbacks --- */
        
        public void OnAnimation1Complete()
        {
            if (heldaSpriteRenderer != null && heldaFrame2Sprite != null)
            {
                heldaSpriteRenderer.sprite = heldaFrame2Sprite;
                heldaObject.transform.localScale = heldaFrame2Scale;
            }
            heldaMovement.PlayMovement2();
        }
        
        public void StartTextBubble1Sequence()
        {
            _isHeldaReadyForSeq1 = true;
            TryStartSequence1();
        }
        
        public void StartAnimation4()
        {
            heldaSpriteRenderer.sprite = heldaFrame4Sprite;
            heldaObject.transform.localScale = heldaFrame4Scale;
            heldaMovement.PlayMovement4();
        }
        
        public void ShowTextBubble5()
        {
            _isHeldaReadyForSeq2 = true;
            TryStartSequence2();
        }

        public void OnAnimation5Complete()
        {
        }
        
        /* --- Triggers --- */
        
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other == ricePotTrigger) _atRicePot = true;

            if (other == frame2Collider)
            {
                _isPlayerAtSeq1 = true;
                _isWaitingForHelda = true; // Officially freeze the player!
                if (Rb != null) Rb.linearVelocity = Vector2.zero;
                
                TryStartSequence1(); 
            }
            
            if (other == letterTrigger)
            {
                _isPlayerAtSeq2 = true;
                _isWaitingForHelda = true; // Officially freeze the player!
                if (Rb != null) Rb.linearVelocity = Vector2.zero;
                
                TryStartSequence2();
            }

            if (other == frame1toframe2Trigger) { CurrentSize = frame2Size; SetSize(); }
            if (other == frame2toframe1Trigger) { CurrentSize = frame1Size; SetSize(); }
            if (other == frame2toframe4Trigger) { CurrentSize = frame4Size; SetSize(); }
            if (other == frame4toframe2Trigger) { CurrentSize = frame2Size; SetSize(); }
            if (other == frame4toframe6Trigger) { CurrentSize = frame6Size; SetSize(); }
            if (other == frame6toframe4Trigger) { CurrentSize = frame4Size; SetSize(); }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other == letterTrigger)
            {
                _isPlayerAtSeq2 = false;
            }
            
            if (other == ricePotTrigger) _atRicePot = false;
        }
    }
}