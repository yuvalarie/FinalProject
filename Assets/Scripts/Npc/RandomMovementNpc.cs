using System;
using System.Collections;
using Objects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Npc
{
    public class RandomMovementNpc : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float minMoveTime = 1f;
        [SerializeField] private float maxMoveTime = 3f;
        [SerializeField] private float minIdleTime = 1f;
        [SerializeField] private float maxIdleTime = 3f;
        private bool _inPortal = false;
        private Vector2 _movementDirection;

        private void Start()
        {
            // StartCoroutine(RandomMovementRoutine());
        }
        
        public void StartRandomMovement()
        {
            StartCoroutine(RandomMovementRoutine());
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.GetComponent<Portal>() != null)
            {
                _inPortal = true;
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if(other.GetComponent<Portal>() != null)
            {
                _inPortal = false;
            }
        }

        private IEnumerator RandomMovementRoutine()
        {
            while (true)
            {
                float moveDirectionX = Random.Range(-1f, 1f);
                _movementDirection = moveDirectionX < 0 ? Vector2.left : Vector2.right;
                float moveTime = Random.Range(minMoveTime, maxMoveTime);
                // StartCoroutine(HandleMovement(moveTime));
                yield return HandleMovement(moveTime);
                if (_inPortal)
                {
                    yield return MoveUntilPortalExit();
                }
                float idleTime = Random.Range(minIdleTime, maxIdleTime);
                yield return new WaitForSeconds(idleTime);
            }
        }

        private IEnumerator HandleMovement(float moveTime)
        {
            float elapsedTime = 0f;
            while (elapsedTime < moveTime)
            {
                transform.Translate(_movementDirection * moveSpeed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
        }

        private IEnumerator MoveUntilPortalExit()
        {
            while (_inPortal)
            {
                transform.Translate(_movementDirection * moveSpeed * Time.deltaTime);
                yield return null; // Wait for the next frame
            }
        }
    }
}
