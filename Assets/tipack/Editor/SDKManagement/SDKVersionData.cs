using System;
using Newtonsoft.Json;
using UnityEngine;

namespace tiplay.SDKManagement
{
    [Serializable]
    public class SDKVersionData
    {
        [SerializeField] private string title;
        [SerializeField] private string version;
        [SerializeField] private string downloadUrl;
        [SerializeField] private string[] uninstallationDirectories;
        [SerializeField] private string[] dependencies;

        public string Title => title;
        public string Version => version;
        public string DownloadURL => downloadUrl;
        public string PackageName => title + " - " + version;
        public string[] UninstallationDirectories => uninstallationDirectories;
        public string[] Dependencies => dependencies;

        public SDKVersionData(string _title, string _version, string _downloadUrl, string[] _uninstallationDirectories, string[] _dependencies)
        {
            title = _title;
            version = _version;
            downloadUrl = _downloadUrl;
            uninstallationDirectories = _uninstallationDirectories;
            dependencies = _dependencies;
        }
    }
}