namespace DebugTools
{
    public readonly struct DebugSceneEntry
    {
        public readonly string DisplayName;
        public readonly string SceneName;

        public bool IsReserved => string.IsNullOrWhiteSpace(SceneName);

        public DebugSceneEntry(string displayName, string sceneName)
        {
            DisplayName = displayName;
            SceneName = sceneName;
        }
    }
}
