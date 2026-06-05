  using System;
  using System.Collections;
  using Objects;
  using UnityEngine;
  using Random = UnityEngine.Random;

  namespace Npc
  {
      public class FriendController : MonoBehaviour
      {
          [Header("Art References")]
          [SerializeField] private SpriteRenderer friendSpriteRenderer;
          [SerializeField] private SpriteRenderer speechBubbleSpriteRenderer;
          [SerializeField] private GameObject speechBubble;

          [Header("Throw Settings")]
          [SerializeField] private float throwDuration = 0.6f;
          [SerializeField] private float throwRotationSpeed = 360f;

          [Header("Roaming Settings")]
          [SerializeField] private float movementDuration = 1.5f;
          [SerializeField] private float bobAmplitude = 0.25f;
          [SerializeField] private float bobFrequency = 2f;
          [SerializeField] private float minIdleTime = 0.5f;
          [SerializeField] private float maxIdleTime = 2f;

          private Sprite _thrownSprite;
          private Bounds _areaBounds;
          // TODO: add a way to offset the speech bubble position that will be relative to the size of the sprite of the roaming friend

          // TODO: add a flip to the X of the sprite when we walk to the right, as all the friends are looking to the left by default.
          // private void Start()
          // {
          //     var testAreaBounds = new Bounds(new Vector3(4.9f, -3.67f, 0f), new Vector3(5f, 0f, 0f)); // Default to current position if not set up
          //     StartRoaming(testAreaBounds);
          // }

          public void Setup(FriendData data)
          {
              friendSpriteRenderer.sprite = data.roamingSprite;
              speechBubbleSpriteRenderer.sprite = data.speechBubbleSprite;
              _thrownSprite = data.thrownSprite;
          }

          public void StartRoaming(Bounds areaBounds)
          {
              _areaBounds = areaBounds;
              StartCoroutine(RoamingRoutine());
          }

          public void ShowSpeechBubble()
          {
              speechBubble.SetActive(true);
          }

          public void HideSpeechBubble()
          {
              speechBubble.SetActive(false);
          }

          public void GetThrown(Vector3 portalPosition)
          {
              friendSpriteRenderer.sprite = _thrownSprite;
              StopAllCoroutines();
              StartCoroutine(FlyToPortalRoutine(portalPosition));
          }

          private IEnumerator RoamingRoutine()
          {
              while (true)
              {
                  Vector3 target = new Vector3(
                      Random.Range(_areaBounds.min.x, _areaBounds.max.x),
                      Random.Range(_areaBounds.min.y, _areaBounds.max.y),
                      transform.position.z
                  );

                  yield return MoveRoutine(target);

                  float idleTime = Random.Range(minIdleTime, maxIdleTime);
                  yield return new WaitForSeconds(idleTime);
              }
          }

          private IEnumerator MoveRoutine(Vector3 target)
          {
              Vector3 startPosition = transform.position;
              float elapsed = 0f;

              while (elapsed < movementDuration)
              {
                  elapsed += Time.deltaTime;
                  float t = elapsed / movementDuration;

                  transform.position = Vector3.Lerp(startPosition, target, t);
                  float bobOffset = Mathf.Sin(t * bobFrequency * Mathf.PI * 2) * bobAmplitude;
                  transform.position += new Vector3(0f, -Mathf.Abs(bobOffset), 0f);
                  transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(t * Mathf.PI * 2) * 2f);

                  yield return null;
              }

              transform.position = target;
              transform.localRotation = Quaternion.identity;
          }

          private IEnumerator FlyToPortalRoutine(Vector3 portalPosition)
          {
              float elapsed = 0f;
              Vector3 startPosition = transform.position;

              while (elapsed < throwDuration)
              {
                  elapsed += Time.deltaTime;
                  float t = elapsed / throwDuration;
                  transform.position = Vector3.Lerp(startPosition, portalPosition, t);
                  transform.Rotate(0f, 0f, throwRotationSpeed * Time.deltaTime);
                  yield return null;
              }

              Destroy(gameObject);
          }
      }
  }