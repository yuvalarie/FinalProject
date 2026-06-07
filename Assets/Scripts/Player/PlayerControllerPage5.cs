using System;
using System.Collections;
using Objects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    /**
     * --- this whole time dave can walk until frame 2 collider ----
     * Helda Animation 1 : plays on start
     * -> Triggers OnAnimation1Complete
     * OnAnimation1Complete : switch to frame 2 sprite, start animation 2
     * -> Triggers StartMagnifierSequence
     * -> Triggers OnMagnifierSequenceComplete
     * OnMagnifierSequenceComplete : show text bubble 1 after 0.5 seconds delay
     * OnInteraction after text bubble 1 shown : show text bubble 2
     * OnInteraction after text bubble 2 shown : magnifier back sequence
     * -> Triggers OnMagnifierBackSequenceComplete
     * OnMagnifierBackSequenceComplete : start exit animation
     * --- dave can walk again past frame 2 collider ---
     * OnExitAnimationComplete : switch to frame 4 sprite, frame 4 enter animation
     * Frame 4 enter animation complete : show text bubble 3
     * OnInteraction after text bubble 3 shown : text bubble 3 disappears, helda exit animation plays
     * Enter frame 5 : letters appears -> on interaction: letter opens -> on interaction: letter dissapears
     */
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
        [SerializeField] private Animator heldaAnimator;
        [SerializeField] private Animator legsAnimator;
        
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
        private bool letterShown;
        private bool letterOpened;
        
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
            if (textBubble1Shown && !textBubble2Shown)
            {
                StartCoroutine(StartTextCoroutine());
            }
            if(textBubble1Shown && textBubble2Shown && !textBubble4Shown)
            {
                StartCoroutine(StartTextBubble4Coroutine());
            }
            if (textBubble1Shown && textBubble2Shown && textBubble4Shown && !textBubble5Shown)
            {
                magnifierObject.BackTransition();
            }
            if(textBubble1Shown && textBubble2Shown && textBubble4Shown && textBubble5Shown && !letterShown)
            {
                textBubble5.SetActive(false);
                heldaAnimator.SetTrigger("Animation5");
            }
            if (letterShown && !letterOpened)
            {
                closedLetterObject.SetActive(false);
                openLetterObject.SetActive(true);
                letterOpened = true;
            }
            else if (letterShown && letterOpened)
            {
                openLetterObject.SetActive(false);
            }
        }

        protected override void HandleMovement()
        {
            if (_canMove) base.HandleMovement();
        }
        
        /* --- Frame 1 Sequence --- */
        
        // will be called by an animation event at the end of Helda's first animation
        public void OnAnimation1Complete()
        {
            if (heldaSpriteRenderer != null && heldaFrame2Sprite != null)
            {
                heldaSpriteRenderer.sprite = heldaFrame2Sprite;
                heldaObject.transform.localScale = heldaFrame2Scale;
            }
            heldaAnimator.SetTrigger("Animation2");
            legsAnimator.SetTrigger("Animation2");
        }
        
        /* --- Frame 2 Sequence --- */
        // will be called by an animation event at the end of Helda's second animation
        public void StartTextBubble1Sequence()
        {
            textBubble1.SetActive(true);
            textBubble1Shown = true;
        }
        
        // called after text bubble 1 is shown and player interacts
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
            heldaAnimator.SetTrigger("Animation3");
            legsAnimator.SetTrigger("Animation3");
        }
        
        // will be called by an animation event at the end of Helda's exit animation
        public void ChangeCanMoveState()
        {
            _canMove = true;
            frame2Collider.enabled = false;
        }
        
        /* --- Frame 4 Sequence --- */
        // will be called by an animation event at the end of Helda's exit animation
        public void StartAnimation4()
        {
            heldaSpriteRenderer.sprite = heldaFrame4Sprite;
            heldaObject.transform.localScale = heldaFrame4Scale;
            heldaAnimator.SetTrigger("Animation4");
        }
        
        // will be called by an animation event at the end of Helda's fourth animation
        public void ShowTextBubble5()
        {
            if (textBubble5 != null)
            {
                textBubble5.SetActive(true);
                textBubble5Shown = true;
            }
        }
        
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other == frame2Collider)
            {
                _canMove = false;
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

            if (other == letterTrigger && !letterShown)
            {
                closedLetterObject.SetActive(true);
                letterShown = true;
            }
        }

    }
}