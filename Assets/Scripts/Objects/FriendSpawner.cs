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

        [Header("References")]
        [SerializeField] private HellPortal hellPortal;

        private int _currentFriendIndex = 0;

        // Called by MiniGame2SceneController when the hand finishes entering the scene
        public void StartSpawning()
        {
            ShowCurrentFriendOnPhone();
        }

        // Called by MiniGame2HandController on right swipe
        // Spawns the friend into their assigned comic panel and shows the next friend on the phone
        public void SpawnFriend()
        {
            FriendData data = friends[_currentFriendIndex];
            AreaBounds bounds = GetBoundsForArea(data.assignedArea);

            // Pick a random spawn position within the assigned frame
            float spawnX = UnityEngine.Random.Range(bounds.bottomLeft.x, bounds.topRight.x);
            float spawnY = UnityEngine.Random.Range(bounds.bottomLeft.y, bounds.topRight.y);

            var instance = Instantiate(friendBasePrefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);
            var controller = instance.GetComponent<FriendController>();
            controller.Setup(data);

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
            FriendData data = friends[_currentFriendIndex];

            var instance = Instantiate(friendBasePrefab, discardSpawnPosition.position, Quaternion.identity);
            var controller = instance.GetComponent<FriendController>();
            controller.Setup(data);

            hellPortal.SuckIn(controller);
            AdvanceToNextFriend();
        }

        private void AdvanceToNextFriend()
        {
            _currentFriendIndex++;

            if (_currentFriendIndex >= friends.Count)
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
            phoneDisplayRenderer.sprite = friends[_currentFriendIndex].appProfileSprite;
        }

        private AreaBounds GetBoundsForArea(MiniGame2FrameArea area)
        {
            foreach (var b in areaBounds)
                if (b.area == area) return b;

            Debug.LogWarning($"No bounds found for area {area}, defaulting to first entry.");
            return areaBounds[0];
        }
    }
}
