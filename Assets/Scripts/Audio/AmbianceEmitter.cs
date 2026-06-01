using FMODUnity;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(StudioEventEmitter))]
    public abstract class AmbianceEmitter : MonoBehaviour
    {
        private StudioEventEmitter _emitter;

        private void Awake()
        {
            _emitter = GetComponent<StudioEventEmitter>();
        }
    }
}
