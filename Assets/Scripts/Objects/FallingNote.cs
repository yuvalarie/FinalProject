using System.Collections.Generic;
using UnityEngine;

public class FallingNote : MonoBehaviour
{
    [Header("Scatter Settings")]
    [SerializeField, Tooltip("How much they jiggle out of place when landing.")] 
    private float scatterPositionAmount = 0.2f;
    [SerializeField, Tooltip("How much they rotate when landing.")] 
    private float scatterRotationAmount = 15f;

    private float _speed;
    private Vector3 _fallDirection;
    private bool _isFalling = true;
    
    private bool _isWaitingToLand = false;
    private float _targetLandY;
    private Transform _floorTransform;

    // Notice we now accept a Vector2 for direction!
    public void Initialize(float fallSpeed, Vector2 direction)
    {
        _speed = fallSpeed;
        _fallDirection = new Vector3(direction.x, direction.y, 0f).normalized;
    }

    private void Update()
    {
        if (_isFalling)
        {
            // Move along the custom diagonal direction we generated in the spawner
            transform.Translate(_fallDirection * (_speed * Time.deltaTime), Space.World);
            
             // If we touched the floor box, keep falling until we hit our randomly assigned depth!
            if (_isWaitingToLand && transform.position.y <= _targetLandY)
            {
                LandAndScatter(_floorTransform, false);
            }
        }
    }
    
    public bool CatchByPlayer(Transform catchZoneTransform, List<FallingNote> caughtNotes)
    {
        // If it already landed on the floor, we can't catch it anymore
        if (!_isFalling || _isWaitingToLand) return false;

        LandAndScatter(catchZoneTransform, true);
        caughtNotes.Add(this);
        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isFalling || _isWaitingToLand) return; 

        // if (other.CompareTag("CatchZone"))
        // {
        //     // Hit the player! We DO want it to snap to the center of the CatchZone
        //     LandAndScatter(other.transform, true);
        // }
        if (other.CompareTag("Floor"))
        {
            _isWaitingToLand = true;
            _floorTransform = other.transform;
            _targetLandY = Random.Range(other.bounds.min.y, other.bounds.max.y);
        }
    }

    private void LandAndScatter(Transform newParent, bool snapToParentCenter)
    {
        _isFalling = false;
        
        if (newParent != null)
        {
            transform.SetParent(newParent);
        }

        // Apply a random position offset to simulate a messy pile
        float randomX = Random.Range(-scatterPositionAmount, scatterPositionAmount);
        float randomY = Random.Range(-scatterPositionAmount, scatterPositionAmount);
        
        if (snapToParentCenter)
        {
            // Teleport to the center of the player's hands (with a little jiggle)
            transform.localPosition = new Vector3(randomX, randomY, -0.01f);
        }
        else
        {
            // Keep its current world position where it hit the floor, just add the jiggle!
            transform.position += new Vector3(randomX, randomY, -0.01f);
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y * 0.01f);
        }

        // Apply a random rotation
        float randomZRot = Random.Range(-scatterRotationAmount, scatterRotationAmount);
        transform.localRotation = Quaternion.Euler(0f, 0f, randomZRot);
    }
}