using System;
using FMODUnity;
using UnityEngine;

namespace Audio
{
    // public enum AmbianceType
    // {
    //     None,
    //     Wind,
    //     Office
    // }
    [RequireComponent(typeof(StudioEventEmitter))]
    public abstract class AmbianceEmitterBase : MonoBehaviour
    {
        protected StudioEventEmitter Emitter;

        private void Awake()
        {
            Emitter = GetComponent<StudioEventEmitter>();
        }

        private void OnDestroy()
        {
            if (Emitter)
            {
                Emitter.Stop();
            }
        }
    }
}
