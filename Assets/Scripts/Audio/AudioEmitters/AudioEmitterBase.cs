using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Audio.AudioEmitters
{
    public abstract class AudioEmitterBase : MonoBehaviour
    {
        protected string AudioEventName;
        protected EventInstance AudioInstance;
        private bool _isAudioInstanceInitialized;

        protected abstract void SetAudioEventName();

        protected virtual void OnEnable()
        {
            SetAudioEventName();
        }

        public virtual void PlayAudioOnce()
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
            if(!_isAudioInstanceInitialized)
            {
                InitializeAudioInstance();
            }
            AudioInstance.start();
        }
        
        public virtual void StopAudio(STOP_MODE stopMode = STOP_MODE.IMMEDIATE)
        {
            if(_isAudioInstanceInitialized)
            {
                AudioInstance.stop(stopMode);
            }
        }

        public virtual void ResumeAudio()
        {
            if(!_isAudioInstanceInitialized)
            {
                StartAudio();
                return;
            }
            AudioInstance.setPaused(false);
        }
        
        public virtual void PauseAudio()
        {
            if(_isAudioInstanceInitialized)
            {
                AudioInstance.setPaused(true);
            }
        }
        
        public virtual void  InitializeAudioInstance()
        {
            var eventReference = FMODEvents.Instance.GetEventReferenceByName(AudioEventName);
            AudioInstance = AudioManager.Instance.CreateInstance(eventReference);
            AudioInstance.set3DAttributes(transform.position.To3DAttributes());
            _isAudioInstanceInitialized = true;
        }
        
        private void OnDestroy()
        {
            if(_isAudioInstanceInitialized)
            {
                AudioInstance.release();
                _isAudioInstanceInitialized = false;
            }
        }
    }
}
