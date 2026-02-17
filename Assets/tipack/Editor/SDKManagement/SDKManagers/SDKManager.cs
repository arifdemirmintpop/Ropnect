using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEditor;
using tiplay.Downloader;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.Windows;

namespace tiplay.SDKManagement
{
    public abstract class SDKManager
    {
        private SDKVersionData selectedVersionData;
        private SDKData sdkData;
        private string[] packageVersionArray;

        public abstract string Label { get; }
        public abstract SDKStatusData StatusData { get; }
        public abstract bool HasSettings { get; }
        public abstract bool IndependentSDK { get; }

        public virtual void OnGUI() { }
        public virtual void OpenSettings() { }
        public virtual void OnPackageRemoved() { }
        public virtual void OnPackageInstalled() { }

        public bool HasAnyInstallableVersion => sdkData.Versions.Length > 0;
        public bool IsInstalled => StatusData.IsInstalled;

        public SDKVersionData SelectedVersionData => FindSelectedSDKVersion();
        public SDKManager(SDKData sdkData)
        {
            this.sdkData = sdkData;

            CheckPackageInstall();
            InitializeVersion();
        }

        private void InitializeVersion()
        {
            if (IsInstalled)
            {
                selectedVersionData = StatusData.installedVersion;
                packageVersionArray = new string[] { selectedVersionData.Version };
                return;
            }

            if (HasAnyInstallableVersion)
            {
                selectedVersionData = sdkData.Versions[0];
                packageVersionArray = sdkData.Versions.Select(version => version.Version).ToArray();
            }
        }

        public void DrawField()
        {
            if (!IsInstalled && !HasAnyInstallableVersion)
                return;

            if (!IndependentSDK)
                return;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label(Label, EditorStyles.boldLabel);


                using (new EditorGUILayout.HorizontalScope())
                {
                    DrawVersionField();

                    if (IsInstalled && HasSettings && GUILayout.Button("Settings", GUILayout.Width(70)))
                        OpenSettings();

                    if (!IsInstalled && GUILayout.Button("Install", GUILayout.Width(70)))
                        InstallPackage();

                    if (IsInstalled && GUILayout.Button("Uninstall", GUILayout.Width(70)))
                        UninstallPackage();
                }

                OnGUI();
            }
        }

        private SDKVersionData DrawVersionField()
        {
            var guiEnabled = GUI.enabled;

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Version", GUILayout.Width(50));

                if (IsInstalled)
                    GUI.enabled = false;

                int selectionIndex = Array.IndexOf(sdkData.Versions, selectedVersionData);
                selectionIndex = Mathf.Clamp(selectionIndex, 0, sdkData.Versions.Length - 1);
                selectionIndex = EditorGUILayout.Popup(selectionIndex, packageVersionArray);

                GUI.enabled = guiEnabled;
                return sdkData.Versions[selectionIndex];
            }
        }

        private void InstallPackage()
        {
            if (!ProjectSDKData.GetInstance().IsInstallable(selectedVersionData))
            {
                DisplayInstallDependenciesError(selectedVersionData);
                return;
            }

            SDKVersionDownloader downloader = new SDKVersionDownloader(selectedVersionData);
            downloader.downloadComplete += OnPackageDownloaded;
            downloader.Download();
        }

        private void DisplayInstallDependenciesError(SDKVersionData versionData)
        {
            string message = "You should install the dependencies.\n";

            foreach (var dependency in versionData.Dependencies)
                message += "\n" + dependency.ToUpper()[0] + dependency.Substring(1);

            EditorUtility.DisplayDialog("Warning", message, "Done");
        }

        void OnPackageDownloaded(SDKVersionData versionData, string downloadPath)
        {
            SDKImportDatabase.AddDatabase(versionData);
            AssetDatabase.ImportPackage(downloadPath, false);
        }

        private void UninstallPackage()
        {
            bool hasDependants = ProjectSDKData.GetInstance().HasDependants(StatusData);
            if (hasDependants) DisplayUninstallDependenciesError(StatusData);
            if (hasDependants) return;

            bool directoriesDeleted = RemovePackageDirectories();
            if (!directoriesDeleted) return;

            StatusData.installedVersion = null;
            SaveStatusData();
            OnPackageRemoved();
        }

        private bool RemovePackageDirectories()
        {
            foreach (var directory in StatusData.installedVersion.UninstallationDirectories)
            {
                if (!Directory.Exists(Application.dataPath + "/" + directory))
                    continue;

                if (!AssetDatabase.MoveAssetToTrash("Assets/" + directory))
                    return false;
            }

            return true;
        }

        private void DisplayUninstallDependenciesError(SDKStatusData sdkData)
        {
            string[] dependencies = ProjectSDKData.GetInstance().GetDependants(sdkData);
            string message = "You should uninstall the dependencies.\n";

            foreach (var dependency in dependencies)
                message += "\n" + dependency.ToUpper()[0] + dependency.Substring(1);

            EditorUtility.DisplayDialog("Warning", message, "Done");
        }

        private void SaveStatusData()
        {
            EditorUtility.SetDirty(StatusData);
            AssetDatabase.SaveAssetIfDirty(StatusData);
        }

        private void CheckPackageInstall()
        {
            if (!SDKImportDatabase.TryGetSDKImportData(sdkData, out var importData))
                return;

            if (!importData.installed)
                return;

            SDKVersionData versionData = sdkData.FindByPackageName(importData.packageName);
            StatusData.installedVersion = versionData;
            SaveStatusData();

            SDKImportDatabase.Remove(importData);
            OnPackageInstalled();
        }

        public void InstallDueDependicies()
        {
            Debug.Log("Installing Due Dependency");
            SDKImportDatabase.AddDependentSDKToDatabase(selectedVersionData);
        }

        public void UninstallDueDependicies()
        {
            StatusData.installedVersion = null;
            SaveStatusData();
            OnPackageRemoved();
        }

        private SDKVersionData FindSelectedSDKVersion()
        {
            SDKVersionData _selectedVersionData = null;

            if (IsInstalled)
                _selectedVersionData = StatusData.installedVersion;

            else if (!IsInstalled && HasAnyInstallableVersion)
                _selectedVersionData = sdkData.Versions[0];

            return _selectedVersionData;
        }
    }
}