using System.Collections;
using System.Collections.Generic;
using Objects;
using Objects.Poster;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class Page13FriendsManager : MonoBehaviour
    {
        [Header("Data References")] 
        [SerializeField] private ChosenFriendsData chosenFriendsData;
        [SerializeField] private FriendsDictionary allFriendsDatabase;
        [SerializeField] private FriendsDataPage13[] setFriends;

        [Header("Pharmacy Line")] 
        [Tooltip("How long to wait before the line advances"), SerializeField] private float waitInLine;
        [Tooltip("The standing spots in the line. Element 0 is the front of the line!")]
        [SerializeField] private Transform[] frame2Waypoints; 
        [Tooltip("Where they walk to completely exit Frame 2")]
        [SerializeField] private Transform frame2ExitPoint;

        [Header("Frame 3")] 
        [SerializeField] private List<GameObject> notes;
        [SerializeField] private Transform handStartPosition;
        [SerializeField] private Transform handEndPosition;
        [SerializeField] private float frame3duration;

        [Header("Pharmacy Line (Frame 4)")]
        [Tooltip("Where they spawn outside Frame 4 before walking in")]
        [SerializeField] private Transform frame4SpawnPoint;
        [Tooltip("The standing spots in Frame 4. Element 0 is the front!")]
        [SerializeField] private Transform[] frame4Waypoints;
        [Tooltip("Where they walk to completely exit Frame 4 and be destroyed")]
        [SerializeField] private Transform frame4ExitPoint;

        [Header("Friend Settings")] 
        [SerializeField] private float frame2duration;
        [SerializeField] private float frame4duration;
        
        private readonly Queue<int> _friendsWaitingToSpawn = new Queue<int>();
        private readonly List<FriendSequenceController> _activeLineF2 = new List<FriendSequenceController>();
        private readonly List<FriendSequenceController> _activeLineF4 = new List<FriendSequenceController>();
        private Dictionary<int, FriendsDataPage13> _friendsDictionary;
        
        private void Start()
        {
            _friendsDictionary = allFriendsDatabase.friendsDictionary;

            foreach (var friend in setFriends)
            {
                chosenFriendsData.AddFriend(friend.id);
            }

            foreach (int friendId in chosenFriendsData.Friends)
            {
                if (friendId != 9) _friendsWaitingToSpawn.Enqueue(friendId);
            }
            
            InitialFillRoutine();
        }
        
        private void InitialFillRoutine()
        {
            // for (int i = 0; i < frame4Waypoints.Length; i++)
            // {
            //     if (_friendsWaitingToSpawn.Count == 0) break;
            //     
            //     SpawnFriend(_friendsWaitingToSpawn.Dequeue(), frame4Waypoints[i], true);
            // }
            
            for (int i = 0; i < frame2Waypoints.Length; i++)
            {
                if (_friendsWaitingToSpawn.Count == 0) break;

                SpawnFriend(_friendsWaitingToSpawn.Dequeue(), frame2Waypoints[i], false);
            }
            
            StartCoroutine(SceneSequenceCoroutine());
        }

        private IEnumerator SceneSequenceCoroutine()
        {
            while (_activeLineF2.Count > 0 || _activeLineF4.Count > 0 || _friendsWaitingToSpawn.Count > 0)
            {
                yield return new WaitForSeconds(waitInLine);
                AdvanceLine();
            }
        }
        
        private void AdvanceLine()
        {
            // 1. ADVANCE FRAME 4 LINE
            if (_activeLineF4.Count > 0)
            {
                // FriendSequenceController leavingF4 = _activeLineF4[0];
                // _activeLineF4.RemoveAt(0);
                // leavingF4.MoveToExitFrame4(frame4ExitPoint);
                
                for (int i = 0; i < _activeLineF4.Count; i++)
                {
                    if (i >= frame4Waypoints.Length) break;
                    Transform targetSpot = frame4Waypoints[i];
                    if (Vector2.Distance(_activeLineF4[i].transform.position, targetSpot.position) > 0.5f)
                    {
                        _activeLineF4[i].MoveToSpotInFrame4(targetSpot);
                    }
                }
            }

            // 2. ADVANCE FRAME 2 LINE
            if (_activeLineF2.Count > 0)
            {
                FriendSequenceController leavingF2 = _activeLineF2[0];
                _activeLineF2.RemoveAt(0);
                
                leavingF2.MoveToExitAndContinue(frame2ExitPoint);

                for (int i = 0; i < _activeLineF2.Count; i++)
                {
                    _activeLineF2[i].MoveToSpotInLine(frame2Waypoints[i]);
                }

                if (_friendsWaitingToSpawn.Count > 0 && _activeLineF2.Count < frame2Waypoints.Length)
                {
                    Transform newTargetSpot = frame2Waypoints[_activeLineF2.Count];
                    SpawnFriend(_friendsWaitingToSpawn.Dequeue(), newTargetSpot, false);
                }
            }
        }
        
        private void SpawnFriend(int id, Transform targetLineSpot, bool isForFrame4)
        {
            if (!_friendsDictionary.TryGetValue(id, out FriendsDataPage13 friendData))
            {
                Debug.LogWarning($"No Page13 friend data found for friend id {id}");
                return;
            }

            if (friendData == null) return;

            GameObject container = new GameObject($"Friend_{id}_Sequence");
            
            container.transform.position = targetLineSpot.position;
            GameObject f2Obj = Instantiate(friendData.frame2Object, container.transform);
            GameObject f4Obj = Instantiate(friendData.frame4Object, container.transform);
            GameObject handObj = Instantiate(friendData.hand, container.transform);
            
            GameObject noteObj = null;
            if (notes.Count > 0)
            {
                noteObj = notes[0];
                notes.RemoveAt(0);
            }
            else
            {
                Debug.LogWarning($"Not enough notes! Skipping note assignment for Friend_{id}");
            }
        
            FriendSequenceController controller = container.AddComponent<FriendSequenceController>();
            
            controller.Initialize(this, f2Obj, f4Obj, frame2duration, frame4duration, isForFrame4, handObj, handStartPosition, handEndPosition, noteObj, frame3duration);
            
            if (isForFrame4)
            {
                _activeLineF4.Add(controller);
            }
            else
            {
                _activeLineF2.Add(controller);
            }

        }
        
        public void FriendReadyForFrame4(FriendSequenceController friend)
        {
            int targetIndex = _activeLineF4.Count;
            
            // Safety check: if line is somehow full, just stack them at the back
            if (targetIndex >= frame4Waypoints.Length) targetIndex = frame4Waypoints.Length - 1;
            
            _activeLineF4.Add(friend);
            friend.EnterFrame4Line(frame4SpawnPoint, frame4Waypoints[targetIndex]);
        }
    }
}
