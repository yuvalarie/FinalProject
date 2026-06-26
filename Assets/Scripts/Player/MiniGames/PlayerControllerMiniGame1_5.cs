using System;
using System.Collections;
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
            var offsetPosition = new Vector3(transform.position.x, transform.position.y + interactVerticalOffset, transform.position.z);
            Collider2D hit = Physics2D.OverlapCircle(offsetPosition, interactRadius, answerLayer);

            if (hit != null)
            {
                Transform selectedItem = hit.transform;
                QuizStage currentStage = quizStages[_currentStageIndex];

                if (hit.CompareTag("Answer"))
                {
                    _currentlyHoveredAnswer.SwitchToChosen();
                    StartCoroutine(AnswerSelectionRoutine(selectedItem, currentStage));
                }
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
                _interactionCount = 2; 
                text2.SetActive(true);
                ResetHandPosition();
                _isAnimating = false;
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
            TrailRenderer[] trails = GetComponentsInChildren<TrailRenderer>();
            foreach (var trail in trails)
            {
                trail.Clear();
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Answer"))
            {
                QuizAnswerObject answer = collision.GetComponent<QuizAnswerObject>();
                if (answer != null)
                {
                    if (answer != _currentlyHoveredAnswer && _currentlyHoveredAnswer != null)
                    {
                        _currentlyHoveredAnswer.SwitchToOriginal();
                    }
                    _currentlyHoveredAnswer = answer;
                    _currentlyHoveredAnswer.SwitchToHover();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Answer"))
            {
                QuizAnswerObject answer = collision.GetComponent<QuizAnswerObject>();
                if (_currentlyHoveredAnswer == answer)
                {
                    _currentlyHoveredAnswer.SwitchToOriginal();
                    _currentlyHoveredAnswer = null;
                }
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