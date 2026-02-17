using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using tiplay.ScriptingDefines;
using System.IO;
using System;
using tiplay.SDKManagement;

public static class ElephantGameKitCoreUtility
{
    public static Action onElephantCoreInstalled;
    public static Action onElephantCoreRemoved;
    public static Action onElephantInstalled;
    public static Action<string> installDependentSDK;
    public static Action<SDKManager[]> sdkManagersCreated;

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

public static class ElephantGameKitEditorUtility
{
    public static string ElephantSceneFullPath => Path.Combine(Application.dataPath, "Elephant/elephant_scene.unity");
    public static string ElephantScenePath => "Assets/Elephant/elephant_scene.unity";
    public static bool IsElephantInstalled => File.Exists(ElephantSceneFullPath);
    public static bool IsElephantEnabled => ElephantScriptingDefineSymbol.IsDefined && ElephantGameKitCoreUtility.IsInstalled;

    public static string FacebookSettingsFullPath => Path.Combine(Application.dataPath, "FacebookSDK/Plugins/Settings/Facebook.Unity.Settings.dll");
    public static bool IsFacebookInstalled => File.Exists(FacebookSettingsFullPath);
    public static bool IsFacebookEnabled => FacebookSDKScriptingDefineSymbol.IsDefined && ElephantGameKitCoreUtility.IsInstalled;
    public static bool IsFacebookReadyForOverride = false;
    public static SDKManager facebookSDK;

    public static string ExternalDependencyFullPath => Path.Combine(Application.dataPath, "ExternalDependencyManager/Editor/1.2.174/Google.IOSResolver.dll");
    public static bool IsExternalDependencyInstalled => File.Exists(ExternalDependencyFullPath);
    public static bool IsExternalDependencyEnabled => ExternalDependencyScriptingDefineSymbol.IsDefined && ElephantGameKitCoreUtility.IsInstalled;
    public static bool IsExternalDependencyReadyForOverride = false;
    public static SDKManager externalDependecy;

    [InitializeOnLoadMethod]
    private static void InitializeElephant()
    {
        ElephantGameKitCoreUtility.onElephantCoreRemoved -= OnElephantCoreRemoved;
        ElephantGameKitCoreUtility.onElephantCoreRemoved += OnElephantCoreRemoved;

        if (ElephantGameKitCoreUtility.IsInstalled && IsElephantInstalled && !ElephantScriptingDefineSymbol.IsDefined)
            ElephantScriptingDefineSymbol.Define();
    }

    [InitializeOnLoadMethod]
    private static void InitializeElephantScene()
    {
        if (!IsElephantInstalled) return;

#if TIPLAY_ENABLE_SDK && TIPLAY_ELEPHANT
        ElephantGameKitEditorUtility.SetActiveElephantSceneOnBuildSettings(true);
#else
        ElephantGameKitEditorUtility.SetActiveElephantSceneOnBuildSettings(false);
#endif
    }

    [InitializeOnLoadMethod]
    private static void ListenImportPackageCompleted()
    {
        AssetDatabase.importPackageCompleted -= CheckElephantImported;
        AssetDatabase.importPackageCompleted += CheckElephantImported;
        ElephantGameKitCoreUtility.sdkManagersCreated -= GetSDKManagers;
        ElephantGameKitCoreUtility.sdkManagersCreated += GetSDKManagers;
    }

    private static void CheckElephantImported(string packageName)
    {
        if (ElephantGameKitCoreUtility.IsInstalled && IsElephantInstalled && !ElephantScriptingDefineSymbol.IsDefined)
            ElephantScriptingDefineSymbol.Define();

        if (ElephantGameKitCoreUtility.IsInstalled && IsFacebookInstalled && !FacebookSDKScriptingDefineSymbol.IsDefined)
        {
            if (facebookSDK == null && !IsFacebookReadyForOverride)
                IsFacebookReadyForOverride = true;
            if (facebookSDK != null)
            {
                IsFacebookReadyForOverride = false;
                facebookSDK.InstallDueDependicies();
            }
        }

        if (ElephantGameKitCoreUtility.IsInstalled && IsExternalDependencyInstalled && !ExternalDependencyScriptingDefineSymbol.IsDefined)
        {
            if (externalDependecy == null && !IsExternalDependencyReadyForOverride)
                IsExternalDependencyReadyForOverride = true;
            if (externalDependecy != null)
            {
                IsExternalDependencyReadyForOverride = false;
                externalDependecy.InstallDueDependicies();
            }
        }
    }

    private static void GetSDKManagers(SDKManager[] _sdkManagers)
    {
        for (int i = 0; i < _sdkManagers.Length; i++)
        {
            if (_sdkManagers[i] != null && _sdkManagers[i].SelectedVersionData != null && !string.IsNullOrEmpty(_sdkManagers[i].SelectedVersionData.Title))
            {
                if(_sdkManagers[i].SelectedVersionData.Title == "Facebook")
                {
                    facebookSDK = _sdkManagers[i];
                    if (facebookSDK != null && IsFacebookReadyForOverride)
                    {
                        IsFacebookReadyForOverride = false;
                        facebookSDK.InstallDueDependicies();
                    }
                }
                else if (_sdkManagers[i].SelectedVersionData.Title == "External Dependency")
                {
                    externalDependecy = _sdkManagers[i];
                    if (externalDependecy != null && IsExternalDependencyReadyForOverride)
                    {
                        IsExternalDependencyReadyForOverride = false;
                        externalDependecy.InstallDueDependicies();
                    }
                }
            }
        }
    }

    private static void OnElephantCoreRemoved()
    {
        ElephantScriptingDefineSymbol.Remove();
        facebookSDK.UninstallDueDependicies();
        externalDependecy.UninstallDueDependicies();
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