using UnityEngine;
using System.Collections;
using UnityEditor;
using tiplay.EditorUtilities;
using System.Collections.Generic;
using tiplay.PackageManagement;
using tiplay.Downloader;
using System;
using PackageInfo = tiplay.PackageManagement.Package;
using System.Linq;

namespace tiplay.Toolkit.DeveloperToolkit
{
    public class PackageManagement : ITool
    {
        public string Title => "Package Management";

        public string Shortcut => string.Empty;

        private PackageListJsonDownloader packageListDownloader = new PackageListJsonDownloader();
        private PackageGUI[] packageGuiArray = new PackageGUI[0];
        private Vector2 packageScroll = Vector2.zero;

        public void OnCreate()
        {
            packageListDownloader.downloadComplete += OnPackageListDownloaded;
        }

        public void OnDestroy()
        {
            packageListDownloader.downloadComplete -= OnPackageListDownloaded;
        }

        private void OnPackageListDownloaded(PackageListResponse response)
        {
            packageGuiArray = response.Packages.Select(package => new PackageGUI(package)).ToArray();
        }

        public void OnEnable()
        {
            ReloadPackages();
        }

        private void ReloadPackages()
        {
            packageGuiArray = new PackageGUI[0];
            packageListDownloader.Download();
        }

        public void OnDisable()
        {

        }

        public void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.LabelField("Can't edit during compile!", EditorStyles.helpBox);
                GUI.enabled = false;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Packages", EditorStyles.boldLabel);

                if (GUILayout.Button("Refresh", GUILayout.Width(100)))
                    ReloadPackages();
            }

            DrawPackages();

            GUI.enabled = true;
        }

        private void DrawPackages()
        {
            packageScroll = EditorGUILayout.BeginScrollView(packageScroll);

            for (int i = 0; i < packageGuiArray.Length; i++)
                packageGuiArray[i].DrawField();

            EditorGUILayout.EndScrollView();
        }
    }
}


