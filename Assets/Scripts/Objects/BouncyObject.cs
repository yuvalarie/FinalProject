using Player;
using UnityEngine;

public class BouncyObject : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField, Tooltip("How hard the player is pushed horizontally.")] 
    private float knockbackForce = 10f;
    
    [SerializeField, Tooltip("How much upward lift is added to the bounce.")] 
    private float upwardLift = 5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Angel"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        
            // --- ADD THIS ---
            var playerScript = collision.gameObject.GetComponent<PlayerControllerBase>(); // Or your specific player class name
            if (playerScript != null) playerScript.StartKnockback(0.2f); 
            // ----------------

            if (playerRb != null)
            {
                Vector2 bounceDirection = (collision.transform.position - transform.position).normalized;
                bounceDirection.y += upwardLift;
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(bounceDirection.normalized * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}