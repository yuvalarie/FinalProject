using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.MiniGames
{
    public class PlayerControllerMiniGame5 : PlayerControllerBase
    {
        [Header("Catching Settings")]
        [SerializeField, Tooltip("Drag the CatchZone child object's BoxCollider2D here.")] 
        private Collider2D catchZoneCollider;
        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.performed || catchZoneCollider == null) return;

            // Ask the Physics engine: "What objects are currently overlapping my CatchZone bounds?"
            Collider2D[] overlappedObjects = Physics2D.OverlapBoxAll(
                catchZoneCollider.bounds.center, 
                catchZoneCollider.bounds.size, 
                0f
            );

            // Loop through everything we found inside the box
            foreach (Collider2D col in overlappedObjects)
            {
                // Check if the object we found has the FallingNote script
                FallingNote note = col.GetComponent<FallingNote>();
                
                if (note != null)
                {
                    // Manually trigger the catch!
                    note.CatchByPlayer(catchZoneCollider.transform);
                }
            }
        }

    }
}