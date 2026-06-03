using Player;
using UnityEngine;

namespace Objects
{
    public class HandleAnimationObjects : MonoBehaviour
    {
        [SerializeField] private PlayerControllerPage4 playerScript;

        public void TriggerPillowTwist()
        {
            playerScript.PlayFrame2SecondAnimation();
        }
        
        public void TriggerHandPat()
        {
            playerScript.PlayFrame3SecondAnimation();
        }
    
        public void TriggerBlanketPull()
        {
            playerScript.PlayFrame4SecondAnimation();
        }
    }
}