using System;
using System.Collections.Generic;
using Objects.Poster;
using Unity.Collections;
using UnityEngine;

namespace Objects
{
    [Serializable]
    public struct FriendEntry
    {
        public int idKey;
        public FriendsDataPage13 friendData;
    }
    
    [CreateAssetMenu(fileName = "FriendsDictionary", menuName = "Page13/Friends Dictionary")]

    public class FriendsDictionary : ScriptableObject
    {
        [SerializeField, Tooltip("Add your friends here. This list builds the dictionary.")]
        private List<FriendEntry> inspectorFriendsList = new List<FriendEntry>();
        
        [HideInInspector]
        public Dictionary<int, FriendsDataPage13> friendsDictionary = new Dictionary<int, FriendsDataPage13>();
        
        private void OnEnable()
        {
            friendsDictionary.Clear();
            foreach (FriendEntry entry in inspectorFriendsList)
            {
                if (!friendsDictionary.ContainsKey(entry.idKey))
                {
                    friendsDictionary.Add(entry.idKey, entry.friendData);
                }
                else
                {
                    Debug.LogWarning($"Wait! You have duplicate keys ({entry.idKey}) in your FriendsDictionary ScriptableObject!");
                }
            }
        }
    }
}