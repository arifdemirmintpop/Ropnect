using System.Text.RegularExpressions;
using tiplay.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace tiplay.Toolkit.DeveloperToolkit
{
    public class AndroidPlayerSettingsDrawer : PlayerSettingsDrawer
    {
        protected override string BuildNumber
        {
            get => PlayerSettings.Android.bundleVersionCode.ToString();
            set
            {
                if (int.TryParse(value, out int buildNumber))
                    PlayerSettings.Android.bundleVersionCode = buildNumber;
            }
        }
    }

    public class IOSPlayerSettingsDrawer : PlayerSettingsDrawer
    {
        protected override string BuildNumber
        {
            get => PlayerSettings.iOS.buildNumber;
            set => PlayerSettings.iOS.buildNumber = value;
        }
    }

    public abstract class PlayerSettingsDrawer
    {
        private Texture2D icon;
        private Texture2D[] icons;

        protected string CompanyName
        {
            get => PlayerSettings.companyName;
            set => PlayerSettings.companyName = value;
        }

        protected string ProductName
        {
            get => PlayerSettings.productName;
            set => PlayerSettings.productName = value;
        }

        protected string BundleVersion
        {
            get => PlayerSettings.bundleVersion;
            set => PlayerSettings.bundleVersion = value;
        }

        protected abstract string BuildNumber { get; set; }

        public void OnGUI()
        {
            DrawProductSettings();
        }

        private void DrawProductSettings()
        {
            EditorGUILayout.LabelField("Product Settings", EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    DrawIconField();

                    using (new EditorGUILayout.VerticalScope())
                    {
                        DrawCompanyNameField();
                        DrawProductNameField();
                        DrawBundleIdentifierField();
                        DrawBuildNumberField();
                        DrawVersionField();
                    }
                }
            }
        }

        private void DrawIconField()
        {
            icons = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown, IconKind.Any);
            icon = icons.Length > 0 ? icons[0] : null;

            using (var gui = new EditorGUI.ChangeCheckScope())
            {
                icon = (Texture2D)EditorGUILayout.ObjectField(icon, typeof(Texture2D), false, GUILayout.Width(100), GUILayout.Height(100));

                if (!gui.changed) return;

                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { icon });
            }
        }

        private static void DrawBundleIdentifierField()
        {
            bool gui = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.TextField("Bundle", PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS));
            GUI.enabled = gui;
        }

        private void DrawProductNameField()
        {
            using (var gui = new EditorGUI.ChangeCheckScope())
            {
                string productName = EditorGUILayout.TextField("Product Name", ProductName);

                if (!gui.changed) return;

                ProductName = productName;
                UpdateApplicationIdentifier();
            }
        }

        private void DrawCompanyNameField()
        {
            using (var gui = new EditorGUI.ChangeCheckScope())
            {
                string companyName = EditorGUILayout.TextField("Company Name", CompanyName);

                if (!gui.changed) return;

                CompanyName = companyName;
                UpdateApplicationIdentifier();
            }
        }

        private void DrawBuildNumberField()
        {
            using (var gui = new EditorGUI.ChangeCheckScope())
            {
                string buildNumber = EditorGUILayout.TextField("Build Number", BuildNumber);

                if (!gui.changed) return;

                BuildNumber = buildNumber;
            }
        }

        private void DrawVersionField()
        {
            using (var gui = new EditorGUI.ChangeCheckScope())
            {
                string version = EditorGUILayout.TextField("Version", BundleVersion);

                if (!gui.changed) return;

                PlayerSettings.bundleVersion = version;
            }
        }

        protected void UpdateApplicationIdentifier()
        {
            PlayerSettingsUtility.SetApplicationIdentifier(BuildTargetGroup.iOS, CompanyName, ProductName);
            PlayerSettingsUtility.SetApplicationIdentifier(BuildTargetGroup.Android, CompanyName, ProductName);
        }
    }
}


