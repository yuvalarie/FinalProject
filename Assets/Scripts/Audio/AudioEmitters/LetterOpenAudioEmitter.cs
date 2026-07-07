using System.Reflection;
using UnityEngine;

namespace Audio.AudioEmitters
{
    public class LetterOpenAudioEmitter : AudioEmitterBase
    {
        [Header("SFX Configuration")]
        [SerializeField, AudioEventName] private string eventName;
        [SerializeField] private bool playOnEnable = false;
        [SerializeField] private bool skipFirstPlay = false;
        
        private bool _triedToPlay = false;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            if (playOnEnable)
            {
                PlayAudioWrapper();
            }
        }


        private void PlayAudioWrapper()
        {
            if (skipFirstPlay && !_triedToPlay)
            {
                _triedToPlay = true;
                return;
            }
            PlayAudioOnce();
        }
        public override void SetAudioEventName()
        {
            var field = typeof(AudioEventNames).GetField(eventName, BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                Debug.LogError($"{gameObject.name}: no constant named '{eventName}' found in AudioEventNames.");
                return;
            }
            AudioEventName = (string)field.GetValue(null);
        }
    }
}
