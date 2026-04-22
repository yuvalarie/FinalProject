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
        
        [SerializeField, Tooltip("How far the player can reach to grab an object.")]
        private float grabRadius = 1f;
        
        [SerializeField, Tooltip("The layer used for valid drop zones.")]
        private LayerMask dropZoneLayer;

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
            Debug.Log("Attempting to pick up item...");

            RaycastHit2D hit = Physics2D.CircleCast(transform.position, grabRadius, Vector2.zero, 0f, grabbableLayer);

            if (hit.collider != null)
            {
                Debug.Log($"SUCCESS: Found '{hit.collider.name}' on the Grabbable layer!");
                _heldItem = hit.collider.gameObject;
                
                Rigidbody2D itemRb = _heldItem.GetComponent<Rigidbody2D>();
                if (itemRb != null) 
                {
                    itemRb.bodyType = RigidbodyType2D.Kinematic; // Kinematic for 2D
                }
                else
                {
                    Debug.LogWarning($"Wait, '{hit.collider.name}' doesn't have a Rigidbody2D attached!");
                }

                _heldItem.transform.position = holdSlot.position;
                _heldItem.transform.SetParent(holdSlot);
            }
            else
            {
                Debug.Log("FAILED: Nothing found on the Grabbable layer within the grab radius.");
            }
        }

        private void DropItem()
        {
            Debug.Log("Attempting to drop item...");

            // 1. Check if we are standing in a valid drop zone
            Collider2D dropZone = Physics2D.OverlapCircle(transform.position, grabRadius, dropZoneLayer);

            if (dropZone != null)
            {
                Debug.Log($"SUCCESS: Dropping '{_heldItem.name}' in zone '{dropZone.name}'");

                _heldItem.transform.SetParent(null);
        
                // Optional: If you want it to drop exactly where the player is standing
                // instead of floating in the HoldSlot position, uncomment this:
                // _heldItem.transform.position = transform.position; 

                _heldItem = null;
            }
            else
            {
                Debug.Log("FAILED: Cannot drop here. You must be in a drop zone!");
            }
        }
        private void OnDrawGizmos()
        {
            // Draws a yellow circle around the player in the Scene view to show exactly where they can grab
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, grabRadius);
        }
    }
}