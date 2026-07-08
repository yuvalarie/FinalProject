using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Audio.Voice
{
    public class VoiceLineSequence : MonoBehaviour
    {
        [Serializable]
        private struct LineEntry
        {
            public CharacterType character;
            public int lineNumber;
        }

        [Header("Voice Line Sequence")]
        [SerializeField] private List<LineEntry> lines;

        private readonly List<EventInstance> _activeInstances = new();

        private void OnDestroy()
        {
            StopAll();
        }

        public void PlayLineByIndex(int index)
        {
            if (!VoiceSystem.Enabled) return;
            if (index < 0 || index >= lines.Count)
            {
                Debug.LogWarning($"VoiceLineSequence: index {index} out of range on {name}");
                return;
            }

            var entry = lines[index];
            var instance = VoiceManager.Instance.PlayLine(entry.character, entry.lineNumber, null);
            _activeInstances.Add(instance);
        }

        public void StopAll()
        {
            foreach (var instance in _activeInstances)
            {
                if (instance.isValid())
                    instance.stop(STOP_MODE.IMMEDIATE);
            }
            _activeInstances.Clear();
        }

        public void CleanInstances()
        {
            _activeInstances.RemoveAll(instance => !instance.isValid());
        }
    }
}
