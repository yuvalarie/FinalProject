using System.Collections.Generic;

namespace DebugTools
{
    public static class DebugSceneCatalog
    {
        public const int FirstSceneSlot = 1;

        private static readonly DebugSceneEntry[] SceneEntries =
        {
            Reserved("StartScreen"),
            Reserved("Reserved - Scene 2"),
            Scene("Page 1"),
            Scene("Page 2"),
            Scene("Page 3"),
            Scene("Page 4"),
            Scene("MiniGame1-WithArt"),
            Scene("Page 6"),
            Scene("MiniGame1.5"),
            Scene("MiniGame2"),
            Scene("Page 8"),
            Scene("MiniGame3"),
            Scene("Page 11"),
            Scene("MiniGame4"),
            Scene("Page 13"),
            Scene("MiniGame4P2", "MIniGame4P2"),
            Scene("Page 15"),
            Scene("Page 16"),
            Reserved("Reserved - Ending Scene 1"),
            Reserved("Reserved - Ending Scene 2"),
            Reserved("Reserved - Ending Scene 3"),
        };

        public static IReadOnlyList<DebugSceneEntry> Entries => SceneEntries;

        public static bool TryGetScene(int sceneSlot, out DebugSceneEntry entry)
        {
            int index = sceneSlot - FirstSceneSlot;
            if (index >= 0 && index < SceneEntries.Length)
            {
                entry = SceneEntries[index];
                return true;
            }

            entry = default;
            return false;
        }

        private static DebugSceneEntry Scene(string sceneName)
        {
            return new DebugSceneEntry(sceneName, sceneName);
        }

        private static DebugSceneEntry Scene(string displayName, string sceneName)
        {
            return new DebugSceneEntry(displayName, sceneName);
        }

        private static DebugSceneEntry Reserved(string displayName)
        {
            return new DebugSceneEntry(displayName, string.Empty);
        }
    }
}
