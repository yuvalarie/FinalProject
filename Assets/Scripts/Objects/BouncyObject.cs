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
        
            var playerScript = collision.gameObject.GetComponent<PlayerControllerBase>(); 
            if (playerScript != null) playerScript.StartKnockback(0.2f); 

            if (playerRb != null)
            {
                // 1. Get the exact pixel where they collided
                Vector2 contactPoint = collision.GetContact(0).point;
                
                // 2. Measure from the contact point to the player, NOT from the center of the bouncy object
                Vector2 bounceDirection = ((Vector2)collision.transform.position - contactPoint).normalized;
                
                // 3. Add the extra lift
                bounceDirection.y += upwardLift;
                
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(bounceDirection.normalized * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}