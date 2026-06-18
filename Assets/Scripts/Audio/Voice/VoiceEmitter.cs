using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Audio.Voice
{
    [Serializable]
    public struct FmodParameter
    {
        public string name;
        public float value;
    }

    [Serializable]
    public struct VoiceProfile
    {
        public string eventName;
        public FmodParameter[] parameters;

        public VoiceProfile(string eventName, FmodParameter[] parameters)
        {
            this.eventName = eventName;
            this.parameters = parameters;
        }
    }

    public class VoiceEmitter : MonoBehaviour
    {
        [Header("Voice Configuration")]
        [SerializeField] private CharacterType characterType;
        [SerializeField, Tooltip("How many characters to treat as read per second")]
        private float charsPerSecond = 15f;

        private const float AUDIO_LENGTH = 0.5f;

        private EventInstance _audioInstance;
        private bool _isInstanceInitialized;
        private Coroutine _speakCoroutine;

        public void Speak(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            if (_speakCoroutine != null)
            {
                StopCoroutine(_speakCoroutine);
                StopAudio();
            }

            var profile = VoiceProfileFactory.Get(characterType);
            float duration = CalculateDuration(text);
            _speakCoroutine = StartCoroutine(SpeakRoutine(profile, duration));
        }

        public void StopSpeaking()
        {
            if (_speakCoroutine != null)
            {
                StopCoroutine(_speakCoroutine);
                _speakCoroutine = null;
            }
            StopAudio();
        }

        private float CalculateDuration(string text)
        {
            int playCount = Mathf.CeilToInt(text.Length / (charsPerSecond * AUDIO_LENGTH));
            return playCount * AUDIO_LENGTH;
        }

        private IEnumerator SpeakRoutine(VoiceProfile profile, float duration)
        {
            StartAudio(profile);
            yield return new WaitForSeconds(duration);
            StopAudio();
            _speakCoroutine = null;
        }

        private void StartAudio(VoiceProfile profile)
        {
            var eventReference = FMODEvents.Instance.GetEventReferenceByName(profile.eventName);
            var parameters = new (string, float)[profile.parameters?.Length ?? 0];
            if (profile.parameters != null)
            {
                for (int i = 0; i < profile.parameters.Length; i++)
                    parameters[i] = (profile.parameters[i].name, profile.parameters[i].value);
            }
            _audioInstance = AudioManager.Instance.CreateInstance(eventReference, parameters);
            _audioInstance.start();
            _isInstanceInitialized = true;
        }

        private void StopAudio()
        {
            if (_isInstanceInitialized)
            {
                _audioInstance.stop(STOP_MODE.IMMEDIATE);
                _audioInstance.release();
                _isInstanceInitialized = false;
            }
        }

        private void OnDestroy()
        {
            StopSpeaking();
        }
    }
}
