using UnityEngine;

namespace Objects
{
    public class FollowXAxisObject : MonoBehaviour
    {
        [Tooltip("The object we want to follow on the X axis")]
        [SerializeField] private Transform targetToFollow;

        void Update()
        {
            if (targetToFollow != null)
            {
                transform.position = new Vector3(targetToFollow.position.x, transform.position.y, transform.position.z);
            }
        }
    }
}