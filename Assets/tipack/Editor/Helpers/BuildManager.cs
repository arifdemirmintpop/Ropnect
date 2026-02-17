using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

namespace tiplay
{
    public class BuildManager
    {
        public Action onBuildAwake;
        public Action onBuildComplete;
        public Action onBuildFail;

        public void Build(string buildPath, BuildTarget buildTarget)
        {
            var buildOptions = CreateBuildOptions(buildPath, buildTarget);
            Build(buildOptions);
        }

        public void DevelopmentBuild(string buildPath, BuildTarget buildTarget, bool autoconnectProfiler = false, bool deepProfiling = false, bool scriptDebugging = false)
        {
            var buildOptions = CreateDevelopmentBuildOptions(buildPath, buildTarget, autoconnectProfiler, deepProfiling, scriptDebugging);
            Build(buildOptions);
        }

        private BuildPlayerOptions CreateBuildOptions(string buildPath, BuildTarget buildTarget)
        {
            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetScenesFromBuildSettings();
            buildPlayerOptions.locationPathName = buildPath;
            buildPlayerOptions.target = buildTarget;
            buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;
            return buildPlayerOptions;
        }

        private BuildPlayerOptions CreateDevelopmentBuildOptions(string buildPath, BuildTarget buildTarget, bool autoconnectProfiler, bool deepProfiling, bool scriptDebugging)
        {
            var buildPlayerOptions = CreateBuildOptions(buildPath, buildTarget);
            buildPlayerOptions.options |= BuildOptions.ShowBuiltPlayer;
            buildPlayerOptions.options |= BuildOptions.Development;
            if (autoconnectProfiler)
                buildPlayerOptions.options |= BuildOptions.ConnectWithProfiler;
            if (deepProfiling)
                buildPlayerOptions.options |= BuildOptions.EnableDeepProfilingSupport;
            if (scriptDebugging)
                buildPlayerOptions.options |= BuildOptions.AllowDebugging;
            return buildPlayerOptions;
        }

        private void Build(BuildPlayerOptions buildOptions)
        {
            if (EditorApplication.isCompiling)
            {
                Debug.LogError("Can't build during compile!");
                return;
            }

            if (string.IsNullOrWhiteSpace(buildOptions.locationPathName))
            {
                EditorUtility.DisplayDialog("Build Error", "Build path can't be empty", "Done");
                return;
            }

            if (!DirectoryIsEmpty(buildOptions.locationPathName))
            {
                EditorUtility.DisplayDialog("Build Error", "Build path is not empty", "Done");
                return;
            }

            ClearPath(buildOptions.locationPathName);

            onBuildAwake?.Invoke();
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            OnBuildComplete(report);
        }

        private static bool DirectoryIsEmpty(string path)
        {
            var info = new DirectoryInfo(path);

            if (info.GetDirectories().Length > 0) return false;
            if (info.GetFiles().Length > 0) return false;

            return true;
        }

        private static void ClearPath(string path)
        {
            if (File.Exists(path))
                File.Delete(path);

            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        private void OnBuildComplete(BuildReport report)
        {
            if (report.summary.result == BuildResult.Succeeded)
            {
                onBuildComplete?.Invoke();
                Debug.Log("Build Complete");
            }

            if (report.summary.result == BuildResult.Failed)
            {
                onBuildFail?.Invoke();
                Debug.LogError("Build Failed");
            }
        }

        private string[] GetScenesFromBuildSettings()
        {
            return EditorBuildSettings.scenes
                .Where(scene => AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path) != null && scene.enabled)
                .Select(scene => scene.path).ToArray();
        }
    }
}


