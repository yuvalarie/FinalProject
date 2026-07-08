using System.Collections;
using Audio.AudioEmitters;
using DG.Tweening;
using Managers;
using Objects.Poster;
using UnityEngine;

namespace Objects 
{
    public class FriendSequenceController : MonoBehaviour
    {
        private Page13FriendsManager _manager;
        private GameObject _frame2Obj;
        private GameObject _frame4Obj;
        private GameObject _handObj;
        private GameObject _noteObj;
        private AudioEmitterBase _footstepSfx;
        private AudioEmitterBase _cardTakeSfx;

        private Transform _handStart;
        private Transform _handEnd;

        private float _frame2Duration;
        private float _frame4Duration;
        private float _handDuration;
        
        private float _scatterRadius = 0.3f; 
        private float _durationVariance = 0.5f;
        private int _minJumps = 3;
        private int _maxJumps = 6;

        public void Initialize(Page13FriendsManager manager, GameObject f2Obj, GameObject f4Obj, float frame2, float frame4, bool startInFrame4, GameObject hand, Transform handStart, Transform handEnd, GameObject note, float handDuration, AudioEmitterBase footstepSfx, AudioEmitterBase cardTakeSfx)
        {
            _manager = manager;
            _frame2Obj = f2Obj;
            _frame4Obj = f4Obj;
            _handObj = hand;
            _noteObj = note;
            _footstepSfx = footstepSfx;
            _cardTakeSfx = cardTakeSfx;

            _frame2Duration = frame2;
            _frame4Duration = frame4;
            _handDuration = handDuration;

            _handStart = handStart;
            _handEnd = handEnd;

            if (startInFrame4)
            {
                _frame2Obj.SetActive(false);
                _frame4Obj.SetActive(true);
            }
            else
            {
                _frame2Obj.SetActive(true);
                _frame4Obj.SetActive(false);
            }
            _frame2Obj.transform.localPosition = Vector3.zero;
            _frame4Obj.transform.localPosition = Vector3.zero;
        }
        
        private Vector3 GetScatteredPosition(Transform exactTarget)
        {
            Vector2 randomOffset = Random.insideUnitCircle * _scatterRadius;
            return exactTarget.position + new Vector3(randomOffset.x, randomOffset.y/2, 0f);
        }

        private int GetRandomJumps() => Random.Range(_minJumps, _maxJumps + 1);
        
        private float GetRandomDuration(float baseDuration) => baseDuration + Random.Range(-_durationVariance, _durationVariance);

        private void PlayFootstepSfx(int jumps, float duration)
        {
            float interval = duration / jumps;
            for (int i = 1; i <= jumps; i++)
            {
                DOVirtual.DelayedCall(interval * i, () => _footstepSfx?.PlayAudioOnce());
            }
        }

       // --- FRAME 2 LOGIC ---
        public void MoveToSpotInLine(Transform targetSpot)
        {
            transform.DOKill();
            int jumps = GetRandomJumps();
            float duration = GetRandomDuration(_frame2Duration);
            PlayFootstepSfx(jumps, duration);
            transform.DOJump(GetScatteredPosition(targetSpot), jumpPower: 0.1f, numJumps: jumps, duration: duration)
                .SetEase(Ease.Linear);
        }

        public void MoveToExitAndContinue(Transform exitSpot)
        {
            transform.DOKill();
            int jumps = GetRandomJumps();
            float duration = GetRandomDuration(_frame2Duration);
            PlayFootstepSfx(jumps, duration);
            transform.DOJump(GetScatteredPosition(exitSpot), jumpPower: 0.1f, numJumps: jumps, duration: duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => Frame3HandRoutine());
        }

        // --- FRAME 3 LOGIC ---
        private void Frame3HandRoutine()
        {
            _handObj.transform.position = _handStart.position;
            _handObj.transform.DOMove(_handEnd.position, _handDuration)
                .OnComplete(() =>
                {
                    _noteObj.transform.SetParent(_handObj.transform);
                    _cardTakeSfx?.PlayAudioOnce();
                    _handObj.transform.DOMove(_handStart.position, _handDuration)
                        .OnComplete(() => _manager.FriendReadyForFrame4(this));
                });
        }

        // --- FRAME 4 LOGIC ---
        public void EnterFrame4Line(Transform spawnPoint, Transform targetSpot)
        {
            _frame2Obj.SetActive(false);
            _frame4Obj.SetActive(true);

            transform.position = GetScatteredPosition(spawnPoint);
            transform.DOKill();
            int jumps = GetRandomJumps();
            float duration = GetRandomDuration(_frame4Duration);
            PlayFootstepSfx(jumps, duration);
            transform.DOJump(GetScatteredPosition(targetSpot), jumpPower: 0.1f, numJumps: jumps, duration: duration)
                .SetEase(Ease.Linear);
        }

        public void MoveToSpotInFrame4(Transform targetSpot)
        {
            transform.DOKill();
            int jumps = GetRandomJumps();
            float duration = GetRandomDuration(_frame4Duration);
            PlayFootstepSfx(jumps, duration);
            transform.DOJump(GetScatteredPosition(targetSpot), jumpPower: 0.1f, numJumps: jumps, duration: duration)
                .SetEase(Ease.Linear);
        }

        public void MoveToExitFrame4(Transform exitSpot)
        {
            transform.DOKill();
            int jumps = GetRandomJumps();
            float duration = GetRandomDuration(_frame4Duration);
            PlayFootstepSfx(jumps, duration);
            transform.DOJump(GetScatteredPosition(exitSpot), jumpPower: 0.1f, numJumps: jumps, duration: duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}