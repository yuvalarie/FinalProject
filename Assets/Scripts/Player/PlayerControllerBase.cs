using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public abstract class PlayerControllerBase : MonoBehaviour
    {
        [SerializeField, Tooltip("Movement speed of the player.")]
        protected float speed = 5f;

        protected Vector2 MoveInput;
    
        protected InputSystem_Actions InputActions;
        protected Rigidbody2D Rb;
        protected SpriteRenderer Renderer;
        protected Color RegColor;
        protected Color TransColor;

        //public bool IsTrans { get; private set; }

        private void Awake()
        {
            InputActions = new InputSystem_Actions();
            InputActions.Game.Enable();
            
            Rb = GetComponent<Rigidbody2D>();
            
            //SetTransparency();
        }

        private void SetTransparency()
        {
            Renderer = GetComponent<SpriteRenderer>();
            RegColor = Renderer.color;
            TransColor.g = RegColor.g;
            TransColor.r = RegColor.r;
            TransColor.b = RegColor.b;
            TransColor.a = 0.5f;
            Renderer.color = TransColor;
            //IsTrans = true;
        }

        private void OnEnable()
        {
            InputActions.Game.MoveRight.performed += ctx => MoveInput.x = 1f;
            InputActions.Game.MoveRight.canceled += ctx => MoveInput.x = 0f;
            InputActions.Game.MoveLeft.performed += ctx => MoveInput.x = -1f;
            InputActions.Game.MoveLeft.canceled += ctx => MoveInput.x = 0f;
            InputActions.Game.MoveUp.performed += ctx => MoveInput.y = 1f;
            InputActions.Game.MoveUp.canceled += ctx => MoveInput.y = 0f;
            InputActions.Game.MoveDown.performed += ctx => MoveInput.y = -1f;
            InputActions.Game.MoveDown.canceled += ctx => MoveInput.y = 0f;
            
            InputActions.Game.Interact.performed += OnInteraction;
            InputActions.Game.Interact.canceled += OnInteraction;
            
            //InputActions.Game.Trans.performed += ctx => { IsTrans = false; Renderer.color = RegColor;
            //};
            //InputActions.Game.Trans.canceled += ctx => { IsTrans = true; Renderer.color = TransColor;
            //};
        }
        
        private void OnDisable()
        {
            InputActions.Game.Disable();
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
    }
}