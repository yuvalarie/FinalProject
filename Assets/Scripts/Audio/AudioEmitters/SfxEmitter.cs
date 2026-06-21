using System.Reflection;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Audio.AudioEmitters
{
    public class SfxEmitter : AudioEmitterBase
    {
        [Header("SFX Configuration")]
        [SerializeField, AudioEventName] private string eventName;
        [SerializeField] private bool playOnEnable = false;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            if (playOnEnable)
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

        public override void PlayAudioOnce() => base.PlayAudioOnce();
        public override void StartAudio() => base.StartAudio();
        public override void StopAudio(STOP_MODE stopMode = STOP_MODE.IMMEDIATE) => base.StopAudio(stopMode);
        public override void ResumeAudio() => base.ResumeAudio();
        public override void PauseAudio() => base.PauseAudio();
        public override void InitializeAudioInstance() => base.InitializeAudioInstance();
    }
}
