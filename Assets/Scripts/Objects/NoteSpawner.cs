using System.Collections;
using System.Collections.Generic;
using Audio;
using Audio.AudioEmitters;
using Player.MiniGames;
using UnityEngine;

namespace Objects
{
    [System.Serializable]
    public struct SpawnerLevel
    {
        [Tooltip("Time in seconds between each note drop.")]
        public float spawnInterval;
        [Tooltip("How fast the notes fall in this level.")]
        public float fallSpeed;
        [Tooltip("How many seconds to stay on this level before automatically upgrading. (Set to 0 for the final level)")]
        public float timeToNextLevel;
    }

    public class NoteSpawner : MonoBehaviour
    {
        [SerializeField] PlayerControllerMiniGame5 playerController;

        [Header("Audio")]
        [SerializeField] private AudioEmitterBase ballotInsertedEmitter;

        [Header("Spawn Settings")]
        [SerializeField] private FallingNote notePrefab;
        [SerializeField, Tooltip("Exact point where ALL notes spawn from.")] 
        private Transform spawnOrigin;
    
        [SerializeField, Tooltip("Max angle (in degrees) notes can deviate from falling straight down. e.g., 30 = -30 to 30 degrees spread.")] 
        private float maxSpreadAngle = 30f;
        [SerializeField, Tooltip("Minimum angle difference from the previous note, so notes are less likely to fly in the same direction repeatedly.")]
        private float minAngleDifferenceFromPrevious = 12f;

        [Header("Difficulty Levels")]
        [SerializeField] private List<SpawnerLevel> difficultyLevels;
        [SerializeField] private int currentLevelIndex = 0;

        private bool _isSpawning = true;
        private bool _hasStarted;
        private float _lastSpreadAngle;
        private bool _hasLastSpreadAngle;

        public void StartGame()
        {
            if (_hasStarted) return; 
            
            _hasStarted = true;
            StartCoroutine(SpawnRoutine());
            StartCoroutine(AutoLevelUpRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (_isSpawning)
            {
                if (difficultyLevels.Count == 0) yield break;

                SpawnerLevel currentLevel = difficultyLevels[currentLevelIndex];
                SpawnNote(currentLevel.fallSpeed);

                yield return new WaitForSeconds(currentLevel.spawnInterval);
            }
        }

        private void SpawnNote(float speed)
        {
            // 1. All notes spawn at the exact same origin point
            Vector3 spawnPos = spawnOrigin.position;
            FallingNote newNote = Instantiate(notePrefab, spawnPos, Quaternion.identity);
            ballotInsertedEmitter?.PlayAudioOnce();

            // 2. Pick a random angle, avoiding directions that are too close to the previous note.
            float randomAngle = GetRandomSpreadAngle();
        
            // 3. Rotate the "Down" direction by that random angle to get our diagonal trajectory
            Vector2 fallDirection = Quaternion.Euler(0, 0, randomAngle) * Vector2.down;

            // 4. Pass both the speed and the new direction to the note
            newNote.Initialize(speed, fallDirection);
        }

        private float GetRandomSpreadAngle()
        {
            float randomAngle = Random.Range(-maxSpreadAngle, maxSpreadAngle);

            if (!_hasLastSpreadAngle || maxSpreadAngle <= 0f)
            {
                _lastSpreadAngle = randomAngle;
                _hasLastSpreadAngle = true;
                return randomAngle;
            }

            float usableMinDifference = Mathf.Clamp(minAngleDifferenceFromPrevious, 0f, maxSpreadAngle);
            const int maxAttempts = 8;

            for (int i = 0; i < maxAttempts && Mathf.Abs(randomAngle - _lastSpreadAngle) < usableMinDifference; i++)
            {
                randomAngle = Random.Range(-maxSpreadAngle, maxSpreadAngle);
            }

            if (Mathf.Abs(randomAngle - _lastSpreadAngle) < usableMinDifference)
            {
                randomAngle = _lastSpreadAngle >= 0f
                    ? Random.Range(-maxSpreadAngle, -usableMinDifference)
                    : Random.Range(usableMinDifference, maxSpreadAngle);
            }

            _lastSpreadAngle = randomAngle;
            return randomAngle;
        }
        
        private IEnumerator AutoLevelUpRoutine()
        {
            while (_isSpawning)
            {
                if (difficultyLevels.Count == 0) yield break;

                float waitTime = difficultyLevels[currentLevelIndex].timeToNextLevel;

                // If waitTime is 0, we assume this is the final level and we stop trying to upgrade
                if (waitTime <= 0) yield break; 

                yield return new WaitForSeconds(waitTime);

                MusicManager.Instance?.AdvanceProgress();

                // Time is up! Move to the next level if one exists
                if (currentLevelIndex < difficultyLevels.Count - 1)
                {
                    currentLevelIndex++;
                    Debug.Log("Level Up! Now on level: " + currentLevelIndex);
                }
                else
                {
                    EndGame();
                    yield break;
                }
            }
        }

        private void EndGame()
        {
            _isSpawning = false; 
    
            Debug.Log("Game Over! All levels completed.");
            
           StartCoroutine(playerController.GameEnded());
        }

        // Draws a helpful red cone in the Scene view so you can see where notes will fly!
        private void OnDrawGizmos()
        {
            if (spawnOrigin != null)
            {
                Gizmos.color = Color.red;
                Vector3 leftBound = Quaternion.Euler(0, 0, maxSpreadAngle) * Vector3.down * 3f;
                Vector3 rightBound = Quaternion.Euler(0, 0, -maxSpreadAngle) * Vector3.down * 3f;
                Gizmos.DrawRay(spawnOrigin.position, leftBound);
                Gizmos.DrawRay(spawnOrigin.position, rightBound);
            }
        }
    }
}
