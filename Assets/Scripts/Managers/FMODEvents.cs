using FMODUnity;
using UnityEngine;

namespace Managers
{
    public class FMODEvents : MonoSingleton<FMODEvents>
    {
        [Header("Ambiance")]
        public EventReference page1OfficeAmbiance;
        public EventReference lettersInTubesAmbiance;
        public EventReference windAmbiance;
    }
}
