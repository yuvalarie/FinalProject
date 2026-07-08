using System.Reflection;
using UnityEngine;

namespace Audio.AudioEmitters
{
    public class AmbianceEmitter : AudioEmitterBase
    {
        [Header("Ambiance Configuration")]
        [SerializeField, AudioEventName] private string eventName;
        [SerializeField] private bool playOnStart = false;
        [SerializeField] private bool startOnEnable = false;
        [SerializeField] private bool stopOnEnable = false;
        [SerializeField] private bool startOnDisable = false;
        [SerializeField] private bool stopOnDisable = false;

        private void Start()
        {
            if (playOnStart)
            {
                StartAudio();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (startOnEnable)
            {
                StartAudio();
            }
            if (stopOnEnable)
            {
                StopAudio();
            }
        }

        private void OnDisable()
        {
            if (startOnDisable)
            {
                StartAudio();
            }
            if (stopOnDisable)
            {
                StopAudio();
            }
        }

        public void PlayAmbiance(params (string paramName, float value)[] parameters)
        {
            StartAudio(parameters);
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
