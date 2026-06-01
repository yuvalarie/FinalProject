using FMODUnity;
using Managers;
using UnityEngine;

namespace Audio
{
    public class FMODEvents : MonoSingleton<FMODEvents>
    {
        [Header("Ambiance")]
        public EventReference page1OfficeAmbiance;
        public EventReference windAmbiance;
        
        [Header("SFX")]
        public EventReference lettersInTubesSFX;
    }
}
