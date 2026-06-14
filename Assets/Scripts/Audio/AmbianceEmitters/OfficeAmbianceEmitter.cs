using UnityEngine;

namespace Audio
{
    public class OfficeAmbianceEmitter : AmbianceEmitter
    {
        private void Start()
        {
            Emitter = AudioManager.Instance.InitializeEventEmitter(FMODEvents.Instance.GetEventReferenceByName(AudioEventNames.OfficeAmbiance), gameObject);
            Emitter.Play();
        }
    }
}
