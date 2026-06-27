using System.Collections;
using UnityEngine;

namespace Audio.AudioEmitters
{
    public class DelayedSfxEmitter : SfxEmitter
    {
        [Header("Delay")]
        [SerializeField] private float delay = 0f;

        public override void PlayAudioOnce(params (string paramName, float value)[] parameters)
        {
            StartCoroutine(PlayOnceDelayed(parameters));
        }

        public override void StartAudio(params (string paramName, float value)[] parameters)
        {
            StartCoroutine(StartDelayed(parameters));
        }

        private IEnumerator PlayOnceDelayed((string paramName, float value)[] parameters)
        {
            yield return new WaitForSeconds(delay);
            base.PlayAudioOnce(parameters);
        }

        private IEnumerator StartDelayed((string paramName, float value)[] parameters)
        {
            yield return new WaitForSeconds(delay);
            base.StartAudio(parameters);
        }
    }
}
