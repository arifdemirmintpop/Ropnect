using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEditor;
using tiplay.Downloader;
using System.Collections.Generic;
using UnityEngine.Windows;

namespace tiplay.PackageManagement
{
    public class PackageGUI
    {
        private Package package;

        private bool isInstalled;

        public PackageGUI(Package package)
        {
            this.package = package;
            isInstalled = PackageDatabase.IsInstalled(package);
        }

        public void DrawField()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {

                    if (GUILayout.Button("ⓘ", GUILayout.Width(25)))
                    {
                        Application.OpenURL(package.DocumentURL);
                    }

                    //EditorGUI.BeginChangeCheck();
                    //isInstalled = EditorGUILayout.ToggleLeft(package.Title, isInstalled);
                    //if (EditorGUI.EndChangeCheck())
                    //{
                    //    if (isInstalled) PackageDatabase.AddPackage(package);
                    //    else PackageDatabase.RemovePackage(package);
                    //}


                    EditorGUILayout.LabelField(package.Title);
                    if (GUILayout.Button("↓ Download", GUILayout.Width(95)))
                    {
                        InstallPackage();
                    }
                }

                if (!string.IsNullOrEmpty(package.Description))
                {
                    GUILayout.Label(package.Description);
                }
            }
        }

        private void InstallPackage()
        {
            PackageDownloader downloader = new PackageDownloader(package);
            downloader.downloadComplete += OnPackageDownloaded;
            downloader.Download();
        }

        void OnPackageDownloaded(Package package, string downloadPath)
        {
            AssetDatabase.ImportPackage(downloadPath, true);

            //bool dialog = EditorUtility.DisplayDialog(string.Empty, "Did you complete the package installation?", "Yes", "No");
            //if (dialog)
            //{
            //    isInstalled = true;
            //    PackageDatabase.AddPackage(package);
            //}
        }
    }
}

