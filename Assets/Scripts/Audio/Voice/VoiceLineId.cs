using System;
using System.Collections.Generic;

namespace Audio.Voice
{
    public static class VoiceLineId
    {
        [Serializable]
        public struct SceneInitialEntry
        {
            public string sceneName;
            public string initial;
        }

        [Serializable]
        public struct CharacterNameEntry
        {
            public CharacterType character;
            public string name;
        }

        private static readonly Dictionary<CharacterType, string> CharacterNames = new();
        private static readonly Dictionary<string, string> SceneInitials = new();

        public static void LoadCharacterNames(List<CharacterNameEntry> entries)
        {
            CharacterNames.Clear();
            foreach (var entry in entries)
                CharacterNames[entry.character] = entry.name;
        }

        public static void LoadSceneInitials(List<SceneInitialEntry> entries)
        {
            SceneInitials.Clear();
            foreach (var entry in entries)
                SceneInitials[entry.sceneName] = entry.initial;
        }

        public static string Build(string sceneName, CharacterType character, int lineNumber)
        {
            string sceneInitial = SceneInitials.GetValueOrDefault(sceneName, "");
            return $"{sceneInitial}_{CharacterNames[character]}_{lineNumber:D2}";
        }
    }
}
