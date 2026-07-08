using System;
using FMOD.Studio;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Audio.Voice
{
    public class RecordedSpeechBubble : MonoBehaviour
    {
        [Header("Recorded Speech Bubble")]
        [SerializeField] private CharacterType character;
        [SerializeField] private int lineNumber;
        [SerializeField] private bool playOnEnable = true;
        [SerializeField] private bool canBeTalkedOver = true;
        [SerializeField] private bool stopOnDisable = true;

        private EventInstance _instance;
        private bool _isInstanceInitialized;

        private void OnEnable()
        {
            if (playOnEnable)
                Speak();
        }

        private void OnDisable()
        {
            if (stopOnDisable)
                StopSelf();
        }

        private void OnDestroy()
        {
            StopSelf();
        }

        public void Speak()
        {
            if (!VoiceSystem.Enabled) return;
            StopSelf();
            _instance = VoiceManager.Instance.PlayLine(character, lineNumber, canBeTalkedOver ? null : (Action)StopSelf);
            _isInstanceInitialized = true;
        }

        private void StopSelf()
        {
            if (_isInstanceInitialized)
            {
                _instance.stop(STOP_MODE.IMMEDIATE);
                _isInstanceInitialized = false;
            }
        }
    }
}
