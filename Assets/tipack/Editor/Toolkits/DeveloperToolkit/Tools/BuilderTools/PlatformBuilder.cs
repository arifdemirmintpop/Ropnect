using System;
using System.IO;
using tiplay.BuildProcessors;
using tiplay.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace tiplay.Toolkit.DeveloperToolkit
{
    public abstract class PlatformBuilder : ITool
    {
        protected enum BuildType { Default, Development };

        protected BuildType buildType = BuildType.Default;
        protected BuildManager buildManager = new BuildManager();

        public abstract string Title { get; }
        public abstract BuildTarget BuildTarget { get; }
        protected abstract PlayerSettingsDrawer PlayerSettingsDrawer { get; }
        protected abstract BuildSettingsData GameSettingsData { get; }
        protected bool autoconnectProfiler = false;
        protected bool deepProfiling = false;
        protected bool scriptDebugging = false;

        public string Shortcut => string.Empty;

        public virtual void OnDestroy() { }
        public virtual void OnCreate() { }
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnPlatformGUI() { }
        public virtual void OnBuildSettingsGUI() { }

        void ITool.OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.LabelField("Can't edit during compile!", EditorStyles.helpBox);
                GUI.enabled = false;
            }

            PlayerSettingsDrawer.OnGUI();
            BuildSettingsEditorDrawer.DrawEditorGUI(GameSettingsData);
            DrawBuildSettingFields();
            OnPlatformGUI();
            DrawBuildButton();

            GUI.enabled = true;
        }


        private void DrawBuildSettingFields()
        {
            EditorGUILayout.LabelField("Build Settings", EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GameDataBuildProcessorEditorUtility.DrawProcessorSettings();
                TibugActivationDrawer.DrawGUI();
                SDKActivationDrawer.DrawGUI();
                DrawBuildTypeField();
                OnBuildSettingsGUI();
            }
        }

        private void DrawBuildButton()
        {
            GUI.skin.button.fontStyle = FontStyle.Bold;
            GUI.skin.button.fontSize = 14;
            if (GUILayout.Button("Start Build", GUILayout.Height(30)))
                StartBuild();
            GUI.skin.button.fontSize = 12;
            GUI.skin.button.fontStyle = FontStyle.Normal;
        }

        private void DrawBuildTypeField()
        {
            buildType = (BuildType)EditorGUILayout.EnumPopup("Build Type", buildType);
        }

        protected void StartBuild()
        {
            var buildPath = GetBuildPath();

            if (buildType == BuildType.Default)
            {
                buildManager.Build(buildPath, BuildTarget);
            }
            else if (buildType == BuildType.Development)
            {
                buildManager.DevelopmentBuild(buildPath, BuildTarget, autoconnectProfiler, deepProfiling, scriptDebugging);
            }
        }

        public abstract string GetBuildPath();
    }
}


