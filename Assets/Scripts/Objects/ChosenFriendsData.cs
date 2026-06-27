using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Objects
{
    [CreateAssetMenu(fileName = "ChosenFriendsData", menuName = "Page13/Chosen Friends Data")]
    public class ChosenFriendsData : ScriptableObject
    {
        public List<int> friends = new List<int>();
    }
}