using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using tiplay.ScriptingDefines;
using System.IO;
using System;

public static class ElephantCoreUtility
{
    public static Action onElephantCoreInstalled;
    public static Action onElephantCoreRemoved;

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        onElephantCoreInstalled -= ElephantCoreScriptingDefineSymbol.Define;
        onElephantCoreInstalled += ElephantCoreScriptingDefineSymbol.Define;

        onElephantCoreRemoved -= ElephantCoreScriptingDefineSymbol.Remove;
        onElephantCoreRemoved += ElephantCoreScriptingDefineSymbol.Remove;
    }

    public static bool IsInstalled => ElephantCoreScriptingDefineSymbol.IsDefined;
}

public static class ElephantEditorUtility
{
    public static string ElephantSceneFullPath => Path.Combine(Application.dataPath, "Elephant/elephant_scene.unity");
    public static string ElephantScenePath => "Assets/Elephant/elephant_scene.unity";
    public static bool IsElephantInstalled => File.Exists(ElephantSceneFullPath);
    public static bool IsElephantEnabled => ElephantScriptingDefineSymbol.IsDefined && ElephantCoreUtility.IsInstalled;

    [InitializeOnLoadMethod]
    private static void InitializeElephant()
    {
        ElephantCoreUtility.onElephantCoreRemoved -= OnElephantCoreRemoved;
        ElephantCoreUtility.onElephantCoreRemoved += OnElephantCoreRemoved;

        if (ElephantCoreUtility.IsInstalled && IsElephantInstalled && !ElephantScriptingDefineSymbol.IsDefined)
            ElephantScriptingDefineSymbol.Define();
    }

    [InitializeOnLoadMethod]
    private static void InitializeElephantScene()
    {
        if (!IsElephantInstalled) return;

#if TIPLAY_ENABLE_SDK && TIPLAY_ELEPHANT
        ElephantEditorUtility.SetActiveElephantSceneOnBuildSettings(true);
#else
        ElephantEditorUtility.SetActiveElephantSceneOnBuildSettings(false);
#endif
    }

    private static void OnElephantCoreRemoved()
    {
        ElephantScriptingDefineSymbol.Remove();
        RemoveElephantSceneFromBuildSettings();
    }

    public static bool HasElephantScene()
    {
        return AssetDatabase.LoadAssetAtPath<SceneAsset>(ElephantScenePath);
    }

    public static void SetActiveElephantSceneOnBuildSettings(bool enabled)
    {
        AddElephantSceneToBuildSettings();

        if (EditorBuildSettings.scenes.Length == 0)
            return;

        if (EditorBuildSettings.scenes[0].path != ElephantScenePath)
            return;

        var scenes = EditorBuildSettings.scenes;
        scenes[0].enabled = enabled;
        EditorBuildSettings.scenes = scenes;
    }

    public static void AddElephantSceneToBuildSettings()
    {
        if (!HasElephantScene())
            return;

        var buildScenes = EditorBuildSettings.scenes.ToList();

        if (buildScenes.Count > 0)
        {
            bool isElephantSceneAlreadyAdded = (buildScenes[0].path == ElephantScenePath);

            if (isElephantSceneAlreadyAdded)
                return;
        }

        EditorBuildSettingsScene elephantBuildScene = new EditorBuildSettingsScene(ElephantScenePath, true);
        buildScenes.Insert(0, elephantBuildScene);
        EditorBuildSettings.scenes = buildScenes.ToArray();
    }

    public static void EditSettings()
    {
#if TIPLAY_ELEPHANT
        ElephantSDK.ElephantEditor.Settings();
#endif
    }

    public static void RemoveElephantSceneFromBuildSettings()
    {
        var buildScenes = EditorBuildSettings.scenes.ToList();

        if (buildScenes.Count == 0)
            return;

        bool hasElephantSceneOnBuildScenes = (buildScenes[0].path == ElephantScenePath);

        if (!hasElephantSceneOnBuildScenes)
            return;

        buildScenes.RemoveAt(0);
        EditorBuildSettings.scenes = buildScenes.ToArray();
    }
}

