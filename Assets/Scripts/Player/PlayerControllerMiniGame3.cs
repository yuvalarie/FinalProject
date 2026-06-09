using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [System.Serializable]
    public struct SimonTask
    {
        public ToolType RequiredTool;
        public AreaType TargetArea;
    }

    public class PlayerControllerMiniGame3 : PlayerControllerBase
    {
        [Header("Simon Game Settings")]
        [SerializeField, Tooltip("How many steps in the final sequence to win?")] 
        private int totalActionsToWin = 5;

        [Header("Current Status (Read Only)")]
        [SerializeField] private ToolType currentHeldTool = ToolType.None;
        [SerializeField] private AreaType currentStandingArea = AreaType.None;
        [SerializeField] private ToolType currentStandingToolStation = ToolType.None;

        private List<SimonTask> fullSequence = new List<SimonTask>();
        private int currentRound = 1; // Tracks which round we are on (e.g., Round 3 means doing 3 steps)
        private int playerStepIndex = 0; // Tracks which step the player is currently executing
        private bool isScreenPlaying = false; // Prevents interaction while the screen is showing the pattern

        protected override void Start()
        {
            base.Start();
            GenerateSequence();
            StartCoroutine(PlaySequenceOnScreen());
        }

        private void GenerateSequence()
        {
            fullSequence.Clear();
            for (int i = 0; i < totalActionsToWin; i++)
            {
                SimonTask newTask = new SimonTask
                {
                    // Random.Range with integers is exclusive on the max value. 
                    // Since we have 4 tools/areas, we range from 1 to 5 to get 1, 2, 3, or 4.
                    RequiredTool = (ToolType)Random.Range(1, 5),
                    TargetArea = (AreaType)Random.Range(1, 5)
                };
                fullSequence.Add(newTask);
            }
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            // Only fire on the button press, and ignore if the screen is currently animating
            if (!context.performed || isScreenPlaying) return;

            // 1. If standing near a tool, pick it up
            if (currentStandingToolStation != ToolType.None)
            {
                currentHeldTool = currentStandingToolStation;
                Debug.Log($"Picked up: {currentHeldTool}");
                // You can add player visual feedback here (e.g., showing the tool in their hand)
                return;
            }

            // 2. If standing near an area AND holding a tool, apply it
            if (currentStandingArea != AreaType.None && currentHeldTool != ToolType.None)
            {
                ValidatePlayerAction(currentHeldTool, currentStandingArea);
            }
        }

        private void ValidatePlayerAction(ToolType usedTool, AreaType appliedArea)
        {
            SimonTask expectedTask = fullSequence[playerStepIndex];

            if (usedTool == expectedTask.RequiredTool && appliedArea == expectedTask.TargetArea)
            {
                // -- SUCCESS --
                Debug.Log("Correct move!");
                playerStepIndex++;

                // Did they finish all the steps required for this round?
                if (playerStepIndex >= currentRound)
                {
                    if (currentRound >= totalActionsToWin)
                    {
                        Debug.Log("MINIGAME WON!");
                        if (endTriggerObject != null) endTriggerObject.SetActive(true);
                        // Trigger your win animations/scene transition here!
                    }
                    else
                    {
                        // Advance to the next round!
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

        // --- Trigger Detection ---
        private void OnTriggerStay2D(Collider2D other)
        {
            //base.OnTriggerEnter2D(other); // Keeps your End trigger logic intact

            SimonInteractable interactable = other.GetComponent<SimonInteractable>();
            //Debug.Log($"Found this interactable {interactable}");
            if (interactable != null)
            {
                if (interactable.isToolStation) currentStandingToolStation = interactable.toolType;
                if (interactable.isArea) currentStandingArea = interactable.areaType;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            SimonInteractable interactable = other.GetComponent<SimonInteractable>();
            if (interactable != null)
            {
                if (interactable.isToolStation) currentStandingToolStation = ToolType.None;
                if (interactable.isArea) currentStandingArea = AreaType.None;
            }
        }

        // --- Screen Display Logic ---
        private IEnumerator PlaySequenceOnScreen()
        {
            isScreenPlaying = true;
            Debug.Log($"--- Displaying Sequence for Round {currentRound} ---");
            
            // Wait a moment so the player can breathe before the screen starts flashing
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < currentRound; i++)
            {
                SimonTask taskToDisplay = fullSequence[i];
                Debug.Log($"SCREEN SHOWS: Tool {taskToDisplay.RequiredTool} at Area {taskToDisplay.TargetArea}");
                
                // TODO: TRIGGER YOUR UI SCREEN CHANGES HERE 
                // e.g., Update an Image component to show the tool sprite, and highlight an area on the mini-screen
                
                yield return new WaitForSeconds(1.5f); // How long the image stays on screen
                
                // TODO: Clear the screen for a brief moment between pictures
                yield return new WaitForSeconds(0.5f); 
            }

            Debug.Log("Player's turn!");
            isScreenPlaying = false; // Unlocks controls
        }
    }
}