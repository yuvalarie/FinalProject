using System;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    [CreateAssetMenu(fileName = "ChosenFriendsData", menuName = "Page13/Chosen Friends Data")]
    public class ChosenFriendsData : ScriptableObject
    {
        private static List<int> _friends = new List<int>();
        private static HashSet<int> _friendLookup = new HashSet<int>();

        public IReadOnlyList<int> Friends => _friends;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetRuntimeState()
        {
            _friends = new List<int>();
            _friendLookup = new HashSet<int>();
        }

        public void Clear()
        {
            EnsureInitialized();
            _friends.Clear();
            _friendLookup.Clear();
        }

        public bool AddFriend(int friendId)
        {
            EnsureInitialized();

            if (!_friendLookup.Add(friendId)) return false;

            _friends.Add(friendId);
            return true;
        }

        private static void EnsureInitialized()
        {
            if (_friends == null) _friends = new List<int>();
            if (_friendLookup == null) _friendLookup = new HashSet<int>(_friends);
        }
    }
}
