using UnityEngine;

namespace Audio
{
    public class OfficeAmbianceEmitter : AmbianceEmitter
    {
        private void Start()
        {
            Emitter = AudioManager.Instance.InitializeEventEmitter(FMODEvents.Instance.page1OfficeAmbiance, gameObject);
            Emitter.Play();
        }
    }
}
