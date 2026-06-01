using Audio;
using FMOD.Studio;
using Managers;
using UnityEngine;

namespace Objects
{
    public class LetterInTube : MonoBehaviour
    {
        private EventInstance _letterSoundInstance;
        public void StartAudio()
        {
            if (_letterSoundInstance.isValid())
            {
                _letterSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                _letterSoundInstance.release();
            }
            var pitchValue = Random.Range(-1f, 1f);
            var volumeValue = Random.Range(-1f, 1f);
            var reference = FMODEvents.Instance.lettersInTubesAmbiance;
            _letterSoundInstance = AudioManager.Instance.CreateInstance(reference, ("LetterTubePitch", pitchValue), ("LetterTubeVolume", volumeValue));
            _letterSoundInstance.start();
        }

        public void StopAudio()
        {
            if (_letterSoundInstance.isValid())
            {
                _letterSoundInstance.release();
            }
        }
    }
}
