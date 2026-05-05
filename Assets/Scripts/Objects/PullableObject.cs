using UnityEngine;

namespace Objects
{
    [RequireComponent(typeof(Collider2D))]
    public class PullableObject : MonoBehaviour
    {
        private Collider2D _pullableCollider;
        [SerializeField] private Vector2 pullDirection = new Vector2(1, 0);

        private bool _isBeingPulled = false;

    }
}
 