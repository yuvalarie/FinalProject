using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [Serializable] 
    public class TuningSettings 
    {
        [Tooltip("Current value of the dial (e.g., starting water temp).")]
        public float currentValue = 20f;
        
        [Tooltip("The perfect target value the player needs to reach.")]
        public float targetValue = 75f;
        
        [Tooltip("The safe zone around the target. +/- this number wins.")]
        public float tolerance = 5f;

        [Tooltip("How fast holding Left/Right changes the value.")]
        public float turnSpeed = 25f;
    }

    
    public class PlayerControllerMiniGame3Old : PlayerControllerBase
    {
        [Header("Tuning Settings")]
        [SerializeField] TuningSettings zone1Settings;
        [SerializeField] TuningSettings zone2Settings;

        private bool _isMiniGameComplete = false;
        private int _currentZone = 1;
        private float currentValue;
        private float targetValue;
        private float tolerance;
        private float turnSpeed;
        
        protected override void Awake()
        {
            base.Awake();
            LoadZoneSettings(1);
        }

        private void LoadZoneSettings(int i)
        {
            if (i == 1)
            {
                currentValue = zone1Settings.currentValue;
                targetValue = zone1Settings.targetValue;
                tolerance = zone1Settings.tolerance;
                turnSpeed = zone1Settings.turnSpeed;
            }
            else
            {
                currentValue = zone2Settings.currentValue;
                targetValue = zone2Settings.targetValue;
                tolerance = zone2Settings.tolerance;
                turnSpeed = zone2Settings.turnSpeed;
            }
        }
        
        protected override void HandleMovement()
        {
            if (_isMiniGameComplete) base.HandleMovement();
            float adjustment = MoveInput.x * turnSpeed;
            currentValue += adjustment * Time.deltaTime;
            currentValue = Mathf.Clamp(currentValue, 0f, 100f);
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed || _isMiniGameComplete) return;

            Debug.Log("Player pressed Interact to lock in the choice...");
            CheckWinCondition();
        }

        private void CheckWinCondition()
        {
            if (Mathf.Abs(currentValue - targetValue) <= tolerance)
            {
                _isMiniGameComplete = true;
                Debug.Log("SUCCESS: Locked in the perfect temperature! Mini-game complete.");
            }
            else
            {
                Debug.Log($"FAILED: Locked in at {currentValue:F1}, but needed to be near {targetValue}. Keep tuning!");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Interaction"))
            {
                _isMiniGameComplete = false;
                currentValue = 20f; // Reset to starting value
                Rb.linearVelocity = Vector2.zero; // Stop any movement
                other.gameObject.SetActive(false);
                _currentZone++;
                LoadZoneSettings(_currentZone);
            }
        }
    }
}