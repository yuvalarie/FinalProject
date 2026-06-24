using Objects.Poster;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Pages
{
    public class PlayerControllerPage13 : PlayerControllerBase
    {
        [Header("Poster Settings")]
        //[SerializeField] PosterManager
        
        private bool atPosterCollider1;
        private bool atPosterCollider2;
        private bool atPosterCollider3;
        
        protected override void OnInteraction(InputAction.CallbackContext context)
        {
            
        }
    }
}