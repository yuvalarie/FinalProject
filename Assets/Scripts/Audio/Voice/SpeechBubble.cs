using UnityEngine;

namespace Audio.Voice
{
    [RequireComponent(typeof(VoiceEmitter))]
    public class SpeechBubble : MonoBehaviour
    {
        [Header("Speech Bubble")]
        [SerializeField] private string text;
        [SerializeField] private bool playOnEnable = true;

        private VoiceEmitter _voiceEmitter;

        private void Awake()
        {
            _voiceEmitter = GetComponent<VoiceEmitter>();
        }

        private void OnEnable()
        {
            if (playOnEnable)
                Speak();
        }

        public void Speak()
        {
            _voiceEmitter.Speak(text);
        }
    }
}
