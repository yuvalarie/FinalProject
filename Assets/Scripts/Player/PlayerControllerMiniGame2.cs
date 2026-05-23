using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControllerMiniGame2 : PlayerControllerBase
    {
        [Header("Slap Settings")]
        [SerializeField, Tooltip("How big the slap hit-box is.")] 
        private float pushRadius = 1f;
        
        [SerializeField, Tooltip("How hard the slap pushes the NPC.")] 
        private float pushForce = 15f;
        
        [SerializeField, Tooltip("The layer the NPCs are on.")] 
        private LayerMask npcLayer;

        private Vector2 _lastFacingDirection = Vector2.right;

        private void Update()
        {
            // Remember which way we are looking so we can slap in that direction
            // even if we stop moving to press the button.
            if (MoveInput.magnitude > 0.1f)
            {
                _lastFacingDirection = MoveInput.normalized;
            }
        }

        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            // if (IsTrans) 
            // {
            //     Debug.Log("Cannot slap while transparent!");
            //     return;
            // }

            // Calculate the center of the slap slightly in front of the player
            Vector2 slapCenter = (Vector2)transform.position + (_lastFacingDirection * 0.75f);

            // Find all colliders on the NPC layer within the slap radius
            Collider2D[] hits = Physics2D.OverlapCircleAll(slapCenter, pushRadius, npcLayer);

            foreach (Collider2D hit in hits)
            {
                // Try to find the WalkingNpc script on the thing we hit
                var npc = hit.GetComponent<Npc.ClimbingNpcController>();
                if (npc != null)
                {
                    Debug.Log($"Slapped {hit.name}!");
                    npc.TakeSlap(_lastFacingDirection, pushForce);
                }
            }
        }

        private void OnDrawGizmos()
        {
            // Draws a red circle in the editor to help you adjust the slap range
            Gizmos.color = Color.red;
            
            Vector2 direction = Application.isPlaying ? _lastFacingDirection : Vector2.right;
            Vector2 slapCenter = (Vector2)transform.position + (direction * 0.75f);
            
            Gizmos.DrawWireSphere(slapCenter, pushRadius);
        }
    }
}