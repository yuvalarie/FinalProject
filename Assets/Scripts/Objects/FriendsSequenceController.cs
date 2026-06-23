using System.Collections;
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
        private FriendsDataPage13 _data;

        private float _frame2Duration;
        private float _frame4Duration;

        public void Initialize(Page13FriendsManager manager, GameObject f2Obj, GameObject f4Obj, float frame2, float frame4, bool startInFrame4)
        {
            _manager = manager;
            _frame2Obj = f2Obj;
            _frame4Obj = f4Obj;
            
            _frame2Duration = frame2;
            _frame4Duration = frame4;

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

        // --- FRAME 2 LOGIC ---
        public void MoveToSpotInLine(Transform targetSpot)
        {
            transform.DOKill();
            transform.DOJump(targetSpot.position, jumpPower: 0.1f, numJumps: 5, duration: _frame2Duration)
                .SetEase(Ease.Linear);
        }

        public void MoveToExitAndContinue(Transform exitSpot)
        {
            transform.DOJump(exitSpot.position, jumpPower: 0.1f, numJumps: 5, duration: _frame2Duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => StartCoroutine(Frame3HandRoutine()));
        }

        // --- FRAME 3 LOGIC ---
        private IEnumerator Frame3HandRoutine()
        {
            // FRAME 3 PLACEHOLDER
            Debug.Log($"Hand interaction starting for {gameObject.name} in Frame 3");
            yield return new WaitForSeconds(2f); // Simulated wait
            
            _manager.FriendReadyForFrame4(this);
        }

        // --- FRAME 2 LOGIC ---
        public void EnterFrame4Line(Transform spawnPoint, Transform targetSpot)
        {
            _frame2Obj.SetActive(false);
            _frame4Obj.SetActive(true);

            transform.position = spawnPoint.position;
            transform.DOKill();
            transform.DOJump(targetSpot.position, jumpPower: 0.1f, numJumps: 5, duration: _frame4Duration)
                .SetEase(Ease.Linear);
        }
        
        public void MoveToSpotInFrame4(Transform targetSpot)
        {
            transform.DOKill();
            transform.DOJump(targetSpot.position, jumpPower: 0.1f, numJumps: 5, duration: _frame4Duration)
                .SetEase(Ease.Linear);
        }

        public void MoveToExitFrame4(Transform exitSpot)
        {
            transform.DOKill();
            transform.DOJump(exitSpot.position, jumpPower: 0.1f, numJumps: 5, duration: _frame4Duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}