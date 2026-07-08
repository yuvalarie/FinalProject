using System.Reflection;
using UnityEngine;

namespace Audio.AudioEmitters
{
    public class ReversedSfxEmitter : AudioEmitterBase
    {
        [Header("SFX Configuration")]
        [SerializeField, AudioEventName] private string eventName;
        [SerializeField] private bool playOnDisable = false;

        private void OnDisable()
        {
            if (playOnDisable)
            {
                PlayAudioOnce();
            }
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
