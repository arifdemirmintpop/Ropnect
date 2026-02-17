using UnityEngine;

namespace tiplay.SDKManagement
{
    public class SDKStatusData : ScriptableObject
    {
        public SDKVersionData installedVersion = null;

        private bool isInstalled;

        public bool IsInstalled { get { return CheckInstallation(); }}//=> installedVersion != null && !string.IsNullOrEmpty(installedVersion.DownloadURL);

        private bool CheckInstallation()
        {
            if (installedVersion.Title == "Facebook" || installedVersion.Title == "External Dependency")
                isInstalled = installedVersion != null ? true : false;
            else if (installedVersion.Title != "Facebook" && installedVersion.Title != "External Dependency")
                isInstalled = ((installedVersion != null) && (!string.IsNullOrEmpty(installedVersion.DownloadURL)));
            return isInstalled;
        }
    }
}