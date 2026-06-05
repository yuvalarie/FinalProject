using System;
using Npc;
using UnityEngine;

namespace Objects
{
    public class HellPortal : MonoBehaviour
    {
        [Header("Art References")] 
        [SerializeField] private Animator animator;
        private static readonly int PlayTrigger = Animator.StringToHash("Play");

        public void SuckIn(FriendController friend)
        {
            gameObject.SetActive(true);
            // animator.SetTrigger(PlayTrigger);
            friend.GetThrown(transform.position,  Hide);
        }

        private void Hide()
        {
            // This method can be called after the animation finishes to hide the portal.
            // You can use an animation event at the end of the animation to call this method.
            gameObject.SetActive(false);
            // we might want to set a dispersing animation here instead of just hiding it, but for now this is fine.
        }
    }
}
