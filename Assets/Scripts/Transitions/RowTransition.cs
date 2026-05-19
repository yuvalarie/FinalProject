using UnityEngine;

namespace Transitions
{
    public class RowTransition : MonoBehaviour
    {
        [Header("Teleport Destination")]
        [Tooltip("The exact Transform where the player will appear in the new row.")]
        [SerializeField] public Transform destinationSpawn;

        [Header("Perspective Settings")]
        [Tooltip("The scale the player should instantly become when arriving at this row.")]
        [SerializeField] public float targetScale = 1f;
        [SerializeField] public int sortingOrder = 0;
        [SerializeField] public float targetSpeed;
    }
}