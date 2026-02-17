using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace tiplay.Toolkit.DeveloperToolkit
{
    public class IOSBuilder : PlatformBuilder
    {
        private PlayerSettingsDrawer playerSettingsDrawer = new IOSPlayerSettingsDrawer();
        protected override PlayerSettingsDrawer PlayerSettingsDrawer => playerSettingsDrawer;

        public override string Title => "IOS Builder";
        public override BuildTarget BuildTarget => BuildTarget.iOS;
        protected override BuildSettingsData GameSettingsData => GlobalData.iOSBuildSettings;

        private string DefaultBuildFolder => $"{PlayerSettings.productName.Replace(" ", "_")}_v{PlayerSettings.bundleVersion}({PlayerSettings.iOS.buildNumber})";
        private string DefaultBuildPath => "~/Documents/" + DefaultBuildFolder;

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
        }

        public override void OnPlatformGUI()
        {
            DrawPodfileProcessorSettings();
        }

        private void DrawPodfileProcessorSettings()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Podfile Processor", EditorStyles.boldLabel);
                PodfileProcessorEditorUtility.DrawProcessorSettings();
            }
        }

        public override string GetBuildPath()
        {
            return EditorUtility.SaveFolderPanel("IOS Build Folder", DefaultBuildPath, DefaultBuildFolder);
        }
    }
}


