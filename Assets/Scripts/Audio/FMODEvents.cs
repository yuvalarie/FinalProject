using FMODUnity;
using Managers;
using UnityEngine;

namespace Audio
{
    public class FMODEvents : PersistentMonoSingleton<FMODEvents>
    {
        [Header("Ambiance")]
        public EventReference page1OfficeAmbiance;
        public EventReference windAmbiance;
        
        [Header("SFX")]
        public EventReference lettersInTubesSFX;
        public EventReference phoneRingSFX;
    }
}
