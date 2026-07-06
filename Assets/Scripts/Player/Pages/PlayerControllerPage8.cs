using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerPage8 : PlayerControllerBase
    {
        [Header("Frames & Objects")]
        [SerializeField] private GameObject frame2;
        [SerializeField] private GameObject frame3;
        [SerializeField] private GameObject frame4;
        [SerializeField] private GameObject frame4Part2;
        [SerializeField] private GameObject frame3Object;
        [SerializeField] private GameObject frame5Object;
        [SerializeField] private GameObject helda;
        
        [Header("UI Elements")]
        [SerializeField] private GameObject textBubble1;
        [SerializeField] private GameObject textBubble2;
        [SerializeField] private GameObject smallLetter;
        [SerializeField] private GameObject letter;
        
        [Header("Colliders")]
        [SerializeField] private Collider2D frame5Collider;
        [SerializeField] private SizeSettings frame5Size;
        [SerializeField] private Collider2D frame1Collider;
        [SerializeField] private SizeSettings frame1Size;
        [SerializeField] private Collider2D freezeCollider;
        [SerializeField] private Collider2D frame3Collider;

        [Header("Wait Settings")] 
        [SerializeField] private float waitForFrame1;
        [SerializeField] private float waitForFrame2;
        [SerializeField] private float waitForFrame3;
        [SerializeField] private float waitForFrame4;

        // State Machine Variables
        private int _interactionCount = 0;
        private bool _isIntroRunning = false;
        private bool _isSequenceActive = false;
        private bool _isAnimating = false;
        private Coroutine _currentSequenceRoutine;

        private bool _hasLetterClosed = false;
        private bool _isTriggerFrozen = false;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            // Interactions are now handled entirely by movement!
        }

        protected override void HandleMovement()
        {
            // Lock the player in place if the intro is running, the text sequence is active, or they hit the freeze collider
            if (_isIntroRunning || _isSequenceActive || _isTriggerFrozen)
            {
                if (Rb != null) Rb.linearVelocity = Vector2.zero;
                return;
            }
            
            base.HandleMovement();
        }

        protected override void OnTextForward(InputAction.CallbackContext context)
        {
            if (!context.performed || !_isSequenceActive || _isAnimating || _interactionCount >= 5) return;

            _interactionCount++;
            if (_currentSequenceRoutine != null) StopCoroutine(_currentSequenceRoutine);
            _currentSequenceRoutine = StartCoroutine(TransitionToState(_interactionCount, true));
        }

        protected override void OnTextBackward(InputAction.CallbackContext context)
        {
            if (!context.performed || !_isSequenceActive || _isAnimating || _interactionCount <= 1) return;

            _interactionCount--;
            if (_currentSequenceRoutine != null) StopCoroutine(_currentSequenceRoutine);
            _currentSequenceRoutine = StartCoroutine(TransitionToState(_interactionCount, false));
        }

        private IEnumerator TransitionToState(int targetState, bool isMovingForward)
        {
            _isAnimating = true;

            // Clear all UI elements first to avoid overlaps
            textBubble1.SetActive(false);
            textBubble2.SetActive(false);
            letter.SetActive(false);
            smallLetter.SetActive(false);

            switch (targetState)
            {
                case 1:
                    textBubble1.SetActive(true);
                    break;
                
                case 2:
                    textBubble2.SetActive(true);
                    break;
                
                case 3:
                    smallLetter.SetActive(true);
                    break;
                
                case 4:
                    letter.SetActive(true);
                    break;
                
                case 5:
                    letter.SetActive(false);
                    _isSequenceActive = false;
                    EndTextInputMode();
                    _hasLetterClosed = true;
                    _isTriggerFrozen = false;
                    _isAnimating = false;
                    break;
            }

            yield return null;
            _isAnimating = false;
        }

        private IEnumerator SceneSequence()
        {
            BeginTextInputMode();
            _isIntroRunning = true;
            helda.SetActive(true);
            yield return new WaitForSeconds(waitForFrame1);
            if (frame2 != null) frame2.SetActive(true);
            yield return new WaitForSeconds(waitForFrame2);
            if (frame3 != null) frame3.SetActive(true);
            yield return new WaitForSeconds(waitForFrame3);
            if (frame4 != null) frame4.SetActive(true);
            yield return new WaitForSeconds(waitForFrame4);
            if (frame4Part2 != null) frame4Part2.SetActive(true);
            
            if (freezeCollider != null) freezeCollider.enabled = false;
            SceneLoader.Instance.PreloadScene(nextSceneName);
            
            yield return new WaitForSeconds(1f);
            
            // The automatic visual intro is done. Hand over control to the Movement State Machine.
            _isIntroRunning = false;
            _isSequenceActive = true;
            _interactionCount = 1;
            freezeCollider.enabled = false;
            
            if (_currentSequenceRoutine != null) StopCoroutine(_currentSequenceRoutine);
            _currentSequenceRoutine = StartCoroutine(TransitionToState(_interactionCount, true));
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);

            if (other == freezeCollider && !_isTriggerFrozen)
            {
                _isTriggerFrozen = true;
                if (Rb != null) Rb.linearVelocity = Vector2.zero;
                StartCoroutine(SceneSequence());
            }

            if (other == frame3Collider)
            {
                if (frame3Object != null)
                {
                    frame3Object.SetActive(true);
                    frame3Object.transform.parent = transform;
                }
                
                if (frame5Object != null)
                {
                    frame5Object.SetActive(true);
                    frame5Object.transform.parent = transform;
                }
            }
            
            if (other.CompareTag("End") && _hasLetterClosed)
            {
                SceneLoader.Instance?.ActivatePreloadedScene();
            }
            
            if (other == frame5Collider)
            {
                CurrentSize = frame5Size;
                SetSize();
            }
            
            if (other == frame1Collider)
            {
                CurrentSize = frame1Size;
                SetSize();
            }
        }
    }
}
