using System;
using System.Collections;
using Audio.AudioEmitters;
using DG.Tweening;
using Managers;
using Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.MiniGames
{
    [Serializable]
    public class QuizStage
    {
        [Tooltip("The parent GameObject holding the text and answers for this stage.")]
        public GameObject stageParent;
        
        [Tooltip("Drag the question objects (Transforms) here in order.")]
        public Transform questionObject;
        
        [Tooltip("Drag the individual answer objects (Transforms) here in order.")]
        public Transform[] answerObjects;
    }
    
    public class PlayerControllerMiniGame1_5  : PlayerControllerBase
    {
        [Header("Chat Settings")] 
        [SerializeField] private float waitForText1;
        [SerializeField] private GameObject text1;
        [SerializeField] private GameObject text2;
        [SerializeField] private GameObject text3;
        [SerializeField] private GameObject text4;
        [SerializeField] private GameObject appLogo;
        [SerializeField] private float animationDuration;
        
        [Header("Quiz Settings")]
        [SerializeField] private QuizStage[] quizStages;
        
        [SerializeField, Tooltip("The layer your answers are on.")] 
        private LayerMask answerLayer;
        
        [SerializeField, Tooltip("How close the hand needs to be to choose an item.")] 
        private float interactRadius = 1f;
        [SerializeField] private float interactVerticalOffset;

        [SerializeField, Tooltip("How long to wait before showing the next question.")] 
        private float waitBeforeNextStage = 2f;
        [SerializeField] private float waitBetweenQandA;
        [SerializeField] private float waitBetweenAnswers;
        
        [Header("Movement Settings")]
        [SerializeField] private bool rotateTowardsMovement = true;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float rotationOffset = 0f;
        
        [Header("Audio Settings")]
        [SerializeField] private AudioEmitterBase hoverEmitter;
        [SerializeField] private AudioEmitterBase hoverExitEmitter;
        [SerializeField] private AudioEmitterBase chosenEmitter;
        

        private Vector2 _lastVelocity;

        private int _currentStageIndex = 0;
        private bool _isAnimating = false;
        private int _interactionCount = 0;
        private bool _disableText;
        private Vector3 _startPosition;
        
        private QuizAnswerObject _currentlyHoveredAnswer;

        protected override void Start()
        {
            base.Start();
            _startPosition = transform.position;
            StartCoroutine(OpeningCoroutine());
        }

        private IEnumerator OpeningCoroutine()
        {
            _isAnimating = true;
            yield return new WaitForSeconds(waitForText1);
            text1.SetActive(true);
            _interactionCount = 1;
            _isAnimating = false;
        }

        protected override void HandleMovement()
        {
            if (_isAnimating) return; 
            base.HandleMovement();
            RotateTowardsMovement(targetVelocity);
            
            // Continuously search for the closest answer while the player moves
            UpdateClosestAnswer(); 
        }
        
        private void RotateTowardsMovement(Vector2 velocity)
        {
            if (!rotateTowardsMovement)
                return;

            if (velocity.sqrMagnitude < 0.001f)
                return;

            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;

            Quaternion targetRotation =
                Quaternion.Euler(0f, 0f, angle + rotationOffset);

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime);
        }

        // --- NEW METHOD: Replaces the old Triggers ---
        private void UpdateClosestAnswer()
        {
            var offsetPosition = new Vector3(transform.position.x, transform.position.y + interactVerticalOffset, transform.position.z);
            
            // Get EVERYTHING inside the radius
            Collider2D[] hits = Physics2D.OverlapCircleAll(offsetPosition, interactRadius, answerLayer);

            QuizAnswerObject closestAnswer = null;
            float closestDistance = float.MaxValue;

            // Find which one is mathematically closest to the hand
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Answer"))
                {
                    float distance = Vector2.Distance(offsetPosition, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestAnswer = hit.GetComponent<QuizAnswerObject>();
                    }
                }
            }

            // If the closest answer has changed, update the visual hover states
            if (closestAnswer != _currentlyHoveredAnswer)
            {
                if (_currentlyHoveredAnswer != null)
                {
                    _currentlyHoveredAnswer.SwitchToOriginal();
                    hoverExitEmitter?.PlayAudioOnce();
                }

                _currentlyHoveredAnswer = closestAnswer;

                if (_currentlyHoveredAnswer != null)
                {
                    _currentlyHoveredAnswer.SwitchToHover();
                    hoverEmitter?.PlayAudioOnce();
                }
            }
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed || _isAnimating) return;

            switch (_interactionCount)
            {
                case 1:
                    if (!_disableText)
                    {
                        text1.SetActive(false);
                        _disableText = true;
                        StartCoroutine(SetStageActive());
                        ResetHandPosition();
                    }
                    else
                    {
                        QuizInteraction();
                    }
                    break;
                case 2:
                    text2.SetActive(false);
                    text3.SetActive(true);
                    _interactionCount++;
                    break;
                case 3:
                    text3.SetActive(false);
                    text4.SetActive(true);
                    _interactionCount++;
                    SceneLoader.Instance?.PreloadScene(nextSceneName);
                    break;
                case 4:
                    text4.SetActive(false);
                    appLogo.SetActive(true);
                    _interactionCount++;
                    break;
                case 5:
                    SceneLoader.Instance?.ActivatePreloadedScene();
                    break;
            }
        }

        private void QuizInteraction()
        {
            // Now, we just check if we already have a hovered item locked in!
            // No new physics checks are needed here.
            if (_currentlyHoveredAnswer != null)
            {
                Transform selectedItem = _currentlyHoveredAnswer.transform;
                QuizStage currentStage = quizStages[_currentStageIndex];

                _currentlyHoveredAnswer.SwitchToChosen();
                chosenEmitter?.PlayAudioOnce();
                StartCoroutine(AnswerSelectionRoutine(selectedItem, currentStage));
            }
        }

        private IEnumerator AnswerSelectionRoutine(Transform selectedItem, QuizStage currentStage)
        {
            _isAnimating = true;
            Rb.linearVelocity = Vector2.zero;

            foreach (Transform item in currentStage.answerObjects)
            {
                if (item == selectedItem)
                {
                    item.DOScale(item.localScale * 1.3f, 0.5f).SetEase(Ease.OutBack);
                }
                else
                {
                    item.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).OnComplete(() => item.gameObject.SetActive(false));
                }
            }

            yield return new WaitForSeconds(waitBeforeNextStage);
            
            // Clear the hover reference when moving to the next stage
            _currentlyHoveredAnswer = null; 
            
            selectedItem.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);

            currentStage.stageParent.SetActive(false);
            _currentStageIndex++;

            if (_currentStageIndex < quizStages.Length)
            {
                ResetHandPosition();
                StartCoroutine(SetStageActive());
            }
            else
            {
                text2.SetActive(true);
                yield return new WaitForSeconds(animationDuration);
                text2.SetActive(false);
                text3.SetActive(true);
                ResetHandPosition();
                _isAnimating = false;
                _interactionCount = 3; 
            }
        }

        private IEnumerator SetStageActive()
        {
            QuizStage currentStage = quizStages[_currentStageIndex];
            currentStage.stageParent.SetActive(true);
            currentStage.questionObject.gameObject.SetActive(true);
            yield return new WaitForSeconds(waitBetweenQandA);
            foreach (var ans in currentStage.answerObjects)
            {
                ans.gameObject.SetActive(true);
                yield return new WaitForSeconds(waitBetweenAnswers);
            }
            _isAnimating = false;
        }

        private void ResetHandPosition()
        {
            transform.position = _startPosition;
            transform.rotation = Quaternion.identity;
            TrailRenderer[] trails = GetComponentsInChildren<TrailRenderer>();
            foreach (var trail in trails)
            {
                trail.Clear();
            }
        }

        private void OnDrawGizmosSelected()
        {
            var offsetPosition = new Vector3(transform.position.x, transform.position.y + interactVerticalOffset, transform.position.z);
            Gizmos.color = Color.darkCyan;
            Gizmos.DrawWireSphere(offsetPosition, interactRadius);
        }
    }
}