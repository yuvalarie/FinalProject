using Audio;
using FMOD.Studio;
using FMODUnity;
using Managers;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Objects
{
    public class LetterInTube : MonoBehaviour
    {
        private EventInstance _letterSoundInstance;

        private void Start()
        {
            var reference = FMODEvents.Instance.lettersInTubesSFX;
            _letterSoundInstance = AudioManager.Instance.CreateInstance(reference);
            RuntimeManager.AttachInstanceToGameObject(_letterSoundInstance, gameObject, true);
        }
        
        public void StartAudio()
        {
            var pitchValue = Random.Range(-1f, 1f);
            var volumeValue = Random.Range(-1f, 1f);
            _letterSoundInstance.setParameterByName("LetterTubePitch", pitchValue);
            _letterSoundInstance.setParameterByName("LetterTubeVolume", volumeValue);
            _letterSoundInstance.start();
        }

        public void StopAudio()
        {
            _letterSoundInstance.stop(STOP_MODE.IMMEDIATE); 
            _letterSoundInstance.setTimelinePosition(0);
        }
    }
}
