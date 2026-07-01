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
          [SerializeField] private GameObject poof;

          [Header("Speech Bubble")]
          [Tooltip("Extra vertical gap between the top of the roaming sprite and the speech bubble.")]
          [SerializeField] private float speechBubbleOffsetY = 0.2f;

          [Header("Throw Settings")]
          [SerializeField] private float throwDuration = 0.6f;
          [SerializeField, Tooltip("Degrees per second")] private float throwRotationSpeed = 360f;

          [Header("Roaming Settings")]
          [SerializeField] private float movementDuration = 1.5f;
          [SerializeField] private float bobAmplitude = 0.25f;
          [SerializeField] private float bobFrequency = 2f;
          [SerializeField] private float minIdleTime = 0.5f;
          [SerializeField] private float maxIdleTime = 2f;

          private Sprite _thrownSprite;
          private Bounds _areaBounds;
          private string _thrownSortingLayer;
          private int _thrownSortingOrder;
          private float _poofDuration = 0.5f;

          // TODO: add a flip to the X of the sprite when we walk to the right, as all the friends are looking to the left by default.
          // private void Start()
          // {
          //     var testAreaBounds = new Bounds(new Vector3(4.9f, -3.67f, 0f), new Vector3(5f, 0f, 0f)); // Default to current position if not set up
          //     StartRoaming(testAreaBounds);
          // }

          public void SetRoamingSettings(float newBobAmplitude, float newBobFrequency)
          {
              bobAmplitude = newBobAmplitude;
              bobFrequency = newBobFrequency;
          }

          public void SetLayers(string roamingLayer, int roamingOrder, string thrownLayer, int thrownOrder)
          {
              friendSpriteRenderer.sortingLayerName = roamingLayer;
              friendSpriteRenderer.sortingOrder = roamingOrder;
              speechBubbleSpriteRenderer.sortingLayerName = roamingLayer;
              speechBubbleSpriteRenderer.sortingOrder = roamingOrder + 2;
              _thrownSortingLayer = thrownLayer;
              _thrownSortingOrder = thrownOrder;
          }

          public void Setup(FriendData data)
          {
              friendSpriteRenderer.sprite = data.roamingSprite;
              speechBubbleSpriteRenderer.sprite = data.speechBubbleSprite;
              _thrownSprite = data.thrownSprite;

              float spriteHeight = data.roamingSprite.bounds.size.y * friendSpriteRenderer.transform.localScale.y;
              Vector3 bubblePos = speechBubble.transform.localPosition;
              bubblePos.y = spriteHeight + speechBubbleOffsetY;
              speechBubble.transform.localPosition = bubblePos;
          }

          private void StartRoaming(Bounds areaBounds)
          {
              _areaBounds = areaBounds;
              StartCoroutine(RoamingRoutine());
          }

          private void ShowSpeechBubble()
          {
              speechBubble.SetActive(true);
          }

          private void HideSpeechBubble()
          {
              speechBubble.SetActive(false);
          }

          // public void ShowSpeechBubbleForDuration(float duration)
          // {
          //     StartCoroutine(SpeechBubbleRoutine(duration));
          // }

          public void ShowSpeechBubbleThenRoam(Bounds areaBounds, float duration)
          {
              StartCoroutine(SpeechBubbleThenRoamRoutine(areaBounds, duration));
          }

          // private IEnumerator SpeechBubbleRoutine(float duration)
          // {
          //     ShowSpeechBubble();
          //     yield return new WaitForSeconds(duration);
          //     HideSpeechBubble();
          // }

          private IEnumerator SpeechBubbleThenRoamRoutine(Bounds areaBounds, float duration)
          {
              poof.SetActive(true);
              yield return new WaitForSeconds(_poofDuration);
              poof.SetActive(false);
              ShowSpeechBubble();
              yield return new WaitForSeconds(duration);
              HideSpeechBubble();
              StartRoaming(areaBounds);
          }

          public void GetThrown(Vector3 portalPosition, Action onComplete = null)
          {
              HideSpeechBubble();
              friendSpriteRenderer.sprite = _thrownSprite;
              friendSpriteRenderer.sortingLayerName = _thrownSortingLayer;
              friendSpriteRenderer.sortingOrder = _thrownSortingOrder;
              StopAllCoroutines();
              StartCoroutine(FlyToPortalRoutine(portalPosition, onComplete));
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
              friendSpriteRenderer.flipX = startPosition.x < target.x;
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

          private IEnumerator FlyToPortalRoutine(Vector3 portalPosition, Action onComplete)
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
              onComplete?.Invoke();
              Destroy(gameObject);
          }
          
          public void Leave(Vector3 leavePoint, Action onComplete = null)
          {
              HideSpeechBubble();
              StopAllCoroutines();
              StartCoroutine(LeaveRoutine(leavePoint, onComplete));
          }

          private IEnumerator LeaveRoutine(Vector3 leavePoint, Action onComplete)
          {
              yield return MoveRoutine(leavePoint);
              onComplete?.Invoke();
              Destroy(gameObject);
          }
      }
  }