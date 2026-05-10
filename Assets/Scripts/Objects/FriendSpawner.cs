using System;
using System.Collections.Generic;
using Npc;
using UnityEngine;

namespace Objects
{
    [Serializable]
    public struct Vector2Pair
    {
        public Vector2 bottomLeft;
        public Vector2 topRight;

        public Vector2Pair(Vector2 bottomLeft, Vector2 topRight)
        {
            this.bottomLeft = bottomLeft;
            this.topRight = topRight;
        }
    }
    public class FriendSpawner : MonoBehaviour
    {
        [SerializeField] private List<Vector2Pair> spawnAreas; // List of tuples containing the bottom-left and top-right corners of spawn areas
        [SerializeField] private List<GameObject> friendPrefabs;
        [SerializeField] private Transform friendShowPosition;
        
        private GameObject _currentShowingFriend;
        private int _currentFriendIndex = 0;

        private void Start()
        {
            ShowFriendAtPosition();
        }
        
        
        public void SpawnFriend()
        {
            if (friendPrefabs.Count == 0 || spawnAreas.Count == 0)
            {
                Debug.LogWarning("No friend prefabs or spawn areas defined.");
                return;
            }

            // Get the current friend prefab to spawn
            GameObject friendPrefab = friendPrefabs[_currentFriendIndex];

            // Randomly select a spawn area
            Vector2Pair spawnArea = spawnAreas[UnityEngine.Random.Range(0, spawnAreas.Count)];

            // Generate a random position within the selected spawn area
            float randomX = UnityEngine.Random.Range(spawnArea.bottomLeft.x, spawnArea.topRight.x);
            float randomY = UnityEngine.Random.Range(spawnArea.bottomLeft.y, spawnArea.topRight.y);
            Vector2 spawnPosition = new Vector2(randomX, randomY);

            // Instantiate the friend prefab at the generated position
            var newFriend = Instantiate(friendPrefab, spawnPosition, Quaternion.identity);
            newFriend.GetComponent<RandomMovementNpc>().StartRandomMovement();

            // // Move to the next friend prefab for the next spawn
            // _currentFriendIndex = (_currentFriendIndex + 1) % friendPrefabs.Count;
            ShowNextFriend();
        }
        
        public void ShowNextFriend()
        {
            Destroy(_currentShowingFriend);
            _currentFriendIndex = (_currentFriendIndex + 1) % friendPrefabs.Count;
            ShowFriendAtPosition();
        }
        
        private void ShowFriendAtPosition()
        {
            if (friendPrefabs.Count == 0 || friendShowPosition == null)
            {
                Debug.LogWarning("No friend prefabs or show position defined.");
                return;
            }
            
            // Instantiate the current friend prefab at the designated show position
            _currentShowingFriend = Instantiate(friendPrefabs[_currentFriendIndex], friendShowPosition.position, Quaternion.identity);
        }
        
    }
}
