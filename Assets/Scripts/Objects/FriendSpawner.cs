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
        [SerializeField] private GameObject phoneDisplayObject;
        private SpriteRenderer _phoneDisplayRenderer;

        [Header("Areas")] [SerializeField] private List<AreaBounds> areaBounds;

        [Header("References")] [SerializeField]
        private HellPortal hellPortal;

        private FriendController _currentShowingFriend;
        private int _currentFriendIndex = 0;

        private void Start()
        {
            if (phoneDisplayObject)
            {
                _phoneDisplayRenderer = phoneDisplayObject.GetComponent<SpriteRenderer>();
                if (_phoneDisplayRenderer == null)
                {
                    Debug.LogError("Phone display object does not have a SpriteRenderer component.");
                }
            }
            
            ShowFriendOnPhone(); // for now we just show the first friend, will add later to call this only when we actually start the minigame
        }

        public void SpawnFriend()
        {
            FriendData data = friends[_currentFriendIndex];
            AreaBounds bounds = GetBoundsForArea(data.assignedArea);

            var instance = Instantiate(friendBasePrefab);
            var controller = instance.GetComponent<FriendController>();
            controller.Setup(data);

            Bounds worldBounds = new Bounds(
                new Vector3((bounds.bottomLeft.x + bounds.topRight.x) / 2f,
                    (bounds.bottomLeft.y + bounds.topRight.y) / 2f, 0f),
                new Vector3(bounds.topRight.x - bounds.bottomLeft.x,
                    bounds.topRight.y - bounds.bottomLeft.y, 0f)
            );

            float spawnX = UnityEngine.Random.Range(bounds.bottomLeft.x, bounds.topRight.x);
            float spawnY = UnityEngine.Random.Range(bounds.bottomLeft.y, bounds.topRight.y);
            instance.transform.position = new Vector3(spawnX, spawnY, 0f);

            controller.StartRoaming(worldBounds);
            ShowNextFriend();
        }

        public void DiscardFriend()
        {
            if (_currentShowingFriend == null) return;
            hellPortal.SuckIn(_currentShowingFriend);
            ShowNextFriend();
        }

        private void ShowNextFriend()
        {
            _currentFriendIndex = (_currentFriendIndex + 1) % friends.Count;
            ShowFriendOnPhone();
        }

        private void ShowFriendOnPhone()
        {
            _phoneDisplayRenderer.sprite = friends[_currentFriendIndex].appProfileSprite;
            _currentShowingFriend = null;
        }

        private AreaBounds GetBoundsForArea(MiniGame2FrameArea area)
        {
            foreach (var b in areaBounds)
                if (b.area == area)
                    return b;

            Debug.LogWarning($"No bounds found for area {area}, defaulting to first entry.");
            return areaBounds[0];
        }
    }
}