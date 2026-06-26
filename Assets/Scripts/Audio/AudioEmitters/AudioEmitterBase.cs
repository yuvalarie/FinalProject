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
        private bool _isAudioInstanceInitialized;

        public abstract void SetAudioEventName();

        protected virtual void OnEnable()
        {
            SetAudioEventName();
        }

        public void PlayAudioOnce() => PlayAudioOnce(System.Array.Empty<(string, float)>());

        public void PlayAudioOnce(params (string paramName, float value)[] parameters)
        {
            if (string.IsNullOrEmpty(AudioEventName))
            {
                // Debug.LogError($"Audio event name is not set for {gameObject.name}");
                return;
            }
            var eventReference = FMODEvents.Instance.GetEventReferenceByName(AudioEventName);
            AudioManager.Instance.PlayOneShot(eventReference, transform.position, parameters);
        }

        public void StartAudio() => StartAudio(System.Array.Empty<(string, float)>());

        public void StartAudio(params (string paramName, float value)[] parameters)
        {
            if(!_isAudioInstanceInitialized)
            {
                InitializeAudioInstance();
            }
            foreach (var (paramName, value) in parameters)
            {
                AudioInstance.setParameterByName(paramName, value);
            }
            AudioInstance.start();
        }
        
        public void StopAudio(STOP_MODE stopMode = STOP_MODE.IMMEDIATE)
        {
            if(_isAudioInstanceInitialized)
            {
                AudioInstance.stop(stopMode);
            }
        }

        public void ResumeAudio()
        {
            if(!_isAudioInstanceInitialized)
            {
                StartAudio();
                return;
            }
            AudioInstance.setPaused(false);
        }
        
        public void PauseAudio()
        {
            if(_isAudioInstanceInitialized)
            {
                AudioInstance.setPaused(true);
            }
        }
        
        public void SetParameter(string paramName, float value)
        {
            if (_isAudioInstanceInitialized)
            {
                AudioInstance.setParameterByName(paramName, value);
            }
        }

        public void InitializeAudioInstance()
        {
            var eventReference = FMODEvents.Instance.GetEventReferenceByName(AudioEventName);
            AudioInstance = AudioManager.Instance.CreateInstance(eventReference);
            // AudioInstance.set3DAttributes(transform.position.To3DAttributes());
            _isAudioInstanceInitialized = true;
        }
        
        private void OnDestroy()
        {
            if(_isAudioInstanceInitialized)
            {
                AudioInstance.stop(STOP_MODE.IMMEDIATE);
                AudioInstance.release();
                _isAudioInstanceInitialized = false;
            }
        }
    }
}
