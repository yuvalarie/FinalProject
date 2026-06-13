using System.Collections;
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
        [SerializeField] private float minRandomDelay = 0f;
        [SerializeField] private float maxRandomDelay = 0.5f;
        private static readonly int Start1 = Animator.StringToHash("Start");

        // private EventInstance _letterSoundInstance;
        private EventReference _letterSoundReference;
        private Animator _animator;
        

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _letterSoundReference = FMODEvents.Instance.GetEventReferenceByName(AudioEventNames.LettersInTubes);
            //_letterSoundInstance = AudioManager.Instance.CreateInstance(reference);
            //RuntimeManager.AttachInstanceToGameObject(_letterSoundInstance, gameObject, true);
            StartAnimationWithRandomDelay();
        }
        
        public void StartAudio()
        {
            var pitchValue = Random.Range(-1f, 1f);
            var volumeValue = Random.Range(-1f, 1f);
            // voice volume will be 1 if random is greater than 0.5, otherwise 0, to create a more distinct on/off effect for the voice
            var voiceVolumeValue = Random.value > 0.7f ? 1f : 0f;
            // _letterSoundInstance.setParameterByName("LetterTubePitch", pitchValue);
            // _letterSoundInstance.setParameterByName("LetterTubeVolume", volumeValue);
            // _letterSoundInstance.start();
            AudioManager.Instance.PlayOneShot(_letterSoundReference, transform.position, ("LetterTubePitch", pitchValue), ("LetterTubeVolume", volumeValue), ("VoiceVolume", voiceVolumeValue));
        }

        public void StopAudio()
        {
            // does nothing for now
            // _letterSoundInstance.stop(STOP_MODE.IMMEDIATE); 
            // _letterSoundInstance.setTimelinePosition(0);
        }
        
        public void StartAnimationWithRandomDelay()
        {
            var randomDelay = Random.Range(minRandomDelay, maxRandomDelay);
            StartCoroutine(StartAnimationAfterDelay(randomDelay));
        }

        private IEnumerator StartAnimationAfterDelay(float randomDelay)
        {
            yield return new WaitForSeconds(randomDelay);
            _animator.SetTrigger(Start1);
        }
    }
}
