using System;
using System.Collections.Generic;
using Npc;
using UnityEngine;

namespace Objects
{
    [Serializable]
    public struct AreaBounds
    {
        public MiniGame2FrameArea area;
        public Vector2 bottomLeft;
        public Vector2 topRight;
        public string roamingSortingLayer;
        public int roamingSortingOrder;
        public float bobAmplitude;
        public float bobFrequency;
    }

    public class FriendSpawner : MonoBehaviour
    {
        [Header("Friends")]
        [SerializeField] private List<FriendData> friends;
        [SerializeField] private GameObject friendBasePrefab;

        [Header("Phone Display")]
        // The SpriteRenderer already in the scene on the phone — we just swap its sprite
        [SerializeField] private SpriteRenderer phoneDisplayRenderer;

        [Header("Spawn Positions")]
        // Where discarded friends appear before flying to the hell portal
        [SerializeField] private Transform discardSpawnPosition;

        [Header("Areas")]
        // One entry per MiniGame2FrameArea — defines the world-space bounds of each comic panel
        [SerializeField] private List<AreaBounds> areaBounds;

        [Header("Thrown Layer")]
        // All thrown friends share the same sorting layer regardless of their frame
        [SerializeField] private string thrownSortingLayer;
        [SerializeField] private int thrownSortingOrder;

        [Header("References")]
        [SerializeField] private HellPortal hellPortal;

        private List<FriendData> _orderedFriends;
        private int _currentFriendIndex = 0;

        private void Start()
        {
            // Build the ordered friend list once — order is determined by the weighted bucket algorithm
            BuildOrderedFriendList();
        }

        // Called by MiniGame2SceneController when the hand finishes entering the scene
        public void StartSpawning()
        {
            ShowCurrentFriendOnPhone();
        }

        // Called by MiniGame2HandController on right swipe
        // Spawns the friend into their assigned comic panel and shows the next friend on the phone
        public void SpawnFriend()
        {
            FriendData data = _orderedFriends[_currentFriendIndex];
            AreaBounds bounds = GetBoundsForArea(data.assignedArea);

            // Pick a random spawn position within the assigned frame
            float spawnX = UnityEngine.Random.Range(bounds.bottomLeft.x, bounds.topRight.x);
            float spawnY = UnityEngine.Random.Range(bounds.bottomLeft.y, bounds.topRight.y);

            var instance = Instantiate(friendBasePrefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);
            var controller = instance.GetComponent<FriendController>();
            controller.Setup(data);
            controller.SetLayers(bounds.roamingSortingLayer, bounds.roamingSortingOrder, thrownSortingLayer, thrownSortingOrder);
            controller.SetRoamingSettings(bounds.bobAmplitude, bounds.bobFrequency);

            // Convert the Vector2 bounds into a Unity Bounds for the roaming logic
            Bounds worldBounds = new Bounds(
                new Vector3((bounds.bottomLeft.x + bounds.topRight.x) / 2f,
                            (bounds.bottomLeft.y + bounds.topRight.y) / 2f, 0f),
                new Vector3(bounds.topRight.x - bounds.bottomLeft.x,
                            bounds.topRight.y - bounds.bottomLeft.y, 0f)
            );

            controller.StartRoaming(worldBounds);
            AdvanceToNextFriend();
        }

        // Called by MiniGame2HandController on left swipe
        // Spawns the friend at the discard position and sends them to the hell portal
        public void DiscardFriend()
        {
            FriendData data = _orderedFriends[_currentFriendIndex];
            AreaBounds bounds = GetBoundsForArea(data.assignedArea);

            var instance = Instantiate(friendBasePrefab, discardSpawnPosition.position, Quaternion.identity);
            var controller = instance.GetComponent<FriendController>();
            controller.Setup(data);
            controller.SetLayers(bounds.roamingSortingLayer, bounds.roamingSortingOrder, thrownSortingLayer, thrownSortingOrder);

            hellPortal.SuckIn(controller);
            AdvanceToNextFriend();
        }

        private void AdvanceToNextFriend()
        {
            _currentFriendIndex++;

            if (_currentFriendIndex >= _orderedFriends.Count)
            {
                // All friends have been swiped — this is where the scene-end event will be fired later
                // (return controls to walking controller, continue the story)
                Debug.Log("FriendSpawner: all friends swiped, scene complete.");
                return;
            }

            ShowCurrentFriendOnPhone();
        }

        private void ShowCurrentFriendOnPhone()
        {
            phoneDisplayRenderer.sprite = _orderedFriends[_currentFriendIndex].appProfileSprite;
        }

        // Builds an ordered list from the friends list using a weighted bucket algorithm.
        // Friends are grouped by frame, each bucket is shuffled, then friends are picked
        // by always choosing from the frame with the lowest completion ratio.
        // Tie on ratio: pick the frame with more friends remaining.
        // Tie on remaining count: pick randomly.
        private void BuildOrderedFriendList()
        {
            // Group friends into buckets by assigned area and shuffle each bucket
            var buckets = new Dictionary<MiniGame2FrameArea, List<FriendData>>();
            foreach (var friend in friends)
            {
                if (!buckets.ContainsKey(friend.assignedArea))
                    buckets[friend.assignedArea] = new List<FriendData>();
                buckets[friend.assignedArea].Add(friend);
            }
            foreach (var bucket in buckets.Values)
                Shuffle(bucket);

            // Track how many have been chosen vs total per area
            var chosen = new Dictionary<MiniGame2FrameArea, int>();
            var totals = new Dictionary<MiniGame2FrameArea, int>();
            foreach (var kvp in buckets)
            {
                chosen[kvp.Key] = 0;
                totals[kvp.Key] = kvp.Value.Count;
            }

            _orderedFriends = new List<FriendData>();

            while (true)
            {
                // Collect non-empty buckets
                var available = new List<MiniGame2FrameArea>();
                foreach (var kvp in buckets)
                    if (kvp.Value.Count > 0)
                        available.Add(kvp.Key);

                if (available.Count == 0) break;

                // Find the lowest completion ratio among available buckets
                float lowestRatio = float.MaxValue;
                foreach (var area in available)
                    lowestRatio = Mathf.Min(lowestRatio, (float)chosen[area] / totals[area]);

                // Collect all buckets tied at that ratio
                var candidates = new List<MiniGame2FrameArea>();
                foreach (var area in available)
                    if ((float)chosen[area] / totals[area] <= lowestRatio + 0.0001f)
                        candidates.Add(area);

                // Tie-break: prefer the bucket with the most friends remaining
                MiniGame2FrameArea selected = candidates[0];
                int maxRemaining = buckets[candidates[0]].Count;
                foreach (var area in candidates)
                {
                    if (buckets[area].Count > maxRemaining)
                    {
                        maxRemaining = buckets[area].Count;
                        selected = area;
                    }
                    else if (buckets[area].Count == maxRemaining && UnityEngine.Random.value > 0.5f)
                    {
                        // Final tie-break: random
                        selected = area;
                    }
                }

                // Take the first friend from the shuffled bucket
                _orderedFriends.Add(buckets[selected][0]);
                buckets[selected].RemoveAt(0);
                chosen[selected]++;
            }
        }

        private void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private AreaBounds GetBoundsForArea(MiniGame2FrameArea area)
        {
            foreach (var b in areaBounds)
                if (b.area == area) return b;

            Debug.LogWarning($"No bounds found for area {area}, defaulting to first entry.");
            return areaBounds[0];
        }

        private void OnDrawGizmos()
        {
            if (areaBounds == null) return;

            foreach (var area in areaBounds)
            {
                Vector3 center = new Vector3(
                    (area.bottomLeft.x + area.topRight.x) / 2f,
                    (area.bottomLeft.y + area.topRight.y) / 2f,
                    0f
                );
                Vector3 size = new Vector3(
                    area.topRight.x - area.bottomLeft.x,
                    area.topRight.y - area.bottomLeft.y,
                    0f
                );

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(center, size);

#if UNITY_EDITOR
                GUIStyle style = new GUIStyle
                {
                    fontSize = 20,
                    normal = { textColor = Color.red }
                };
                UnityEditor.Handles.Label(center, area.area.ToString(), style);
#endif
            }
        }
    }
}
