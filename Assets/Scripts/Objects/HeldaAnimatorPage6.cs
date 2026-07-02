using DG.Tweening;
using Audio.AudioEmitters;
using Player;
using UnityEngine;

namespace Objects
{
    public class HeldaAnimatorPage6 : MonoBehaviour
    {
        [SerializeField] private PlayerControllerPage6 player;

        [Header("Movement Targets")]
        [SerializeField] private Transform target1;
        [SerializeField] private Transform target2;
        [SerializeField] private Transform target3;
        [SerializeField] private Transform target4;
        [SerializeField] private Transform target5;

        [Header("Bouncy Walk Settings")]
        [SerializeField, Tooltip("How long the movement takes.")] 
        private float duration = 1.5f;
        [SerializeField, Tooltip("How high she bounces on the Y axis while moving.")] 
        private float jumpPower = 0.2f;
        [SerializeField, Tooltip("How many 'hops' she takes to reach the target.")] 
        private int numberOfJumps = 3;
        
        [Header("Legs Animation Settings")]
        [SerializeField] private Transform legsTargetPosition2;
        [SerializeField] private Transform legsTargetPosition3;
        [SerializeField] private GameObject legsObject;

        [Header("Footstep SFX")]
        [SerializeField, Tooltip("Plays once per hop, timed to land when she hits the ground.")]
        private AudioEmitterBase footstepSfx;

        private void Start()
        {
            PlayMovement1();
        }

        private void PlayFootstepSfx()
        {
            float interval = duration / numberOfJumps;
            for (int i = 1; i <= numberOfJumps; i++)
            {
                DOVirtual.DelayedCall(interval * i, () => footstepSfx?.PlayAudioOnce());
            }
        }

        public void PlayMovement1()
        {
            PlayFootstepSfx();
            transform.DOJump(target1.position, jumpPower, numberOfJumps, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => player.OnAnimation1Complete());
        }

        public void PlayMovement2()
        {
            PlayFootstepSfx();
            Sequence sequence2 = DOTween.Sequence();
            sequence2.Append(transform.DOJump(target2.position, jumpPower, numberOfJumps, duration)
                .SetEase(Ease.Linear));
            sequence2.Join(legsObject.transform.DOJump(legsTargetPosition2.position, jumpPower, numberOfJumps, duration)
                .SetEase(Ease.Linear));
            sequence2.OnComplete(() => player.StartTextBubble1Sequence());
        }

        public void PlayMovement3()
        {
            PlayFootstepSfx();
            Sequence sequence3 = DOTween.Sequence();
            sequence3.Append(transform.DOJump(target3.position, jumpPower, numberOfJumps, duration)
                .SetEase(Ease.Linear));
            sequence3.Join(legsObject.transform.DOJump(legsTargetPosition3.position, jumpPower, numberOfJumps, duration)
                .SetEase(Ease.Linear));
            sequence3.OnComplete(() => 
            {
                //player.ChangeCanMoveState();
                player.StartAnimation4();
            });
        }

        public void PlayMovement4()
        {
            PlayFootstepSfx();
            transform.DOJump(target4.position, jumpPower, numberOfJumps, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => player.ShowTextBubble5());
        }

        public void PlayMovement5()
        {
            PlayFootstepSfx();
            transform.DOJump(target5.position, jumpPower, numberOfJumps, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => player.OnAnimation5Complete());
        }
    }
}