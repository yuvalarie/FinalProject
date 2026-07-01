using System;
using Audio.AudioEmitters;
using FMOD.Studio;
using Npc;
using UnityEngine;

namespace Objects
{
    public class HellPortal : MonoBehaviour
    {
        [Header("Art References")] 
        [SerializeField] private Animator animator;
        private static readonly int PlayTrigger = Animator.StringToHash("Play");
        [SerializeField] private AudioEmitterBase audioEmitter;

        // Counts how many friends are currently flying toward the portal.
        // Portal stays open until all in-flight friends have arrived (shared-pointer pattern).
        private int _inFlightCount;

        public void SuckIn(FriendController friend)
        {
            gameObject.SetActive(true);
            // animator.SetTrigger(PlayTrigger);
            _inFlightCount++;
            if (_inFlightCount == 1)
            {
                // Only play the sound when the first friend is sucked in, to avoid overlapping sounds.
                audioEmitter.StartAudio();
                // audioEmitter.StartAudio(("Indicator", 0f)); // 0 == start, which will fade out the music
            }
            friend.GetThrown(transform.position, OnFriendArrived);
        }

        // Called when each individual friend reaches the portal.
        // Only hides once the last in-flight friend lands.
        private void OnFriendArrived()
        {
            _inFlightCount--;
            if (_inFlightCount <= 0)
            {
                _inFlightCount = 0; // guard against going negative from unexpected calls
                Hide();
            }
        }

        private void Hide()
        {
            // This method can be called after the animation finishes to hide the portal.
            // You can use an animation event at the end of the animation to call this method.
            // audioEmitter.SetParameter("Indicator", 1f); // 1 == end, which will fade in the music
            audioEmitter.StopAudio(STOP_MODE.ALLOWFADEOUT);
            gameObject.SetActive(false);
            // we might want to set a dispersing animation here instead of just hiding it, but for now this is fine.
        }
    }
}
