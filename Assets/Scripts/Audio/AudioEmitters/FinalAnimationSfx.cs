using System;
using System.Reflection;
using UnityEngine;

namespace Audio.AudioEmitters
{
    public class FinalAnimationSfx : MonoBehaviour
    {
        [Header("SFX Configuration")]
        [SerializeField, AudioEventName] private string shakeEvent;
        [SerializeField, AudioEventName] private string walkEvent;
        [SerializeField, AudioEventName] private string showPaperEvent;
        [SerializeField, AudioEventName] private string ahhhEvent;

        public void PlayShake() => PlayOnce(shakeEvent);
        public void PlayWalk() => PlayOnce(walkEvent);
        public void PlayShowPaper() => PlayOnce(showPaperEvent);
        public void PlayAhhh() => PlayOnce(ahhhEvent);

        public void StartFinalAnimation()
        {
            MusicManager.Instance?.StartBreakdown();
        }
        
        public void StopFinalAnimation()
        {
            MusicManager.Instance?.StopBreakdown();
        }
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

        private void OnDisable()
        {
            StopFinalAnimation();
        }
    }
}
