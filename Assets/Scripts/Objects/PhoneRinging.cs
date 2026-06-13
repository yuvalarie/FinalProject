using Audio;
using FMODUnity;
using UnityEngine;

namespace Objects
{
    public class PhoneRinging : MonoBehaviour
    {
        
        private EventReference _phoneRingReference;

        private void Start()
        {
            _phoneRingReference = FMODEvents.Instance.GetEventReferenceByName(AudioEventNames.PhoneRing);
        }
        
        public void PlayRingPhoneAudio()
        {
            AudioManager.Instance.PlayOneShot(_phoneRingReference, transform.position);
        }

    }
}
