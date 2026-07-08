using System.Reflection;
using UnityEngine;

namespace Audio.AudioEmitters
{
    public class HellAnimationSfx : MonoBehaviour
    {
        [Header("SFX Configuration")]
        [SerializeField, AudioEventName] private string closeCircleEvent;
        [SerializeField, AudioEventName] private string sparkleEvent;
        [SerializeField, AudioEventName] private string popEvent;

        [Header("Ambiance")]
        [SerializeField] private AmbianceEmitter hellAmbianceEmitter;

        public void PlayCloseCircle() => PlayOnce(closeCircleEvent);
        public void PlaySparkle() => PlayOnce(sparkleEvent);
        public void PlayPop() => PlayOnce(popEvent);

        public void StartAmbiance() => hellAmbianceEmitter.StartAudio();
        public void StopAmbiance() => hellAmbianceEmitter.StopAudio();

        private void PlayOnce(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }
            var field = typeof(AudioEventNames).GetField(eventName, BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                Debug.LogError($"{gameObject.name}: no constant named '{eventName}' found in AudioEventNames.");
                return;
            }
            var resolvedEventName = (string)field.GetValue(null);
            var eventReference = FMODEvents.Instance.GetEventReferenceByName(resolvedEventName);
            AudioManager.Instance.PlayOneShot(eventReference, transform.position);
        }
    }
}
