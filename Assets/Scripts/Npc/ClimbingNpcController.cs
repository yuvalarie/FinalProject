using System.Collections;
using UnityEngine;

namespace Npc
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ClimbingNpcController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField, Tooltip("How fast the NPC walks.")] 
        private float walkSpeed = 2f;
        
        [SerializeField, Tooltip("Which direction they constantly walk in.")] 
        private Vector2 walkDirection = Vector2.right;

        [Header("Balance System")]
        [SerializeField, Tooltip("How much balance the NPC has before falling.")] 
        private float maxBalance = 100f;
        
        [SerializeField, Tooltip("How fast they regain balance over time.")] 
        private float balanceRecoveryRate = 20f;
        
        [SerializeField, Tooltip("How long they stay on the floor before getting up.")] 
        private float timeToGetUp = 3f;

        private Rigidbody2D _rb;
        private float _currentBalance;
        
        public bool IsFallen { get; private set; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _currentBalance = maxBalance;
        }

        private void FixedUpdate()
        {
            if (IsFallen) return;

            if (_currentBalance < maxBalance)
            {
                _currentBalance += balanceRecoveryRate * Time.fixedDeltaTime;
            }
            
            float targetX = walkDirection.normalized.x * walkSpeed;
            _rb.linearVelocity = new Vector2(
                Mathf.Lerp(_rb.linearVelocity.x, targetX, Time.fixedDeltaTime * 5f), 
                _rb.linearVelocity.y
            );
        }

        public void TakeSlap(Vector2 pushDirection, float pushForce)
        {
            if (IsFallen) return;

            _rb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);

            _currentBalance -= (pushForce * 3f); 
            Debug.Log($"{gameObject.name} balance is now: {_currentBalance}");

            if (_currentBalance <= 0)
            {
                StartCoroutine(FallOverRoutine());
            }
        }

        private IEnumerator FallOverRoutine()
        {
            Debug.Log($"{gameObject.name} lost their balance and fell over!");
            IsFallen = true;
            _currentBalance = 0f;

            // --- PLACEHOLDER VISUAL ---
            // Rotate the NPC backwards to look "fallen". 
            // When you get animations, delete this and write: _animator.SetTrigger("Fall");
            float fallAngle = walkDirection.x > 0 ? 90f : -90f;
            transform.rotation = Quaternion.Euler(0, 0, fallAngle);

            yield return new WaitForSeconds(timeToGetUp);

            Debug.Log($"{gameObject.name} is getting back up.");
            
            // --- PLACEHOLDER VISUAL ---
            // Stand back up
            // When you get animations, write: _animator.SetTrigger("GetUp");
            transform.rotation = Quaternion.identity;
            
            _currentBalance = maxBalance;
            IsFallen = false;
        }
    }
}