using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public abstract class PlayerControllerBase : MonoBehaviour
    {
        [SerializeField, Tooltip("Movement speed of the player.")] protected float speed = 5f;
        
        [SerializeField, Tooltip("next scene name")] protected string nextSceneName;
        
        [SerializeField] protected GameObject endTriggerObject;

        protected Vector2 MoveInput;

        private InputSystem_Actions _inputActions;
        protected Rigidbody2D Rb;
        
        protected virtual void Awake()
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Game.Enable();
            
            Rb = GetComponent<Rigidbody2D>();
        }
        
        protected virtual void Start()
        {
            SceneLoader.Instance?.PreloadScene(nextSceneName);
        }

        private void OnEnable()
        {
            _inputActions.Game.MoveRight.performed += ctx => MoveInput.x = 1f;
            _inputActions.Game.MoveRight.canceled += ctx => MoveInput.x = 0f;
            _inputActions.Game.MoveLeft.performed += ctx => MoveInput.x = -1f;
            _inputActions.Game.MoveLeft.canceled += ctx => MoveInput.x = 0f;
            _inputActions.Game.MoveUp.performed += ctx => MoveInput.y = 1f;
            _inputActions.Game.MoveUp.canceled += ctx => MoveInput.y = 0f;
            _inputActions.Game.MoveDown.performed += ctx => MoveInput.y = -1f;
            _inputActions.Game.MoveDown.canceled += ctx => MoveInput.y = 0f;
            
            _inputActions.Game.Interact.performed += OnInteraction;
            _inputActions.Game.Interact.canceled += OnInteraction;
        }
        
        private void OnDisable()
        {
            _inputActions.Game.Disable();
        }

        protected virtual void FixedUpdate()
        {
            HandleMovement(); 
        }
        
        protected virtual void HandleMovement()
        {
            Vector3 targetVelocity = new Vector2(MoveInput.x * speed, MoveInput.y * speed);
            Rb.linearVelocity = targetVelocity;
        }

        protected abstract void OnInteraction(InputAction.CallbackContext context);

        private void OnDestroy()
        {
            _inputActions?.Dispose();
        }
        
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("End"))
            {
                SceneLoader.Instance?.ActivatePreloadedScene();
            }
        }
    }
}