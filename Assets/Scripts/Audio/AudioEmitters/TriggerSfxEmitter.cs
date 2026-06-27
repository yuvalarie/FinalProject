using UnityEngine;

namespace Audio.AudioEmitters
{
    public class TriggerSfxEmitter : SfxEmitter
    {
        [Header("Trigger Filter")]
        [Tooltip("Only objects with this tag will trigger the audio. Leave empty to trigger on any collider.")]
        [SerializeField] private string requiredTag;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (string.IsNullOrEmpty(requiredTag) || other.CompareTag(requiredTag))
                PlayAudioOnce();
        }
    }
}
