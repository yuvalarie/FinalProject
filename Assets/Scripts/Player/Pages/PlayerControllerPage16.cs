using System.Collections;
using Audio.AudioEmitters;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player.Pages
{
    public class PlayerControllerPage16 : PlayerControllerBase
    {
        [Header("Camera Settings")]
        [SerializeField] private Animator cameraAnimator;
        [SerializeField] private float cameraAnimationDuration;
        
        [Header("Helda Settings")]
        [SerializeField] private GameObject heldaObject;
        [SerializeField] private GameObject heldaText1;
        [SerializeField] private GameObject heldaText2;
        [SerializeField] private Transform heldaEnterPosition;
        [SerializeField] private float heldaEnterDuration;
        [SerializeField, Tooltip("How high she bounces on the Y axis while moving.")] 
        private float jumpPower = 0.2f;
        [SerializeField, Tooltip("How many 'hops' she takes to reach the target.")]
        private int numberOfJumps = 3;

        [Header("Footstep SFX")]
        [SerializeField] private AudioEmitterBase heldaFootstepSfx;

        [Header("Letter Settings")]
        [SerializeField] private GameObject nedText;
        [SerializeField] private GameObject smallLetter;
        [SerializeField] private GameObject bigLetter;
        
        [Header("Animation Settings")]
        [SerializeField] private GameObject finalAnimationObject;
        [SerializeField] private float finalAnimationDuration;
        [SerializeField] private float waitBeforeFinalAnimation;
        [SerializeField] private float waitAfterFinalAnimation;
        [SerializeField] private float waitAfterFinalAnimationBeforeZoomOut;
        [SerializeField] private float waiBeforeNedText;
        
        [Header("Trigger Settings")]
        [SerializeField] private Collider2D startSequenceTrigger;
        [SerializeField] private Collider2D heldaCollider;

        [Header("End Sequence Settings")] 
        [SerializeField] private GameObject poofObject;
        [SerializeField] private Animator poofAnimator;
        [SerializeField] private float poofDuration;
        [SerializeField] private GameObject blackScreen;
        [SerializeField] private float blackScreenFadeDuration;

        private bool hasHeldaSequenceStarted;
        private bool hasAnimationPlayed;
        private bool hasAnimationEnded;

        private bool atHeldaCollider;
        
        private bool isHeldaTextSequenceActive;
        private bool isHeldaTextSequenceAnimating;
        private int heldaTextSequenceState;
        private Coroutine currentHeldaTextSequenceRoutine;
        
        private bool isTextSequenceActive;
        private bool isTextSequenceAnimating;
        private int textSequenceState;
        private Coroutine currentTextSequenceRoutine;
        private bool hasTextSequenceEnded;
        
        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if(!context.performed) return;
            if(hasTextSequenceEnded && atHeldaCollider)
            {
                StartCoroutine(EndSequenceCoroutine());
            }
        }

        private IEnumerator EndSequenceCoroutine()
        {
            poofObject.SetActive(true);
            poofAnimator.SetTrigger("Poof");
            yield return new WaitForSeconds(poofDuration);
            blackScreen.SetActive(true);
            yield return new WaitForSeconds(blackScreenFadeDuration);
            SceneLoader.Instance.ActivatePreloadedScene();
        }

        protected override void HandleMovement()
        {
            if (isHeldaTextSequenceActive || isTextSequenceActive)
            {
                if (Rb != null) Rb.linearVelocity = Vector2.zero;
                return;
            }
            
            if(!hasHeldaSequenceStarted || (hasHeldaSequenceStarted && hasAnimationEnded)) base.HandleMovement();
        }

        protected override void OnTextForward(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (isHeldaTextSequenceActive)
            {
                if (isHeldaTextSequenceAnimating || heldaTextSequenceState >= 3) return;

                heldaTextSequenceState++;
                if (currentHeldaTextSequenceRoutine != null) StopCoroutine(currentHeldaTextSequenceRoutine);
                currentHeldaTextSequenceRoutine = StartCoroutine(TransitionToHeldaTextState(heldaTextSequenceState));
                return;
            }

            if (!isTextSequenceActive || isTextSequenceAnimating || textSequenceState >= 4) return;

            textSequenceState++;
            if (currentTextSequenceRoutine != null) StopCoroutine(currentTextSequenceRoutine);
            currentTextSequenceRoutine = StartCoroutine(TransitionToTextState(textSequenceState));
        }

        protected override void OnTextBackward(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (isHeldaTextSequenceActive)
            {
                if (isHeldaTextSequenceAnimating || heldaTextSequenceState <= 1) return;

                heldaTextSequenceState--;
                if (currentHeldaTextSequenceRoutine != null) StopCoroutine(currentHeldaTextSequenceRoutine);
                currentHeldaTextSequenceRoutine = StartCoroutine(TransitionToHeldaTextState(heldaTextSequenceState));
                return;
            }

            if (!isTextSequenceActive || isTextSequenceAnimating || textSequenceState <= 1) return;

            textSequenceState--;
            if (currentTextSequenceRoutine != null) StopCoroutine(currentTextSequenceRoutine);
            currentTextSequenceRoutine = StartCoroutine(TransitionToTextState(textSequenceState));
        }

        private void StartHeldaTextSequence()
        {
            BeginTextInputMode();
            isHeldaTextSequenceActive = true;
            heldaTextSequenceState = 1;
            
            if (currentHeldaTextSequenceRoutine != null) StopCoroutine(currentHeldaTextSequenceRoutine);
            currentHeldaTextSequenceRoutine = StartCoroutine(TransitionToHeldaTextState(heldaTextSequenceState));
        }

        private IEnumerator TransitionToHeldaTextState(int targetState)
        {
            isHeldaTextSequenceAnimating = true;

            heldaText1.SetActive(false);
            heldaText2.SetActive(false);

            switch (targetState)
            {
                case 1:
                    heldaText1.SetActive(true);
                    break;
                case 2:
                    heldaText2.SetActive(true);
                    break;
                case 3:
                    isHeldaTextSequenceActive = false;
                    EndTextInputMode();
                    if (!hasAnimationPlayed)
                    {
                        hasAnimationPlayed = true;
                        StartCoroutine(FinalAnimationCoroutine());
                    }
                    break;
            }

            yield return null;
            isHeldaTextSequenceAnimating = false;
        }

        private IEnumerator FinalAnimationCoroutine()
        {
            heldaText1.SetActive(false);
            heldaText2.SetActive(false);
            cameraAnimator.SetTrigger("ZoomIn");
            yield return new WaitForSeconds(cameraAnimationDuration);
            yield return new WaitForSeconds(waitBeforeFinalAnimation);
            finalAnimationObject.SetActive(true);
            //cameraAnimator.SetTrigger("ZoomOutSize");
            yield return new WaitForSeconds(cameraAnimationDuration);
            yield return new WaitForSeconds(finalAnimationDuration);
            //cameraAnimator.SetTrigger("ZoomInSize");
            yield return new WaitForSeconds(cameraAnimationDuration);
            yield return new WaitForSeconds(waitAfterFinalAnimation);
            finalAnimationObject.SetActive(false);
            yield return new WaitForSeconds(waitAfterFinalAnimationBeforeZoomOut);
            cameraAnimator.SetTrigger("ZoomOut");
            yield return new WaitForSeconds(cameraAnimationDuration);
            hasAnimationEnded = true;
            SceneLoader.Instance.PreloadScene(nextSceneName);
            yield return new WaitForSeconds(waiBeforeNedText);
            StartTextSequence();
        }

        private void StartTextSequence()
        {
            BeginTextInputMode();
            isTextSequenceActive = true;
            textSequenceState = 1;
            
            if (currentTextSequenceRoutine != null) StopCoroutine(currentTextSequenceRoutine);
            currentTextSequenceRoutine = StartCoroutine(TransitionToTextState(textSequenceState));
        }

        private IEnumerator TransitionToTextState(int targetState)
        {
            isTextSequenceAnimating = true;

            nedText.SetActive(false);
            smallLetter.SetActive(false);
            bigLetter.SetActive(false);

            switch (targetState)
            {
                case 1:
                    nedText.SetActive(true);
                    break;
                case 2:
                    smallLetter.SetActive(true);
                    break;
                case 3:
                    bigLetter.SetActive(true);
                    break;
                case 4:
                    isTextSequenceActive = false;
                    hasTextSequenceEnded = true;
                    EndTextInputMode();
                    break;
            }

            yield return null;
            isTextSequenceAnimating = false;
        }

        private void PlayHeldaFootstepSfx(float duration)
        {
            float interval = duration / numberOfJumps;
            for (int i = 1; i <= numberOfJumps; i++)
            {
                DOVirtual.DelayedCall(interval * i, () => heldaFootstepSfx?.PlayAudioOnce());
            }
        }

        private void HeldaSequence()
        {
            PlayHeldaFootstepSfx(heldaEnterDuration);
            heldaObject.transform.DOJump(heldaEnterPosition.position, jumpPower, numberOfJumps, heldaEnterDuration)
                .SetEase(Ease.Linear)
                .OnComplete(StartHeldaTextSequence);
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other == startSequenceTrigger && !hasHeldaSequenceStarted)
            {
                if (Rb != null) Rb.linearVelocity = Vector2.zero;
                hasHeldaSequenceStarted = true;
                HeldaSequence();
            }
            
            if (other == heldaCollider)
            {
                atHeldaCollider = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other == heldaCollider)
            {
                atHeldaCollider = false;
            }
        }
    }
}


