using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField, Tooltip("Movement speed of the player.")]
        private float speed = 5f;
    
        [SerializeField, Tooltip("Transform where the held object will sit.")]
        private Transform holdSlot;

        [SerializeField, Tooltip("The layer used for grabbable objects.")]
        private LayerMask grabbableLayer;

        private GameObject _heldItem;
        private Vector2 _moveInput;
    
        private InputSystem_Actions _inputActions;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Game.Enable();
            _rb = GetComponent<Rigidbody2D>();
        }
        
        private void OnEnable()
        {
            _inputActions.Game.MoveRight.performed += ctx => _moveInput.x = 1f;
            _inputActions.Game.MoveRight.canceled += ctx => _moveInput.x = 0f;
            _inputActions.Game.MoveLeft.performed += ctx => _moveInput.x = -1f;
            _inputActions.Game.MoveLeft.canceled += ctx => _moveInput.x = 0f;
            _inputActions.Game.MoveUp.performed += ctx => _moveInput.y = 1f;
            _inputActions.Game.MoveUp.canceled += ctx => _moveInput.y = 0f;
            _inputActions.Game.MoveDown.performed += ctx => _moveInput.y = -1f;
            _inputActions.Game.MoveDown.canceled += ctx => _moveInput.y = 0f;
            _inputActions.Game.PickUp.performed += OnPickUp;
        }
        
        private void OnDisable()
        {
            _inputActions.Game.Disable();
        }

        void FixedUpdate()
        {
            HandleMovement(); 
        }
        
        private void OnPickUp(InputAction.CallbackContext context)
        {
            if (_heldItem == null) TryPickUp();
            else DropItem();
        }

        private void HandleMovement()
        {
            Vector3 targetVelocity = new Vector2(_moveInput.x * speed, _moveInput.y * speed);
            _rb.linearVelocity = targetVelocity;
        }

        private void TryPickUp()
        {
            // Using SphereCast or Raycast on a specific layer
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 2f, grabbableLayer))
            {
                _heldItem = hit.collider.gameObject;
                Rigidbody rb = _heldItem.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                _heldItem.transform.position = holdSlot.position;
                _heldItem.transform.SetParent(holdSlot);
            }
        }

        private void DropItem()
        {
            Rigidbody rb = _heldItem.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;

            _heldItem.transform.SetParent(null);
            _heldItem = null;
        }
    }
}