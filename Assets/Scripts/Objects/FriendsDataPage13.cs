using UnityEngine;

namespace Objects.Poster
{
    [CreateAssetMenu(fileName = "FriendsDataPage13", menuName = "Page13/Friend Data")]
    public class FriendsDataPage13 : ScriptableObject
    {
        public int id;
        public GameObject frame2Object;
        public GameObject frame4Object;
        public GameObject hand;
        public ShoeType shoeType;
    }

    public enum ShoeType
    {
        Sneakers,
        Heels
    }
}