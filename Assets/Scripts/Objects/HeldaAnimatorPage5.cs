using Player;
using UnityEngine;

namespace Objects
{
    public class HeldaAnimatorPage5 : MonoBehaviour
    {
        [SerializeField] private PlayerControllerPage5 player;

        public void OnAnimation1End()
        {
            player.OnAnimation1Complete();
        }
        
        public void OnAnimation2End()
        {
            player.StartTextBubble1Sequence();
        }
        
        public void OnAnimation3End()
        {
            player.ChangeCanMoveState();
            player.StartAnimation4();
        }
        
        public void OnAnimation4End()
        {
            player.ShowTextBubble5();
        }
    }
}