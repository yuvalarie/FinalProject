using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using DG.Tweening;
using Managers;
using Objects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Player
{
    [System.Serializable]
    public struct SimonTask
    {
        public ToolType RequiredTool;
        public AreaType TargetArea;
    }
    
    [System.Serializable]
    public struct CombinationSpriteMapping
    {
        public ToolType Tool;
        public AreaType Area;
        public GameObject Sprite;
        public Vector3 position;
        public bool flip;
    }

    public class PlayerControllerMiniGame3 : PlayerControllerBase
    {
        [Header("Simon Game Settings")]
        [SerializeField, Tooltip("How many steps in the final sequence to win?")] 
        private int totalActionsToWin = 5;
        
        [Header("Hold Settings")]
        [SerializeField, Tooltip("Transform where the tool will sit when held by the player")] 
        private Transform holdSlot;
        [SerializeField, Tooltip("Add tools here if they face the opposite way by default and need inverted flipping logic")] 
        private List<ToolType> invertedFacingToolsList;
        [SerializeField] private float backToPlaceDuration;
        
        [Header("UI Screen Settings")]
        [SerializeField, Tooltip("How long should the next task be shown")] private float showTaskDuration;
        [SerializeField, Tooltip("How long should the next task be shown")] private float waitBetweenTasksDuration;
        [SerializeField, Tooltip("How long should the old task be shown")] private float waitForOldTask;
        
        [Header("Failure Feedback Settings")]
        [SerializeField, Tooltip("The mouth Transform to vibrate on failure")] 
        private Transform mouthTransform;
        [SerializeField, Tooltip("Duration of the mouth vibration")] 
        private float mouthVibrateDuration = 0.3f;
        [SerializeField, Tooltip("Strength of the mouth positional shake")] 
        private float mouthVibrateStrength = 0.2f;
        [SerializeField, Tooltip("How many times the mouth jitters")] 
        private int mouthVibrateVibrato = 15;
        [SerializeField, Tooltip("List of 4 GameObjects to randomly pick from on failure")] 
        private List<GameObject> failureObjectsList;
        [SerializeField, Tooltip("How long the failure object stays visible")] 
        private float failureObjectDisplayDuration = 1f;
        [SerializeField] private Animator bloodAnimator;
        
        [Header("Success Feedback Settings")]
        [SerializeField, Tooltip("Duration of the tool vibration when correct")] 
        private float toolVibrateDuration = 0.2f;
        
        [SerializeField, Tooltip("Map each tool+area combination to its specific sprite here")] 
        private List<CombinationSpriteMapping> combinationSpritesList;
        
        [Header("Detection Settings")]
        [SerializeField, Tooltip("How far the player can reach to detect areas and tools.")]
        private float detectionRadius = 1.5f;
        [SerializeField, Tooltip("The layer mask for your tools and areas.")]
        private LayerMask interactableLayer;
        [SerializeField] private Transform grabSlot;

        [Header("Current Status (Read Only)")]
        [SerializeField] private ToolType currentHeldTool = ToolType.None;
        [SerializeField] private AreaType currentStandingArea = AreaType.None;
        [SerializeField] private ToolType currentStandingToolStation = ToolType.None;

        [Header("End Sequence")] 
        [SerializeField] private GameObject endReaction;

        private List<SimonTask> fullSequence = new List<SimonTask>();
        private int currentRound = 1; 
        private int playerStepIndex = 0; 
        private bool isScreenPlaying = false; 
        
        private SimonInteractable _currentStandingToolInteractable;
        private GameObject _heldToolObject;
        private SpriteRenderer _heldToolSpriteRenderer;
        private Animator _heldToolAnimator;
        private Vector3 _heldToolOriginalLocalPosition;
        private Quaternion _heldToolOriginalLocalRotation;
        private Transform _heldToolOriginalParent;
        private Vector3 _playerStartPosition;
        private bool _isEnd;
        private bool _flipx;
        
        protected override void Start()
        {
            base.Start();
            _playerStartPosition = transform.position;
            GenerateSequence();
            StartCoroutine(PlaySequenceOnScreen());
        }

        protected override void HandleMovement()
        {
            if (isScreenPlaying)
            {
                if (Rb != null) Rb.linearVelocity = Vector2.zero;
                MoveInput = Vector2.zero; 
                return;
            }

            base.HandleMovement();
        }

        private void Update()
        {
            if (_heldToolSpriteRenderer != null && Mathf.Abs(MoveInput.x) > 0.1f && !isScreenPlaying)
            {
                bool isMovingLeft = MoveInput.x < 0;
                bool invertFlip = invertedFacingToolsList.Contains(currentHeldTool);

                if (invertFlip)
                {
                    _heldToolSpriteRenderer.flipX = !isMovingLeft;
                }
                else
                {
                    _heldToolSpriteRenderer.flipX = isMovingLeft;
                }
            }
        }

        private void GenerateSequence()
        {
            fullSequence.Clear();
            List<int> toolBag = new List<int>();
            List<int> areaBag = new List<int>();

            int lastToolUsed = -1;
            int lastAreaUsed = -1;

            for (int i = 0; i < totalActionsToWin; i++)
            {
                if (toolBag.Count == 0) toolBag.AddRange(new int[] { 1, 2, 3, 4 });
                if (areaBag.Count == 0) areaBag.AddRange(new int[] { 1, 2, 3, 4 });

                int randomToolIndex = Random.Range(0, toolBag.Count);
                
                if (toolBag.Count == 4 && toolBag[randomToolIndex] == lastToolUsed)
                {
                    randomToolIndex = (randomToolIndex + 1) % 4; 
                }
                
                int selectedToolInt = toolBag[randomToolIndex];
                toolBag.RemoveAt(randomToolIndex);
                lastToolUsed = selectedToolInt;

                int randomAreaIndex = Random.Range(0, areaBag.Count);
                
                if (areaBag.Count == 4 && areaBag[randomAreaIndex] == lastAreaUsed)
                {
                    randomAreaIndex = (randomAreaIndex + 1) % 4;
                }
                
                int selectedAreaInt = areaBag[randomAreaIndex];
                areaBag.RemoveAt(randomAreaIndex);
                lastAreaUsed = selectedAreaInt;

                SimonTask newTask = new SimonTask
                {
                    RequiredTool = (ToolType)selectedToolInt,
                    TargetArea = (AreaType)selectedAreaInt
                };
                fullSequence.Add(newTask);
            }
        }
        
        private void UpdateClosestInteractables()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(grabSlot.position, detectionRadius, interactableLayer);
            
            SimonInteractable closestTool = null;
            SimonInteractable closestArea = null;
            float minToolDist = float.MaxValue;
            float minAreaDist = float.MaxValue;

            foreach (var hit in hitColliders)
            {
                SimonInteractable interactable = hit.GetComponent<SimonInteractable>();
                if (interactable != null)
                {
                    // Calculate exact distance to this interactable
                    float dist = Vector2.Distance(transform.position, hit.transform.position);
                    
                    if (interactable.isToolStation && dist < minToolDist)
                    {
                        minToolDist = dist;
                        closestTool = interactable;
                    }
                    
                    if (interactable.isArea && dist < minAreaDist)
                    {
                        minAreaDist = dist;
                        closestArea = interactable;
                    }
                }
            }

            // Update Tool Station Reference
            if (closestTool != null)
            {
                currentStandingToolStation = closestTool.toolType;
                _currentStandingToolInteractable = closestTool;
            }
            else
            {
                currentStandingToolStation = ToolType.None;
                _currentStandingToolInteractable = null;
            }

            // Update Area Reference
            if (closestArea != null)
            {
                // Assuming your SimonInteractable script uses 'areaType' for the AreaType enum
                currentStandingArea = closestArea.areaType; 
            }
            else
            {
                currentStandingArea = AreaType.None;
            }
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed || isScreenPlaying) return;
            //if(_isEnd) SceneLoader.Instance.ActivatePreloadedScene();
            
            UpdateClosestInteractables();

            if (currentStandingArea != AreaType.None && currentHeldTool != ToolType.None)
            {
                ValidatePlayerAction(currentHeldTool, currentStandingArea);
                return;
            }
            
            if (currentStandingToolStation != ToolType.None && currentHeldTool == ToolType.None)
            {
                currentHeldTool = currentStandingToolStation;
                
                if (_currentStandingToolInteractable != null)
                {
                    _heldToolObject = _currentStandingToolInteractable.gameObject;
                    _heldToolOriginalLocalPosition = _heldToolObject.transform.localPosition;
                    _heldToolOriginalLocalRotation = _heldToolObject.transform.localRotation;
                    _heldToolOriginalParent = _heldToolObject.transform.parent;
                    _heldToolSpriteRenderer = _heldToolObject.GetComponent<SpriteRenderer>();
                    if (_heldToolSpriteRenderer == null)
                        _heldToolSpriteRenderer = _heldToolObject.GetComponentInChildren<SpriteRenderer>();
                    _heldToolAnimator = _heldToolObject.GetComponent<Animator>();
                    if (_heldToolAnimator == null)
                        _heldToolAnimator = _heldToolObject.GetComponentInChildren<Animator>();

                    if (holdSlot != null)
                    {
                        _heldToolObject.transform.SetParent(holdSlot);
                        _heldToolObject.transform.localPosition = Vector3.zero;
                    }
                }
                
                Debug.Log($"Picked up: {currentHeldTool}");
                return;
            }
        }
        
        private void ReturnHeldTool()
        {
            if (_heldToolObject != null)
            {
                _heldToolObject.transform.DOComplete();
                _heldToolObject.transform.SetParent(_heldToolOriginalParent);
                _heldToolObject.transform.localPosition = _heldToolOriginalLocalPosition;
                _heldToolObject.transform.localRotation = _heldToolOriginalLocalRotation;
                
                if (_heldToolSpriteRenderer != null)
                    _heldToolSpriteRenderer.flipX = false;
                    
                _heldToolSpriteRenderer = null;
                _heldToolAnimator = null;
                _heldToolObject = null;
            }
            currentHeldTool = ToolType.None;
        }

        private void ValidatePlayerAction(ToolType usedTool, AreaType appliedArea)
        {
            SimonTask expectedTask = fullSequence[playerStepIndex];
            
            if (usedTool == expectedTask.RequiredTool && appliedArea == expectedTask.TargetArea)
            {
                Debug.Log("Correct move!");
                StartCoroutine(HandleSuccessRoutine(usedTool, appliedArea));
            }
            else
            {
                Debug.Log("WRONG MOVE! Restarting this round.");
                StartCoroutine(HandleFailureRoutine());
            }
        }
        
        private void ClearInteractionStates()
        {
            currentStandingArea = AreaType.None;
            currentStandingToolStation = ToolType.None;
            _currentStandingToolInteractable = null;
        }

        private Vector3 FindPosition(ToolType tool, AreaType area)
        {
            foreach (var mapping in combinationSpritesList)
            {
                if (mapping.Tool == tool && mapping.Area == area)
                {
                    _flipx = mapping.flip;
                    return mapping.position;
                }
            }
            Debug.Log($"Didnt find pod for {tool}, {area}");
            return Vector3.zero;
        }

        private IEnumerator HandleSuccessRoutine(ToolType tool, AreaType area)
        {
            isScreenPlaying = true;
            _heldToolObject.transform.SetParent(null);
            transform.DOMove(_playerStartPosition, backToPlaceDuration);
            ClearInteractionStates();
            _heldToolObject.transform.localPosition = FindPosition(tool, area);
            
            if (_heldToolSpriteRenderer != null)
                _heldToolSpriteRenderer.flipX = _flipx;
                
            if(_heldToolAnimator != null) _heldToolAnimator.SetTrigger("Play");
            yield return new WaitForSeconds(toolVibrateDuration);
            ReturnHeldTool();
            
            playerStepIndex++;

            if (playerStepIndex >= currentRound)
            {
                if (currentRound >= totalActionsToWin)
                {
                    Debug.Log("MINIGAME WON!");
                    StartCoroutine(EndSequence());
                }
                else
                {
                    currentRound++;
                    MusicManager.Instance?.AdvanceProgress();
                    playerStepIndex = 0;
                    StartCoroutine(PlaySequenceOnScreen());
                }
            }
            else
            {
                isScreenPlaying = false;
            }
        }

        private IEnumerator EndSequence()
        {
            yield return new WaitForSeconds(0.5f);
            endReaction.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _isEnd = true;
        }

        private IEnumerator HandleFailureRoutine()
        {
            isScreenPlaying = true;
            if(bloodAnimator != null) bloodAnimator.SetTrigger("Play");
            
            if (mouthTransform != null)
            {
                mouthTransform.DOComplete();
                mouthTransform.DOShakePosition(mouthVibrateDuration, mouthVibrateStrength, mouthVibrateVibrato);
            }

            GameObject chosenFailureObject = null;
            if (failureObjectsList != null && failureObjectsList.Count > 0)
            {
                chosenFailureObject = failureObjectsList[Random.Range(0, failureObjectsList.Count)];
                if (chosenFailureObject != null)
                {
                    chosenFailureObject.SetActive(true);
                }
            }

            yield return new WaitForSeconds(failureObjectDisplayDuration);

            if (chosenFailureObject != null)
            {
                chosenFailureObject.SetActive(false);
            }

            playerStepIndex = 0; 
            ReturnHeldTool();
            transform.DOMove(_playerStartPosition, backToPlaceDuration);
            ClearInteractionStates();
            StartCoroutine(PlaySequenceOnScreen()); 
        }
        
        private GameObject GetSpriteForCombination(ToolType tool, AreaType area)
        {
            foreach (var mapping in combinationSpritesList)
            {
                if (mapping.Tool == tool && mapping.Area == area) 
                    return mapping.Sprite;
            }
            return null;
        }

        private IEnumerator PlaySequenceOnScreen()
        {
            isScreenPlaying = true;
            Debug.Log($"--- Displaying Sequence for Round {currentRound} ---");
            
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < currentRound; i++)
            {
                SimonTask taskToDisplay = fullSequence[i];
                Debug.Log($"SCREEN SHOWS: Tool {taskToDisplay.RequiredTool} at Area {taskToDisplay.TargetArea}");

                var display = GetSpriteForCombination(taskToDisplay.RequiredTool, taskToDisplay.TargetArea);
                if (display != null)
                {
                    display.SetActive(true);
                }
                
                if (i == currentRound - 1) yield return new WaitForSeconds(showTaskDuration);
                else yield return new WaitForSeconds(waitForOldTask);
                
                if (display != null) display.SetActive(false);
                
                yield return new WaitForSeconds(waitBetweenTasksDuration); 
            }
            
            if(currentRound  == 3) SceneLoader.Instance?.PreloadScene(nextSceneName);

            Debug.Log("Player's turn!");
            isScreenPlaying = false;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(grabSlot.position, detectionRadius);
        }
    }
}