using System;
using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public abstract class PlayerControllerBase : MonoBehaviour
    {
        [SerializeField, Tooltip("Drag the PlayerArtSettings ScriptableObject here.")] protected PlayerArtController artSettings;
        
        [SerializeField, Tooltip("Movement speed of the player.")] protected float speed = 5f;
        
        [SerializeField, Tooltip("next scene name")] protected string nextSceneName;

        [SerializeField, Tooltip("initial size")] protected SizeSettings initialSize = SizeSettings.None;
        
        private InputSystem_Actions _inputActions;
        protected Rigidbody2D Rb;
        protected Vector2 MoveInput;

        protected Animator Animator;
        protected SpriteRenderer SpriteRenderer;
        protected SizeSettings currentSize;
        
        protected virtual void Awake()
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Game.Enable();
            
            Rb = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        protected virtual void Start()
        {
            currentSize = initialSize;
            SetSize();
            StartCoroutine(DelayedSceneLoading());
        }

        private IEnumerator DelayedSceneLoading()
        {
            yield return null;
            SceneLoader.Instance?.PreloadScene(nextSceneName);
        }

        protected virtual void OnEnable()
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

        protected void SetSize()
        {
            switch (currentSize)
            {
                case SizeSettings.Small:
                    SetSmall();
                    break;
                case SizeSettings.Medium:
                    SetMedium();
                    break;
                case SizeSettings.Large:
                    SetLarge();
                    break;
                case SizeSettings.ExtraLarge:
                    SetExtraLarge();
                    break;
                case SizeSettings.None:
                    return;
            }
        }

        protected void SetSmall()
        {
            if (Animator != null) Animator.runtimeAnimatorController = artSettings.smallAnimatorController;
            SpriteRenderer.sprite = artSettings.smallSprite;
            transform.localScale = new Vector3(artSettings.smallSize, artSettings.smallSize);
        }
        
        protected void SetMedium()
        {
            if (Animator != null) Animator.runtimeAnimatorController = artSettings.mediumLargeAnimatorController;
            SpriteRenderer.sprite = artSettings.mediumLargeSprite;
            transform.localScale = new Vector3(artSettings.mediumSize, artSettings.mediumSize);
        }
        
        protected void SetLarge()
        {
            if (Animator != null) Animator.runtimeAnimatorController = artSettings.mediumLargeAnimatorController;
            SpriteRenderer.sprite = artSettings.mediumLargeSprite;
            transform.localScale = new Vector3(artSettings.largeSize, artSettings.largeSize);
        }
        
        protected void SetExtraLarge()
        {
            if (Animator != null) Animator.runtimeAnimatorController = artSettings.extraLargeAnimatorController;
            SpriteRenderer.sprite = artSettings.extraLargeSprite;
            transform.localScale = new Vector3(artSettings.extraLargeSize, artSettings.extraLargeSize);
        }
    }
}