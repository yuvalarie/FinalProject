using System.Reflection;
using UnityEngine;

namespace Audio.AudioEmitters
{
    public class AmbianceEmitter : AudioEmitterBase
    {
        [Header("Ambiance Configuration")]
        [SerializeField, AudioEventName] private string eventName;
        [SerializeField] private bool playOnEnable = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (playOnEnable)
            {
                StartAudio();
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
