using System;
using UnityEngine;

namespace Audio
{
    public abstract class AudioCallerBase : MonoBehaviour
    {
        protected string AudioEventName;
        
        protected abstract void SetAudioEventName();

        protected void OnEnable()
        {
            SetAudioEventName();
        }

        public virtual void PlayAudio()
        {
            if (string.IsNullOrEmpty(AudioEventName))
            {
                Debug.LogError($"Audio event name is not set for {gameObject.name}");
                return;
            }
            var eventReference = FMODEvents.Instance.GetEventReferenceByName(AudioEventName);
            AudioManager.Instance.PlayOneShot(eventReference, transform.position);
        }
    }
}
