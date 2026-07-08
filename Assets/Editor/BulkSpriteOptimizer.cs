using System.Linq;
using UnityEditor;
using UnityEngine;

public class BulkSpriteOptimizerWindow : EditorWindow
{
    private const int TargetMaxSize = 4096;
    private const int AssetsPerTick = 1;

    private static readonly string KeyPrefix = "BulkSpriteOptimizer_" + Application.dataPath.GetHashCode() + "_";
    private static readonly string KeyPaths = KeyPrefix + "Paths";
    private static readonly string KeyIndex = KeyPrefix + "Index";
    private static readonly string KeyTotal = KeyPrefix + "Total";
    private static readonly string KeyDone = KeyPrefix + "Done";
    private static readonly string KeySkipped = KeyPrefix + "Skipped";
    private static readonly string KeyFinished = KeyPrefix + "Finished";

    private static bool _running;
    private static double _accumulatedSeconds;
    private static int _sessionProcessedCount;
    private static double _lastTickTime;

    [MenuItem("Tools/Bulk Sprite Optimizer")]
    private static void Open()
    {
        GetWindow<BulkSpriteOptimizerWindow>("Sprite Optimizer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Bulk Sprite Optimizer", EditorStyles.boldLabel);
        GUILayout.Label($"Target Max Size: {TargetMaxSize}, Crunch: Off");

        EditorGUILayout.Space();

        int total = EditorPrefs.GetInt(KeyTotal, 0);
        int index = EditorPrefs.GetInt(KeyIndex, 0);
        int done = EditorPrefs.GetInt(KeyDone, 0);
        int skipped = EditorPrefs.GetInt(KeySkipped, 0);
        bool finished = EditorPrefs.GetBool(KeyFinished, false);

        float progress = total > 0 ? (float)index / total : 0f;
        Rect rect = EditorGUILayout.GetControlRect(false, 20);
        EditorGUI.ProgressBar(rect, progress, $"{index}/{total}");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Reimported:", done.ToString());
        EditorGUILayout.LabelField("Skipped (already optimal):", skipped.ToString());
        EditorGUILayout.LabelField("Remaining:", (total - index).ToString());
        EditorGUILayout.LabelField("Finished:", finished.ToString());
        EditorGUILayout.LabelField("Estimated time left:", GetEtaLabel(index, total));

        EditorGUILayout.Space();
        GUI.enabled = !_running;
        if (GUILayout.Button(finished ? "Start Fresh Pass" : "Start / Resume"))
        {
            Start();
        }
        GUI.enabled = _running;
        if (GUILayout.Button("Stop"))
        {
            Stop();
        }
        GUI.enabled = true;
    }

    private void Update()
    {
        if (_running) Repaint();
    }

    private static string GetEtaLabel(int index, int total)
    {
        if (!_running || _sessionProcessedCount <= 0)
        {
            return _running ? "calculating..." : "-";
        }

        double secondsPerAsset = _accumulatedSeconds / _sessionProcessedCount;
        double etaSeconds = secondsPerAsset * (total - index);
        var eta = System.TimeSpan.FromSeconds(etaSeconds);
        return $"{(int)eta.TotalMinutes}m {eta.Seconds}s";
    }

    private static void Start()
    {
        if (_running)
        {
            Debug.LogWarning("[BulkSpriteOptimizer] Already running.");
            return;
        }

        bool finished = EditorPrefs.GetBool(KeyFinished, false);
        bool hasSavedRun = EditorPrefs.HasKey(KeyPaths);

        if (!hasSavedRun || finished)
        {
            var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets" });
            var paths = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => !p.Contains("/Editor/"))
                .OrderBy(p => p, System.StringComparer.Ordinal)
                .ToArray();

            EditorPrefs.SetString(KeyPaths, string.Join("\n", paths));
            EditorPrefs.SetInt(KeyIndex, 0);
            EditorPrefs.SetInt(KeyTotal, paths.Length);
            EditorPrefs.SetInt(KeyDone, 0);
            EditorPrefs.SetInt(KeySkipped, 0);
            EditorPrefs.SetBool(KeyFinished, false);

            Debug.Log($"[BulkSpriteOptimizer] Fresh pass. Queued {paths.Length} textures.");
        }
        else
        {
            int index = EditorPrefs.GetInt(KeyIndex, 0);
            int total = EditorPrefs.GetInt(KeyTotal, 0);
            Debug.Log($"[BulkSpriteOptimizer] Resuming at {index}/{total}.");
        }

        _running = true;
        _accumulatedSeconds = 0;
        _sessionProcessedCount = 0;
        _lastTickTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += Tick;
    }

    private static void Stop()
    {
        if (!_running)
        {
            Debug.LogWarning("[BulkSpriteOptimizer] Not running.");
            return;
        }

        EditorApplication.update -= Tick;
        _running = false;

        int index = EditorPrefs.GetInt(KeyIndex, 0);
        int total = EditorPrefs.GetInt(KeyTotal, 0);
        Debug.Log($"[BulkSpriteOptimizer] Stopped at {index}/{total}. Resume anytime with Start.");
    }

    private static void Tick()
    {
        var paths = EditorPrefs.GetString(KeyPaths, "").Split('\n');
        int index = EditorPrefs.GetInt(KeyIndex, 0);
        int total = EditorPrefs.GetInt(KeyTotal, 0);
        int done = EditorPrefs.GetInt(KeyDone, 0);
        int skipped = EditorPrefs.GetInt(KeySkipped, 0);

        int processedThisTick = 0;
        for (int i = 0; i < AssetsPerTick && index < total; i++)
        {
            bool wasReimported = ProcessOne(paths[index]);
            if (wasReimported) done++; else skipped++;
            index++;
            processedThisTick++;
        }

        double now = EditorApplication.timeSinceStartup;
        _accumulatedSeconds += now - _lastTickTime;
        _lastTickTime = now;
        _sessionProcessedCount += processedThisTick;

        EditorPrefs.SetInt(KeyIndex, index);
        EditorPrefs.SetInt(KeyDone, done);
        EditorPrefs.SetInt(KeySkipped, skipped);

        if (index >= total)
        {
            EditorApplication.update -= Tick;
            _running = false;
            EditorPrefs.SetBool(KeyFinished, true);
            Debug.Log($"[BulkSpriteOptimizer] Done. {done} reimported, {skipped} already optimal/skipped, {total} total.");
        }
    }

    private static bool ProcessOne(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null || importer.textureType != TextureImporterType.Sprite)
        {
            return false;
        }

        bool needsChange = importer.maxTextureSize > TargetMaxSize || importer.crunchedCompression;
        if (!needsChange)
        {
            return false;
        }

        importer.maxTextureSize = Mathf.Min(importer.maxTextureSize, TargetMaxSize);
        importer.crunchedCompression = false;
        importer.textureCompression = TextureImporterCompression.Compressed;
        importer.SaveAndReimport();
        return true;
    }
}