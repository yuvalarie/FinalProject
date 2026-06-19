using System.Reflection;
using UnityEngine;

namespace Audio.AudioEmitters
{
    public class SfxEmitter : AudioEmitterBase
    {
        [Header("SFX Configuration")]
        [SerializeField, AudioEventName] private string eventName;

        protected override void SetAudioEventName()
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
