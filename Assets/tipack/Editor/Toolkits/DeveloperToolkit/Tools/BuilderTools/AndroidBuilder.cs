using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace tiplay.Toolkit.DeveloperToolkit
{
    public class AndroidBuilder : PlatformBuilder
    {
        private static string[] buildExtensions = { "apk", "aab" };

        private PlayerSettingsDrawer playerSettingsDrawer = new AndroidPlayerSettingsDrawer();

        public override string Title => "Android Builder";
        public override BuildTarget BuildTarget => BuildTarget.Android;
        protected override BuildSettingsData GameSettingsData => GlobalData.AndroidBuildSettings;
        protected override PlayerSettingsDrawer PlayerSettingsDrawer => playerSettingsDrawer;

        private string SelectedBuildExtension => EditorUserBuildSettings.buildAppBundle ? "aab" : "apk";
        private string DefaultBuildName => $"{PlayerSettings.productName.Replace(" ", "_")}_v{PlayerSettings.bundleVersion}({PlayerSettings.Android.bundleVersionCode})";
        private string DefaultBuildPath => "~/Documents/" + DefaultBuildName;

        public override string GetBuildPath()
        {
            return EditorUtility.SaveFilePanel("Android Build Path", DefaultBuildPath, DefaultBuildName, SelectedBuildExtension);
        }

        private string GetAPKName()
        {
            return $"{PlayerSettings.applicationIdentifier}-v{PlayerSettings.bundleVersion}({PlayerSettings.Android.bundleVersionCode}).apk";
        }

        public override void OnBuildSettingsGUI()
        {
            DrawBuildExtensionField();
        }

        private void DrawBuildExtensionField()
        {
            if (buildType == PlatformBuilder.BuildType.Development)
            {
                autoconnectProfiler = EditorGUILayout.Toggle("Autoconnect Profiler", autoconnectProfiler);
                deepProfiling = EditorGUILayout.Toggle("Deep Profiling", deepProfiling);
                scriptDebugging = EditorGUILayout.Toggle("Script Debugging", scriptDebugging);
            }
            
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                int selectionIndex = Array.IndexOf(buildExtensions, EditorUserBuildSettings.buildAppBundle ? "aab" : "apk");
                selectionIndex = EditorGUILayout.Popup("Build Extension", selectionIndex, buildExtensions);

                if (!scope.changed) return;

                EditorUserBuildSettings.buildAppBundle = (buildExtensions[selectionIndex] == "aab");
            }
        }
    }
}


