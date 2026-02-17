using UnityEngine;
using System.Collections;
using UnityEditor;
using tiplay.EditorUtilities;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;

namespace tiplay.Toolkit.DeveloperToolkit
{
    public class ProjectInitializer : ITool
    {
        private enum TargetPlatform { Android, IOS };
        private enum TargetColorSpace { Linear, Gamma };

        private string productName;
        private string companyName;
        private TargetPlatform targetPlatform;
        private UIOrientation orientation;
        private TargetColorSpace colorSpace;
        private EditorWindow window;

        private PackageDrawer[] packageDrawers = new PackageDrawer[] {
            new PackageDrawer("Visual Studio", "com.unity.ide.visualstudio"),
            new PackageDrawer("Visual Studio Code", "com.unity.ide.vscode"),
            new PackageDrawer("Jetbrains Rider", "com.unity.ide.rider"),
            new PackageDrawer("2D Sprite", "com.unity.2d.sprite"),
            new PackageDrawer("Cinemachine", "com.unity.cinemachine"),
            new PackageDrawer("Post Processing", "com.unity.postprocessing"),
            new PackageDrawer("Unity Recorder", "com.unity.recorder")
        };

        public string Title => "Project Initializer";

        public string Shortcut => string.Empty;

        public ProjectInitializer(EditorWindow window)
        {
            this.window = window;
        }

        public void OnDestroy()
        {
        }

        public void OnCreate()
        {

        }

        public void OnEnable()
        {
            orientation = UIOrientation.Portrait;
            companyName = "tiplaystudio";
            productName = PlayerSettings.productName;
            colorSpace = TargetColorSpace.Linear;
            targetPlatform = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ? TargetPlatform.Android : TargetPlatform.IOS;
            EditorApplication.update += window.Repaint;
        }

        public void OnDisable()
        {
            EditorApplication.update -= window.Repaint;
        }

        public void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.LabelField("Can't edit during compile!", EditorStyles.helpBox);
                GUI.enabled = false;
            }

            EditorGUILayout.LabelField("Project Settings", EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                companyName = EditorGUILayout.TextField("Company Name", companyName);
                productName = EditorGUILayout.TextField("Product Name", productName);
                orientation = (UIOrientation)EditorGUILayout.EnumPopup("Orientation", orientation);
                colorSpace = (TargetColorSpace)EditorGUILayout.EnumPopup("Color Space", colorSpace);
                targetPlatform = (TargetPlatform)EditorGUILayout.EnumPopup("Platform", targetPlatform);
            }

            if (GUILayout.Button("Initialize"))
            {
                InitializeOnClick();
            }

            DrawPackages();

            GUI.enabled = true;
        }

        private void DrawPackages()
        {
            EditorGUILayout.LabelField("Packages", EditorStyles.boldLabel);

            for (int i = 0; i < packageDrawers.Length; i++)
                packageDrawers[i].OnGUI();
        }

        private void InitializeOnClick()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(GetTargetGroup(), GetTarget());
            PlayerSettings.companyName = companyName;
            PlayerSettings.defaultInterfaceOrientation = orientation;
            PlayerSettings.productName = productName;

            PlayerSettingsUtility.SetApplicationIdentifier(BuildTargetGroup.iOS, companyName, productName);
            PlayerSettingsUtility.SetApplicationIdentifier(BuildTargetGroup.Android, companyName, productName);

            PlayerSettings.colorSpace = GetColorSpace();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private BuildTargetGroup GetTargetGroup()
        {
            if (targetPlatform == TargetPlatform.Android)
                return BuildTargetGroup.Android;

            return BuildTargetGroup.iOS;
        }

        private BuildTarget GetTarget()
        {
            if (targetPlatform == TargetPlatform.Android)
                return BuildTarget.Android;

            return BuildTarget.iOS;
        }

        private ColorSpace GetColorSpace()
        {
            if (colorSpace == TargetColorSpace.Gamma)
                return ColorSpace.Gamma;

            return ColorSpace.Linear;
        }
    }
}


