using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio.AudioEmitters;
using DG.Tweening;
using UnityEngine;

namespace Objects
{
    [DisallowMultipleComponent]
    public class MechanicalCounter : MonoBehaviour
    {
        [Header("Value")]
        [SerializeField] private long startingValue;
        [SerializeField] private bool autoIncrement = true;
        [Min(0.05f)]
        [SerializeField] private float incrementEverySeconds = 1f;

        [Header("Digits")]
        [Tooltip("Leave empty to auto-use child SpriteRenderers whose names start with 0 through 9, left to right.")]
        [SerializeField] private SpriteRenderer[] digitColumns;
        [Tooltip("Leave empty to use the sprites from children named 0 through 9.")]
        [SerializeField] private Sprite[] digitSprites = new Sprite[10];
        [Tooltip("0 means use every discovered digit column.")]
        [Min(0)]
        [SerializeField] private int visibleDigitCount;
        [SerializeField] private bool padWithZeroes = true;
        [SerializeField] private bool hideUnusedColumns = true;

        [Header("Roll Animation")]
        [Min(0.01f)]
        [SerializeField] private float rollDuration = 0.35f;
        [SerializeField] private Ease rollEase = Ease.OutCubic;
        [Tooltip("How far a digit sprite travels vertically during one roll.")]
        [Min(0.01f)]
        [SerializeField] private float rollDistance = 0.5f;
        
        [Header("Scene Continuity")]
        [SerializeField] private bool saveValueForNextScene = true;
        [SerializeField] private bool loadSavedValueOnStart = true;
        [SerializeField] private string persistenceKey = "WishesFulfilled";

        [Header("Audio")]
        [SerializeField] private AudioEmitterBase incrementAudioEmitter;
        [Tooltip("Delay added between each rolling digit's sound, so multiple digits changing at once don't clash.")]
        [SerializeField] private float digitAudioStagger = 0.05f;

        private static readonly Dictionary<string, long> PersistedValues = new();

        private readonly List<SpriteRenderer> temporaryRenderers = new();
        private Vector3[] homeLocalPositions;
        private int[] shownDigits;
        private long currentValue;
        private Sequence activeRoll;
        private Coroutine incrementRoutine;
        private bool initialized;

private void Awake()
        {
            Initialize();
            long initialValue = startingValue;

            if (loadSavedValueOnStart && PersistedValues.TryGetValue(persistenceKey, out long savedValue))
            {
                initialValue = savedValue;
            }

            SetValueInstant(initialValue);
        }

        private void OnEnable()
        {
            if (initialized && autoIncrement)
            {
                incrementRoutine = StartCoroutine(AutoIncrementRoutine());
            }
        }

        private void OnDisable()
        {
            StopAutoIncrement();
            activeRoll?.Kill();
            SavePersistedValue();
            ClearTemporaryRenderers();
        }

        public void SetValue(long value)
        {
            SetValue(value, true);
        }

        public void SetValue(long value, bool animate)
        {
            Initialize();

            value = value < 0L ? 0L : value;
            int[] nextDigits = ToDigits(value);
            currentValue = value;
            SavePersistedValue();

            activeRoll?.Kill();
            ClearTemporaryRenderers();

            if (!animate || shownDigits == null)
            {
                ApplyDigitsInstant(nextDigits);
                return;
            }

            activeRoll = DOTween.Sequence();
            bool anyDigitChanged = false;
            int changedDigitCount = 0;

            for (int i = 0; i < digitColumns.Length; i++)
            {
                digitColumns[i].transform.localPosition = homeLocalPositions[i];

                if (shownDigits[i] == nextDigits[i])
                {
                    digitColumns[i].sprite = digitSprites[shownDigits[i]];
                    continue;
                }

                anyDigitChanged = true;
                activeRoll.Join(RollDigit(i, nextDigits[i]));
                incrementAudioEmitter?.PlayAudioOnceWithDelay(changedDigitCount * digitAudioStagger);
                changedDigitCount++;
            }

            if (!anyDigitChanged)
            {
                return;
            }
            // incrementAudioEmitter?.PlayAudioOnce(); // if we want one audio for the whole roll instead of per digit, we can do this instead of the staggered ones above
            activeRoll.OnComplete(() => ApplyDigitsInstant(nextDigits));
        }

        public void SetValueInstant(long value)
        {
            Initialize();
            value = value < 0L ? 0L : value;
            ApplyDigitsInstant(ToDigits(value));
            currentValue = value;
            SavePersistedValue();
        }

        public void Increment()
        {
            SetValue(currentValue == long.MaxValue ? 0L : currentValue + 1L);
        }

        private void Initialize()
        {
            if (initialized)
            {
                return;
            }

            AutoDiscoverDigits();

            if (digitColumns == null || digitColumns.Length == 0)
            {
                Debug.LogWarning($"{nameof(MechanicalCounter)} on {name} could not find any digit columns.", this);
                return;
            }

            int activeDigitCount = visibleDigitCount <= 0 ? digitColumns.Length : Mathf.Min(visibleDigitCount, digitColumns.Length);
            digitColumns = digitColumns.Take(activeDigitCount).ToArray();
            homeLocalPositions = digitColumns.Select(column => column.transform.localPosition).ToArray();
            shownDigits = new int[digitColumns.Length];

            for (int i = 0; i < digitColumns.Length; i++)
            {
                digitColumns[i].enabled = true;
            }

            initialized = true;
        }

        private void AutoDiscoverDigits()
        {
            SpriteRenderer[] discovered = GetComponentsInChildren<SpriteRenderer>(true)
                .Where(renderer => TryGetDigitFromName(renderer.name, out _))
                .OrderBy(renderer => renderer.transform.localPosition.x)
                .ToArray();

            if (digitColumns == null || digitColumns.Length == 0)
            {
                digitColumns = discovered;
            }

            if (digitSprites == null || digitSprites.Length != 10)
            {
                digitSprites = new Sprite[10];
            }

            foreach (SpriteRenderer renderer in discovered)
            {
                if (TryGetDigitFromName(renderer.name, out int digit) && renderer.sprite != null && digitSprites[digit] == null)
                {
                    digitSprites[digit] = renderer.sprite;
                }
            }

            if (hideUnusedColumns && digitColumns != null && visibleDigitCount > 0)
            {
                for (int i = visibleDigitCount; i < digitColumns.Length; i++)
                {
                    digitColumns[i].enabled = false;
                }
            }
        }

        private static bool TryGetDigitFromName(string objectName, out int digit)
        {
            digit = 0;

            if (string.IsNullOrEmpty(objectName) || !char.IsDigit(objectName[0]))
            {
                return false;
            }

            digit = objectName[0] - '0';
            return true;
        }

        private int[] ToDigits(long value)
        {
            string valueText = value.ToString();
            int digitAmount = digitColumns.Length;

            if (valueText.Length > digitAmount)
            {
                valueText = valueText.Substring(valueText.Length - digitAmount, digitAmount);
            }
            else if (padWithZeroes)
            {
                valueText = valueText.PadLeft(digitAmount, '0');
            }

            if (!padWithZeroes)
            {
                valueText = valueText.PadLeft(digitAmount, ' ');
            }

            int[] digits = new int[digitAmount];
            for (int i = 0; i < digitAmount; i++)
            {
                digits[i] = char.IsDigit(valueText[i]) ? valueText[i] - '0' : 0;
            }

            return digits;
        }

        private Tween RollDigit(int index, int nextDigit)
        {
            SpriteRenderer currentColumn = digitColumns[index];
            Transform currentTransform = currentColumn.transform;
            Vector3 homePosition = homeLocalPositions[index];

            SpriteRenderer incomingColumn = CreateTemporaryRenderer(currentColumn, nextDigit, index);
            Transform incomingTransform = incomingColumn.transform;
            incomingTransform.localPosition = homePosition - Vector3.up * rollDistance;

            Sequence roll = DOTween.Sequence();
            roll.Join(currentTransform.DOLocalMoveY(homePosition.y + rollDistance, rollDuration).SetEase(rollEase));
            roll.Join(incomingTransform.DOLocalMoveY(homePosition.y, rollDuration).SetEase(rollEase));
            return roll;
        }

        private SpriteRenderer CreateTemporaryRenderer(SpriteRenderer source, int digit, int index)
        {
            GameObject temporary = new GameObject($"{source.name}_RollingTo_{digit}");
            temporary.transform.SetParent(source.transform.parent, false);
            temporary.transform.localPosition = homeLocalPositions[index];
            temporary.transform.localRotation = source.transform.localRotation;
            temporary.transform.localScale = source.transform.localScale;

            SpriteRenderer renderer = temporary.AddComponent<SpriteRenderer>();
            renderer.sprite = digitSprites[digit];
            renderer.color = source.color;
            renderer.flipX = source.flipX;
            renderer.flipY = source.flipY;
            renderer.maskInteraction = source.maskInteraction;
            renderer.sortingLayerID = source.sortingLayerID;
            renderer.sortingOrder = source.sortingOrder;
            renderer.sharedMaterial = source.sharedMaterial;

            temporaryRenderers.Add(renderer);
            return renderer;
        }

        private void ApplyDigitsInstant(int[] digits)
        {
            for (int i = 0; i < digitColumns.Length; i++)
            {
                digitColumns[i].transform.localPosition = homeLocalPositions[i];
                digitColumns[i].sprite = digitSprites[digits[i]];
                shownDigits[i] = digits[i];
            }

            ClearTemporaryRenderers();
        }

        private IEnumerator AutoIncrementRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(incrementEverySeconds);
                Increment();
            }
        }

        private void StopAutoIncrement()
        {
            if (incrementRoutine == null)
            {
                return;
            }

            StopCoroutine(incrementRoutine);
            incrementRoutine = null;
        }

        private void ClearTemporaryRenderers()
        {
            foreach (SpriteRenderer renderer in temporaryRenderers)
            {
                if (renderer != null)
                {
                    Destroy(renderer.gameObject);
                }
            }

            temporaryRenderers.Clear();
        }
    

private void SavePersistedValue()
        {
            if (!saveValueForNextScene || string.IsNullOrEmpty(persistenceKey))
            {
                return;
            }

            PersistedValues[persistenceKey] = currentValue;
        }
        public static void ClearPersistedValues()
        {
            PersistedValues.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetPersistedValues()
        {
            ClearPersistedValues();
        }
}
}
