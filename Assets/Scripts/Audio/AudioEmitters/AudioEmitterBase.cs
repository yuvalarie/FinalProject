using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Audio.AudioEmitters
{
    public abstract class AudioEmitterBase : MonoBehaviour
    {
        protected string AudioEventName;
        protected EventInstance AudioInstance;

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

        public virtual void StartAudio()
        {
            var eventReference = FMODEvents.Instance.GetEventReferenceByName(AudioEventName);
            AudioInstance = AudioManager.Instance.CreateInstance(eventReference);
            AudioInstance.set3DAttributes(transform.position.To3DAttributes());
            AudioInstance.start();
        }
        
        public virtual void StopAudio(STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)
        {
            AudioInstance.stop(stopMode);
            AudioInstance.release();
        }
    }
}
