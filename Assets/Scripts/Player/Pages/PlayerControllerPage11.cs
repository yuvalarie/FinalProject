using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Pages
{
    public class PlayerControllerPage11 : PlayerControllerBase
    {
        [Header("Start Sequence")] 
        [SerializeField] private Animator door;
        [SerializeField] private GameObject helda;
        [SerializeField] private Transform targetPos;
        [SerializeField] private float animationDuration;
        
        [Header("Text Sequence")] 
        [SerializeField] private GameObject heldaState1;
        [SerializeField] private Animator radio;
        [SerializeField] private GameObject heldaState2;
        [SerializeField] private GameObject text3;
        [SerializeField] private GameObject text4;
        [SerializeField] private GameObject text5;
        [SerializeField] private GameObject smallLetter;
        [SerializeField] private float smallLetterAnimationDuration;
        [SerializeField] private GameObject bigLetter;
        [SerializeField] private float letterAnimationDuration;
        
        [Header("Bouncy Walk Settings")]
        [SerializeField, Tooltip("How long the movement takes.")] 
        private float duration = 1.5f;
        [SerializeField, Tooltip("How high she bounces on the Y axis while moving.")] 
        private float jumpPower = 0.2f;
        [SerializeField, Tooltip("How many 'hops' she takes to reach the target.")] 
        private int numberOfJumps = 3;

        [Header("Frame 2 back Transition Setting")] 
        [SerializeField] private Collider2D frame1Exit;
        [SerializeField] private Transform frame1ExitPos;
        [SerializeField] private Collider2D frame2BackEnter;
        [SerializeField] private Transform frame2BackEnterPos;
        [SerializeField] private Animator door1;
        [SerializeField] private SpriteRenderer door1SpriteRenderer;
        [SerializeField] private GameObject door2Open;
        [SerializeField] private GameObject door2Close;
        [SerializeField] private int backSortingOrder;
        [SerializeField] private SizeSettings backSize;
        [SerializeField] private Collider2D backCollider;
        [SerializeField] private float door1AnimationDuration;
        
        [Header("Frame 2 front Transition Setting")] 
        [SerializeField] private Collider2D frame2BackExit;
        [SerializeField] private Transform frame2BackExitPos;
        [SerializeField] private Collider2D frame2FrontEnter;
        [SerializeField] private Transform frame2FrontEnterPos;
        [SerializeField] private Animator door3;
        [SerializeField] private SpriteRenderer door3SpriteRenderer;
        [SerializeField] private GameObject door4Open;
        [SerializeField] private GameObject door4Close;
        [SerializeField] private int frontSortingOrder;
        [SerializeField] private SizeSettings frontSize;
        [SerializeField] private Collider2D frontCollider;

        [Header("Frame 4 Settings")] 
        [SerializeField] private Collider2D frame2Exit;
        [SerializeField] private Collider2D frame4Enter;
        [SerializeField] private SizeSettings frame4Size;
        [SerializeField] private Animator door5;
        
        private bool isSequenceActive = false;
        private bool isAnimating = false;
        private int interactionCount;

        private bool atBackCollider;
        private bool atFrontCollider;
        private bool isDoor1Open = false;
        private bool isDoor2Open = false;
        private bool isDoor4Open = false;
        private bool atDoor3;
        private bool isDoor3Open;

        protected override void Start()
        {
            base.Start();
            StartCoroutine(StartSequence());
        }

        private IEnumerator StartSequence()
        {
            BeginTextInputMode();
            isSequenceActive = true;
            isAnimating = true;
            door.SetTrigger("Open");
            yield return new WaitForSeconds(animationDuration);
            helda.transform.DOJump(targetPos.position, jumpPower, numberOfJumps, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    isAnimating = false;
                    helda.SetActive(false);
                    heldaState1.SetActive(true);
                    radio.SetTrigger("Text1");
                    interactionCount = 2;
                });
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (atBackCollider && !isDoor2Open)
            {
                Debug.Log("Opening door 2");
                door2Open.SetActive(true);
                door2Close.SetActive(false);
                isDoor2Open = true;
            }
            else if (atFrontCollider && !isDoor4Open && isDoor2Open)
            {
                Debug.Log("Opening door 4");
                door4Open.SetActive(true);
                door4Close.SetActive(false);
                door5.SetTrigger("Open");
                isDoor4Open = true;
            }
            else if (atDoor3 && !isDoor3Open)
            {
                StartCoroutine(Door3Coroutine());
            }
        }

        private IEnumerator Door3Coroutine()
        {
            door3.SetTrigger("Open");
            isDoor3Open = true;
            yield return new WaitForSeconds(door1AnimationDuration);
            var col = door3.GetComponent<Collider2D>();
            col.enabled = false;
            door3SpriteRenderer.sortingOrder = frontSortingOrder - 1;
        }

        protected override void HandleMovement()
        {
            if (isSequenceActive)
            {
                Rb.linearVelocity = Vector2.zero;
                return;
            }
            
            base.HandleMovement();
        }
        
        protected override void OnTextForward(InputAction.CallbackContext context)
        {
            if (!context.performed || !isSequenceActive || isAnimating || interactionCount >= 9) return;

            interactionCount++;
            StartCoroutine(TransitionToState(interactionCount, true));
        }

        protected override void OnTextBackward(InputAction.CallbackContext context)
        {
            if (!context.performed || !isSequenceActive || isAnimating || interactionCount <= 0) return;

            if(interactionCount > 2) interactionCount--;
            StartCoroutine(TransitionToState(interactionCount, false));
        }
        
        private IEnumerator TransitionToState(int targetState, bool isMovingForward)
        {
            if (targetState < 2) yield break;
            helda.SetActive(false);
            heldaState1.SetActive(false);
            heldaState2.SetActive(false);
            text3.SetActive(false);
            text4.SetActive(false);
            text5.SetActive(false);
            smallLetter.SetActive(false);
            bigLetter.SetActive(false);
            isAnimating = true;

            switch (targetState)
            {
                case 2:
                    heldaState1.SetActive(true);
                    radio.SetTrigger("Text1");
                    break;
                case 3:
                    heldaState1.SetActive(true);
                    radio.SetTrigger("Text2");
                    break;
                case 4:
                    radio.SetTrigger("Idle");
                    heldaState2.SetActive(true);
                    text3.SetActive(true);
                    break;
                case 5:
                    heldaState2.SetActive(true);
                    text4.SetActive(true);
                    break;
                case 6:
                    heldaState2.SetActive(true);
                    text5.SetActive(true);
                    break;
                case 7:
                    heldaState2.SetActive(true);
                    text5.SetActive(false);
                    smallLetter.SetActive(true);
                    yield return new WaitForSeconds(smallLetterAnimationDuration);
                    break;
                case 8:
                    heldaState2.SetActive(true);
                    bigLetter.SetActive(true);
                    yield return new WaitForSeconds(letterAnimationDuration);
                    break;
                case 9:
                    heldaState2.SetActive(true);
                    bigLetter.SetActive(false);
                    isSequenceActive = false;
                    EndTextInputMode();
                    break;
            }

            isAnimating = false;
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other == backCollider)
            {
                atBackCollider = true;
            }
            if (other == frontCollider)
            {
                atFrontCollider = true;
            }
            if (other == frame1Exit)
            {
                transform.position = frame1ExitPos.position;
            }
            else if (other == frame2BackEnter)
            {
                transform.position = frame2BackEnterPos.position;
                SpriteRenderer.sortingOrder = backSortingOrder;
                if(!isDoor1Open)
                {
                   StartCoroutine(Door1Coroutine());
                }
                else
                {
                    door1SpriteRenderer.sortingOrder = backSortingOrder - 1;
                    var col = door1.GetComponent<Collider2D>();
                    col.enabled = false;
                }
            }
            else if (other == frame2FrontEnter)
            {
                transform.position = frame2FrontEnterPos.position;
                SpriteRenderer.sortingOrder = frontSortingOrder;
                CurrentSize = frontSize;
                SetSize();
                atDoor3 = true;
                frame2BackEnter.enabled = false;
            }
            else if (other == frame2BackExit)
            {
                transform.position = frame2BackExitPos.position;
                SpriteRenderer.sortingOrder = backSortingOrder;
                CurrentSize = backSize;
                SetSize();
                frame2BackEnter.enabled = true;
            }
            else if (other == frame4Enter)
            {
                CurrentSize = frame4Size;
                SetSize();
            }
            else if (other == frame2Exit)
            {
                CurrentSize = frontSize;
                SetSize();
            }
        }

        private IEnumerator Door1Coroutine()
        {
            door1.SetTrigger("Open");
            isDoor1Open = true;
            yield return new WaitForSeconds(door1AnimationDuration);
            door1SpriteRenderer.sortingOrder = backSortingOrder - 1;
            var col = door1.GetComponent<Collider2D>();
            col.enabled = false;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other == backCollider)
            {
                atBackCollider = false;
            }
            if (other == frontCollider)
            {
                atFrontCollider = false;
            }
        }
    }
}
