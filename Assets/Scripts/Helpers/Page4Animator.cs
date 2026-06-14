using Player;
using UnityEngine;

namespace Objects
{
    public class Page4Animator : MonoBehaviour
    {
        [SerializeField] private PlayerControllerPage4 player;

        public void TextBubble2()
        {
            player.SetActiveTextBubble2();
        }
        
        public void TextBubble3()
        {
            player.SetActiveTextBubble3();
        }
        
        public void TextBubble4()
        {
            player.SetActiveTextBubble4();
        }
        
        public void TextBubble5()
        {
            player.SetActiveTextBubble5();
        }
    }
}