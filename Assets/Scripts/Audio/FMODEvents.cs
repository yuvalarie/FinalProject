using FMODUnity;
using Managers;
using UnityEngine;

namespace Audio
{
    public class FMODEvents : MonoSingleton<FMODEvents>
    {
        [Header("Ambiance")]
        public EventReference page1OfficeAmbiance;
        public EventReference lettersInTubesAmbiance;
        public EventReference windAmbiance;
    }
}
