using System;
using System.Collections;
using Managers;
using Unity.VisualScripting;
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
        [SerializeField] protected Transform graphicsRoot;

        [SerializeField] private Collider2D smallCollider;
        [SerializeField] private Collider2D mediumLargeCollider;
        [SerializeField] private Collider2D extraLargeCollider;
        
        private InputSystem_Actions _inputActions;
        private bool _isUsingTextInput;
        protected Rigidbody2D Rb;
        protected Vector2 MoveInput;
        protected Vector2 targetVelocity;

        protected Animator Animator;
        protected SpriteRenderer SpriteRenderer;
        protected SizeSettings CurrentSize;
        private Vector2 _currentVisualOffset;
        
        public bool IsStunned = false;
        
        protected virtual void Awake()
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Game.Enable();
            
            Rb = GetComponent<Rigidbody2D>();
            if (graphicsRoot == null) return;
            Animator = graphicsRoot.GetComponent<Animator>();
            SpriteRenderer = graphicsRoot.GetComponent<SpriteRenderer>();
        }
        
        protected virtual void Start()
        {
            CurrentSize = initialSize;
            SetSize();
        }
        
        protected virtual void OnEnable()
        {
            _inputActions.Game.MoveRight.performed += OnMoveRightPerformed;
            _inputActions.Game.MoveRight.canceled += OnMoveRightCanceled;
            _inputActions.Game.MoveLeft.performed += OnMoveLeftPerformed;
            _inputActions.Game.MoveLeft.canceled += OnMoveLeftCanceled;
            _inputActions.Game.MoveUp.performed += OnMoveUpPerformed;
            _inputActions.Game.MoveUp.canceled += OnMoveUpCanceled;
            _inputActions.Game.MoveDown.performed += OnMoveDownPerformed;
            _inputActions.Game.MoveDown.canceled += OnMoveDownCanceled;
            
            _inputActions.Game.Interact.performed += OnInteraction;
            _inputActions.Game.Interact.canceled += OnInteraction;

            _inputActions.Text.Forward.performed += OnTextForward;
            _inputActions.Text.Backward.performed += OnTextBackward;

            if (_isUsingTextInput)
            {
                _inputActions.Text.Enable();
                _inputActions.Game.Disable();
            }
            else
            {
                _inputActions.Game.Enable();
                _inputActions.Text.Disable();
            }
        }
        
        private void OnDisable()
        {
            _inputActions.Game.MoveRight.performed -= OnMoveRightPerformed;
            _inputActions.Game.MoveRight.canceled -= OnMoveRightCanceled;
            _inputActions.Game.MoveLeft.performed -= OnMoveLeftPerformed;
            _inputActions.Game.MoveLeft.canceled -= OnMoveLeftCanceled;
            _inputActions.Game.MoveUp.performed -= OnMoveUpPerformed;
            _inputActions.Game.MoveUp.canceled -= OnMoveUpCanceled;
            _inputActions.Game.MoveDown.performed -= OnMoveDownPerformed;
            _inputActions.Game.MoveDown.canceled -= OnMoveDownCanceled;
            
            _inputActions.Game.Interact.performed -= OnInteraction;
            _inputActions.Game.Interact.canceled -= OnInteraction;

            _inputActions.Text.Forward.performed -= OnTextForward;
            _inputActions.Text.Backward.performed -= OnTextBackward;

            _inputActions.Game.Disable();
            _inputActions.Text.Disable();
        }

        protected virtual void FixedUpdate()
        {
            HandleMovement(); 
        }
        
        protected virtual void HandleMovement()
        {
            if(IsStunned) return;
            targetVelocity = new Vector2(MoveInput.x * speed, MoveInput.y * speed);
            Rb.linearVelocity = targetVelocity;

            if (SpriteRenderer != null)
            {
                if (MoveInput.x < 0)
                {
                    SpriteRenderer.flipX = true;
                    if(graphicsRoot != null && _currentVisualOffset != Vector2.zero) graphicsRoot.localPosition = new Vector3(-_currentVisualOffset.x, _currentVisualOffset.y, 0f);
                }
                else if (MoveInput.x > 0)
                {
                    SpriteRenderer.flipX = false;
                    if(graphicsRoot != null && _currentVisualOffset != Vector2.zero) graphicsRoot.localPosition = new Vector3(_currentVisualOffset.x, _currentVisualOffset.y, 0f);
                }
            }
        }

        protected abstract void OnInteraction(InputAction.CallbackContext context);

        protected virtual void OnTextForward(InputAction.CallbackContext context)
        {
        }

        protected virtual void OnTextBackward(InputAction.CallbackContext context)
        {
        }

        protected void BeginTextInputMode()
        {
            _isUsingTextInput = true;
            StopMovementInput();
            _inputActions.Game.Disable();
            _inputActions.Text.Enable();
        }

        protected void EndTextInputMode()
        {
            _isUsingTextInput = false;
            StopMovementInput();
            _inputActions.Text.Disable();
            _inputActions.Game.Enable();
        }

        private void StopMovementInput()
        {
            MoveInput = Vector2.zero;
            targetVelocity = Vector2.zero;
            if (Rb != null) Rb.linearVelocity = Vector2.zero;
        }

        private void OnMoveRightPerformed(InputAction.CallbackContext context) => MoveInput.x = 1f;
        private void OnMoveRightCanceled(InputAction.CallbackContext context) => MoveInput.x = 0f;
        private void OnMoveLeftPerformed(InputAction.CallbackContext context) => MoveInput.x = -1f;
        private void OnMoveLeftCanceled(InputAction.CallbackContext context) => MoveInput.x = 0f;
        private void OnMoveUpPerformed(InputAction.CallbackContext context) => MoveInput.y = 1f;
        private void OnMoveUpCanceled(InputAction.CallbackContext context) => MoveInput.y = 0f;
        private void OnMoveDownPerformed(InputAction.CallbackContext context) => MoveInput.y = -1f;
        private void OnMoveDownCanceled(InputAction.CallbackContext context) => MoveInput.y = 0f;

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

            if (other.CompareTag("Middle"))
            {
                SceneLoader.Instance?.PreloadScene(nextSceneName);
            }
        }
        
        public void StartKnockback(float duration) 
        {
            StartCoroutine(KnockbackRoutine(duration));
        }

        private IEnumerator KnockbackRoutine(float duration)
        {
            IsStunned = true;
            yield return new WaitForSeconds(duration);
            IsStunned = false;
        }

        protected void SetSize()
        {
            if (artSettings == null) return;
            switch (CurrentSize)
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
        
        private void ApplyVisualOffset(Vector2 offset)
        {
            _currentVisualOffset = offset;
            float xSign = SpriteRenderer.flipX ? -1f : 1f;
            graphicsRoot.localPosition = new Vector3(_currentVisualOffset.x * xSign, _currentVisualOffset.y, 0f);
        }

        protected void SetSmall()
        {
            if (Animator != null) Animator.runtimeAnimatorController = artSettings.smallAnimatorController;
            SpriteRenderer.sprite = artSettings.smallSprite;
            transform.localScale = new Vector3(artSettings.smallSize, artSettings.smallSize);
            ApplyVisualOffset(artSettings.smallOffset);
            if(smallCollider != null) smallCollider.enabled = true;
            if(mediumLargeCollider != null) mediumLargeCollider.enabled = false;
            if(extraLargeCollider != null) extraLargeCollider.enabled = false;
        }
        
        protected void SetMedium()
        {
            if (Animator != null) Animator.runtimeAnimatorController = artSettings.mediumLargeAnimatorController;
            SpriteRenderer.sprite = artSettings.mediumLargeSprite;
            transform.localScale = new Vector3(artSettings.mediumSize, artSettings.mediumSize);
            ApplyVisualOffset(artSettings.mediumLargeOffset);
            if(smallCollider != null) smallCollider.enabled = false;
            if(mediumLargeCollider != null) mediumLargeCollider.enabled = true;
            if(extraLargeCollider != null) extraLargeCollider.enabled = false;
        }
        
        protected void SetLarge()
        {
            if (Animator != null) Animator.runtimeAnimatorController = artSettings.mediumLargeAnimatorController;
            SpriteRenderer.sprite = artSettings.mediumLargeSprite;
            transform.localScale = new Vector3(artSettings.largeSize, artSettings.largeSize);
            ApplyVisualOffset(artSettings.mediumLargeOffset);
        }
        
        protected void SetExtraLarge()
        {
            if (Animator != null) Animator.runtimeAnimatorController = artSettings.extraLargeAnimatorController;
            SpriteRenderer.sprite = artSettings.extraLargeSprite;
            transform.localScale = new Vector3(artSettings.extraLargeSize, artSettings.extraLargeSize);
            ApplyVisualOffset(artSettings.extraLargeOffset);
            if(smallCollider != null) smallCollider.enabled = false;
            if(mediumLargeCollider != null) mediumLargeCollider.enabled = false;
            if(extraLargeCollider != null) extraLargeCollider.enabled = true;
        }
    }
}
