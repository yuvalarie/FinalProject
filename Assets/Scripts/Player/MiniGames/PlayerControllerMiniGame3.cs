using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using Objects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    }

    public class PlayerControllerMiniGame3 : PlayerControllerBase
    {
        [Header("Simon Game Settings")]
        [SerializeField, Tooltip("How many steps in the final sequence to win?")] 
        private int totalActionsToWin = 5;
        
        [Header("Hold Settings")]
        [SerializeField, Tooltip("Transform where the tool will sit when held by the player")] 
        private Transform holdSlot;
        
        [Header("UI Screen Settings")]
        [SerializeField, Tooltip("How long should the next task be shown")] private float showTaskDuration;
        [SerializeField, Tooltip("How long should the next task be shown")] private float waitBetweenTasksDuration;
        
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
        
        [Header("Success Feedback Settings")]
        [SerializeField, Tooltip("Duration of the tool vibration when correct")] 
        private float toolVibrateDuration = 0.2f;
        [SerializeField, Tooltip("Strength of the tool vibration (Z-axis rotation)")] 
        private float toolVibrateStrength = 30f;
        [SerializeField, Tooltip("How many times it vibrates back and forth")] 
        private int toolVibrateVibrato = 10;
        
        [SerializeField, Tooltip("Map each tool+area combination to its specific sprite here")] 
        private List<CombinationSpriteMapping> combinationSpritesList;

        [Header("Current Status (Read Only)")]
        [SerializeField] private ToolType currentHeldTool = ToolType.None;
        [SerializeField] private AreaType currentStandingArea = AreaType.None;
        [SerializeField] private ToolType currentStandingToolStation = ToolType.None;

        [Header("End Sequence")] 
        [SerializeField] private GameObject endReaction;

        private List<SimonTask> fullSequence = new List<SimonTask>();
        private int currentRound = 1; // Tracks which round we are on (e.g., Round 3 means doing 3 steps)
        private int playerStepIndex = 0; // Tracks which step the player is currently executing
        private bool isScreenPlaying = false; // Prevents interaction while the screen is showing the pattern
        
        private SimonInteractable _currentStandingToolInteractable;
        private GameObject _heldToolObject;
        private Vector3 _heldToolOriginalPosition;
        private Transform _heldToolOriginalParent;
        private Vector3 _playerStartPosition;
        private bool isEnd;
        
        protected override void Start()
        {
            base.Start();
            _playerStartPosition = transform.position;
            GenerateSequence();
            StartCoroutine(PlaySequenceOnScreen());
        }

        private void GenerateSequence()
        {
            fullSequence.Clear();
            // for (int i = 0; i < totalActionsToWin; i++)
            // {
            //     SimonTask newTask = new SimonTask
            //     {
            //         RequiredTool = (ToolType)Random.Range(1, 5),
            //         TargetArea = (AreaType)Random.Range(1, 5)
            //     };
            //     fullSequence.Add(newTask);
            // }
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

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed || isScreenPlaying) return;
            if(isEnd) SceneLoader.Instance.ActivatePreloadedScene();

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
                    _heldToolOriginalPosition = _heldToolObject.transform.position;
                    _heldToolOriginalParent = _heldToolObject.transform.parent;

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
                _heldToolObject.transform.position = _heldToolOriginalPosition;
                _heldToolObject.transform.rotation = Quaternion.identity;
                _heldToolObject = null;
            }
            currentHeldTool = ToolType.None;
        }

        private void ValidatePlayerAction(ToolType usedTool, AreaType appliedArea)
        {
            SimonTask expectedTask = fullSequence[playerStepIndex];
            
            if (usedTool == expectedTask.RequiredTool && appliedArea == expectedTask.TargetArea)
            {
                // -- SUCCESS --
                Debug.Log("Correct move!");
                StartCoroutine(HandleSuccessRoutine());
            }
            else
            {
                // -- FAILURE --
                Debug.Log("WRONG MOVE! Restarting this round.");
                StartCoroutine(HandleFailureRoutine());
            }
        }

        private IEnumerator HandleSuccessRoutine()
        {
            isScreenPlaying = true;
            if (_heldToolObject != null)
            {
                _heldToolObject.transform.DOPunchRotation(new Vector3(0, 0, toolVibrateStrength), toolVibrateDuration, toolVibrateVibrato, 1f);
                yield return new WaitForSeconds(toolVibrateDuration);
            }
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
                    playerStepIndex = 0;
                    transform.position = _playerStartPosition;
                    StartCoroutine(PlaySequenceOnScreen());
                }
            }
            else
            {
                isScreenPlaying = false;
                transform.position = _playerStartPosition;
            }
        }

        private IEnumerator EndSequence()
        {
            yield return new WaitForSeconds(0.5f);
            endReaction.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            isEnd = true;
        }

        private IEnumerator HandleFailureRoutine()
        {
            isScreenPlaying = true;
            
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
            transform.position = _playerStartPosition;
            StartCoroutine(PlaySequenceOnScreen()); 
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            SimonInteractable interactable = other.GetComponent<SimonInteractable>();
            if (interactable != null)
            {
                if (interactable.isToolStation)
                {
                    currentStandingToolStation = interactable.toolType;
                    _currentStandingToolInteractable = interactable;
                }
                if (interactable.isArea) currentStandingArea = interactable.areaType;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            SimonInteractable interactable = other.GetComponent<SimonInteractable>();
            if (interactable != null)
            {
                // Only clear the references if we are exiting the specific tool we are standing at
                if (interactable.isToolStation && currentStandingToolStation == interactable.toolType)
                {
                    currentStandingToolStation = ToolType.None;
                    if (_currentStandingToolInteractable == interactable) 
                        _currentStandingToolInteractable = null;
                }
                if (interactable.isArea && currentStandingArea == interactable.areaType) 
                    currentStandingArea = AreaType.None;
            }
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

        // --- Screen Display Logic ---
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
                
                yield return new WaitForSeconds(showTaskDuration);
                
                if (display != null) display.SetActive(false);
                
                yield return new WaitForSeconds(waitBetweenTasksDuration); 
            }

            Debug.Log("Player's turn!");
            isScreenPlaying = false;
        }
    }
}