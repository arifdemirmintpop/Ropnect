using UnityEngine;
using UnityEditor;

namespace tiplay
{
    public class PackageDrawer
    {
        private string packageId;
        private string label;
        private bool isInstalled;
        private bool isLatestVersionInstalled;
        private bool initialized = true;

        public PackageDrawer(string label, string packageId)
        {
            this.label = label;
            this.packageId = packageId;

            isInstalled = PackageManagerController.IsPackageInstalled(packageId);

            if (!isInstalled) return;

            initialized = false;
            PackageManagerController.IsInstalledLatestVersion(packageId, (result) =>
            {
                isLatestVersionInstalled = result;
                initialized = true;
            });
        }

        public void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(label);

                bool guiEnabled = GUI.enabled;
                string installButtonLabel = isInstalled ? "Update" : "Install";

                if (isLatestVersionInstalled)
                {
                    installButtonLabel = "Installed";
                    GUI.enabled = false;
                }

                if (!initialized)
                {
                    installButtonLabel = "Wait";
                    GUI.enabled = false;
                }

                if (GUILayout.Button(installButtonLabel, GUILayout.Width(100)))
                    PackageManagerController.InstallPackage(packageId);

                GUI.enabled = guiEnabled;
            }
        }
    }
}


