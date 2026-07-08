using System.Reflection;
using UnityEngine;

namespace Audio.AudioEmitters
{
    public class RepeatableSfxEmitter : AudioEmitterBase
    {
        [Header("SFX Configuration")]
        [SerializeField, AudioEventName] private string eventName;
        [SerializeField] private bool playOnEnable = false;
        [SerializeField] private bool muteFirstPlay = false;
        [SerializeField] private bool onlyFirstPlay = false;

        private bool _hasEnabledBefore = false;

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
            var isFirstPlay = !_hasEnabledBefore;
            _hasEnabledBefore = true;

            if (muteFirstPlay && isFirstPlay)
            {
                return;
            }
            if (onlyFirstPlay && !isFirstPlay)
            {
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
