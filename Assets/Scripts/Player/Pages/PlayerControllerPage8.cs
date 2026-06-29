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

        // State Machine Variables
        private int _interactionCount = 0;
        private bool _isIntroRunning = false;
        private bool _isSequenceActive = false;
        private bool _isAnimating = false;
        private bool _axisInUse = false;
        private Coroutine _currentSequenceRoutine;

        private bool _hasLetterClosed = false;
        private bool _isTriggerFrozen = false;

        protected override void Start()
        {
            base.Start();
            SceneLoader.Instance.PreloadScene(nextSceneName);
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

        private void Update()
        {
            if (!_isSequenceActive) return;

            if (Mathf.Abs(MoveInput.x) < 0.1f)
            {
                _axisInUse = false;
            }

            if (_isAnimating || _axisInUse) return;

            // Move Forward (Right)
            if (MoveInput.x > 0.5f)
            {
                // Prevent overstepping past the end of the sequence
                if (_interactionCount >= 5) return;
                
                _axisInUse = true;
                _interactionCount++;
                if (_currentSequenceRoutine != null) StopCoroutine(_currentSequenceRoutine);
                _currentSequenceRoutine = StartCoroutine(TransitionToState(_interactionCount, true));
            }
            // Move Backward (Left)
            else if (MoveInput.x < -0.5f)
            {
                // Prevent walking back before the start of the current sequence
                if (_interactionCount <= 1) return;
                
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
            _isIntroRunning = true;
            helda.SetActive(true);
            yield return new WaitForSeconds(5f);
            if (frame2 != null) frame2.SetActive(true);
            yield return new WaitForSeconds(1f);
            if (frame3 != null) frame3.SetActive(true);
            yield return new WaitForSeconds(1f);
            if (frame4 != null) frame4.SetActive(true);
            yield return new WaitForSeconds(1f);
            if (frame4Part2 != null) frame4Part2.SetActive(true);
            
            if (freezeCollider != null) freezeCollider.enabled = false;
            
            yield return new WaitForSeconds(1f);
            
            // The automatic visual intro is done. Hand over control to the Movement State Machine.
            _isIntroRunning = false;
            _isSequenceActive = true;
            _interactionCount = 1;
            
            // Mark axis in use immediately to prevent accidental double-skipping if they are holding Right
            if (Mathf.Abs(MoveInput.x) > 0.1f) _axisInUse = true;
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