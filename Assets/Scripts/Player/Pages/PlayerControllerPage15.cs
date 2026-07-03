using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Pages
{
    public class PlayerControllerPage15 : PlayerControllerBase
    {
        [Header("Sequence Settings")] 
        [SerializeField] private GameObject screen0;
        [SerializeField] private GameObject screen1;
        [SerializeField] private GameObject screen2;
        [SerializeField] private GameObject screen3;
        [SerializeField] private GameObject screen4;
        
        [Header("Trigger Settings")]
        [SerializeField] private Collider2D startCollider;
        
        private bool isSequenceActive = false;
        private bool hasSequenceStarted = false;
        private bool isAnimating = false;
        private int interactionCount;

        protected override void Start()
        {
            base.Start();
            SceneLoader.Instance.PreloadScene(nextSceneName);
        }
        
        protected override void OnInteraction(InputAction.CallbackContext context)
        {
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
            if (!context.performed || !isSequenceActive || isAnimating || interactionCount >= 4) return;

            interactionCount++;
            TransitionToState(interactionCount, true);
        }

        protected override void OnTextBackward(InputAction.CallbackContext context)
        {
            if (!context.performed || !isSequenceActive || isAnimating || interactionCount <= 0) return;

            if(interactionCount > 1) interactionCount--;
            TransitionToState(interactionCount, false);
        }
        
        private void TransitionToState(int targetState, bool isMovingForward)
        {
            screen0.SetActive(false);
            screen1.SetActive(false);
            screen2.SetActive(false);
            screen3.SetActive(false);
            screen4.SetActive(false);
            isAnimating = true;

            switch (targetState)
            {
                case 1:
                    screen1.SetActive(true);
                    break;
                case 2:
                    screen2.SetActive(true);
                    break;
                case 3:
                    screen3.SetActive(true);
                    break;
                case 4:
                    screen4.SetActive(true);
                    isSequenceActive = false;
                    EndTextInputMode();
                    break;
            }

            isAnimating = false;
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (other == startCollider && !hasSequenceStarted)
            {
                hasSequenceStarted = true;
                isSequenceActive = true;
                interactionCount = 1;
                BeginTextInputMode();
                TransitionToState(interactionCount, true);
            }
        }
    }
}
