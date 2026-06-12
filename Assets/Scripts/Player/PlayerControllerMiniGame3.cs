using System.Collections;
using System.Collections.Generic;
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
    public struct ToolSpriteMapping
    {
        public ToolType Tool;
        public Sprite Sprite;
    }

    [System.Serializable]
    public struct AreaSpriteMapping
    {
        public AreaType Area;
        public Sprite Sprite;
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
        [SerializeField, Tooltip("The UI Image component showing the Tool")] 
        private GameObject screenToolImage;
        [SerializeField, Tooltip("The UI Image component showing the Area highlight")] 
        private GameObject screenAreaImage;
        [SerializeField, Tooltip("How long should the next task be shown")] private float showTaskDuration;
        
        [SerializeField, Tooltip("Map each tool to its sprite here")] 
        private List<ToolSpriteMapping> toolSpritesList;
        [SerializeField, Tooltip("Map each area to its sprite here")] 
        private List<AreaSpriteMapping> areaSpritesList;

        [Header("Current Status (Read Only)")]
        [SerializeField] private ToolType currentHeldTool = ToolType.None;
        [SerializeField] private AreaType currentStandingArea = AreaType.None;
        [SerializeField] private ToolType currentStandingToolStation = ToolType.None;

        private List<SimonTask> fullSequence = new List<SimonTask>();
        private int currentRound = 1; // Tracks which round we are on (e.g., Round 3 means doing 3 steps)
        private int playerStepIndex = 0; // Tracks which step the player is currently executing
        private bool isScreenPlaying = false; // Prevents interaction while the screen is showing the pattern

        private SpriteRenderer _screenToolImageSpriteRenderer;
        private SpriteRenderer _screenAreaImageSpriteRenderer;
        
        private SimonInteractable _currentStandingToolInteractable;
        private GameObject _heldToolObject;
        private Vector3 _heldToolOriginalPosition;
        private Transform _heldToolOriginalParent;
        
        protected override void Start()
        {
            base.Start();
            if (screenToolImage != null)
                _screenToolImageSpriteRenderer = screenToolImage.GetComponent<SpriteRenderer>();
            if (screenAreaImage != null)
                _screenAreaImageSpriteRenderer = screenAreaImage.GetComponent<SpriteRenderer>();
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
                _heldToolObject.transform.SetParent(_heldToolOriginalParent);
                _heldToolObject.transform.position = _heldToolOriginalPosition;
                _heldToolObject = null;
            }
            currentHeldTool = ToolType.None;
        }

        private void ValidatePlayerAction(ToolType usedTool, AreaType appliedArea)
        {
            SimonTask expectedTask = fullSequence[playerStepIndex];

            ReturnHeldTool();
            
            if (usedTool == expectedTask.RequiredTool && appliedArea == expectedTask.TargetArea)
            {
                // -- SUCCESS --
                Debug.Log("Correct move!");
                playerStepIndex++;

                if (playerStepIndex >= currentRound)
                {
                    if (currentRound >= totalActionsToWin)
                    {
                        Debug.Log("MINIGAME WON!");
                        //if (endTriggerObject != null) endTriggerObject.SetActive(true);
                    }
                    else
                    {
                        currentRound++;
                        playerStepIndex = 0;
                        currentHeldTool = ToolType.None; // Reset tool for next round
                        StartCoroutine(PlaySequenceOnScreen());
                    }
                }
            }
            else
            {
                // -- FAILURE --
                Debug.Log("WRONG MOVE! Restarting this round.");
                
                // TODO: ADD VISUAL/AUDIO FAILURE EFFECT HERE (e.g., buzzer sound, screen flashing red)
                
                playerStepIndex = 0; // Reset their progress for this round
                currentHeldTool = ToolType.None; // Drop their tool
                StartCoroutine(PlaySequenceOnScreen()); // Replay the same sequence to them
            }
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
        
        private Sprite GetSpriteForTool(ToolType tool)
        {
            foreach (var mapping in toolSpritesList)
            {
                if (mapping.Tool == tool) return mapping.Sprite;
            }
            return null; // Return nothing if it's missing
        }
        
        private Sprite GetSpriteForArea(AreaType area)
        {
            foreach (var mapping in areaSpritesList)
            {
                if (mapping.Area == area) return mapping.Sprite;
            }
            return null; // Return nothing if it's missing
        }

        // --- Screen Display Logic ---
        private IEnumerator PlaySequenceOnScreen()
        {
            isScreenPlaying = true;
            Debug.Log($"--- Displaying Sequence for Round {currentRound} ---");
            
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < currentRound; i++)
            {
                SimonTask taskToDisplay = fullSequence[i];
                Debug.Log($"SCREEN SHOWS: Tool {taskToDisplay.RequiredTool} at Area {taskToDisplay.TargetArea}");

                if (_screenToolImageSpriteRenderer != null && _screenAreaImageSpriteRenderer != null)
                {
                    _screenToolImageSpriteRenderer.sprite = GetSpriteForTool(taskToDisplay.RequiredTool);
                    _screenAreaImageSpriteRenderer.sprite = GetSpriteForArea(taskToDisplay.TargetArea);
                    
                    screenToolImage.gameObject.SetActive(true);
                    screenAreaImage.gameObject.SetActive(true);
                }
                
                yield return new WaitForSeconds(showTaskDuration);
                
                if (screenToolImage != null) screenToolImage.gameObject.SetActive(false);
                if (screenAreaImage != null) screenAreaImage.gameObject.SetActive(false);
                
                yield return new WaitForSeconds(0.5f); 
            }

            Debug.Log("Player's turn!");
            isScreenPlaying = false;
        }
    }
}