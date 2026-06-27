using UnityEngine;

namespace Audio.AudioEmitters
{
    public class PhoneAudioController : MonoBehaviour
    {
        [Header("Audio Emitters")]
        [SerializeField] private SfxEmitter ringEmitter;
        [SerializeField] private SfxEmitter pickupEmitter;

        public void RingStart() => ringEmitter.ResumeAudio();
        public void RingStop() => ringEmitter.PauseAudio();
        public void Pickup() => pickupEmitter.PlayAudioOnce();
    }
}
