using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.IO;
using UnityEditor;
using System.Linq;
using tiplay.SDKManagement;
using tiplay.Downloader;

namespace tiplay.Toolkit.DeveloperToolkit
{
    public class SDKManagement : ITool
    {
        private SDKJsonDownloader sdkJsonDownloader;

        private SDKManager[] sdkManagers = new SDKManager[0];

        private ExternalDependencySDKManager externalDependencyField;
        private FacebookSDKManager facebookField;
        private GameAnalyticsSDKManager gameAnalyticsField;
        private ElephantSDKManager elephantField;
        private TibugSDKManager tibugField;

        string ITool.Title => "SDK Management";

        string ITool.Shortcut => string.Empty;

        void ITool.OnCreate()
        {
            sdkJsonDownloader = new SDKJsonDownloader();
            sdkJsonDownloader.downloadComplete += OnSDKJsonDownloadComplete;
            sdkJsonDownloader.Download();
        }

        private void OnSDKJsonDownloadComplete(SDKWebResponse jsonData)
        {
            SDKVersionData[] facebookSDKVersions = { new SDKVersionData("Facebook", "v11.0.0", "", new string[] { }, new string[] { }) };
            SDKVersionData[] externalDependencySDKVersions = { new SDKVersionData("External Dependency", "1.2.174", "", new string[] { }, new string[] { }) };
            sdkManagers = new SDKManager[] {
                new ElephantGameKitSDKManager(jsonData),
                new ExternalDependencySDKManager(new SDKData(externalDependencySDKVersions)),
                new FacebookSDKManager(new SDKData(facebookSDKVersions)),
                new GameAnalyticsSDKManager(jsonData),
                new ElephantSDKManager(jsonData),
                new TibugSDKManager(jsonData),
            };
            ElephantGameKitCoreUtility.sdkManagersCreated.Invoke(sdkManagers);
        }

        void ITool.OnDestroy() { }

        void ITool.OnDisable() { }

        void ITool.OnEnable() { }

        void ITool.OnGUI()
        {
            //int fontSize = EditorStyles.helpBox.fontSize;
            //EditorStyles.helpBox.fontSize = 12;
            //EditorStyles.helpBox.fontStyle = FontStyle.Bold;
            //EditorGUILayout.HelpBox("Projede el ile kurulan sdk'lar varsa bu aracı kullanmadan önce kaldırılmalı.", MessageType.Warning);
            //EditorStyles.helpBox.fontSize = fontSize;
            //EditorStyles.helpBox.fontStyle = FontStyle.Normal;

            if (EditorApplication.isCompiling)
                GUI.enabled = false;

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawSDKFields();

                if (GUILayout.Button("Refresh List", GUILayout.Width(100), GUILayout.Height(80)))
                {
                    sdkJsonDownloader.Download();
                }
            }

            GUI.enabled = true;
        }

        private void DrawSDKFields()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                for (int i = 0; i < sdkManagers.Length; i++)
                    sdkManagers[i].DrawField();
            }
        }

        public void InstallDependentSDK(string _title)
        {
            foreach (SDKManager sdkManager in sdkManagers)
            {
                if (sdkManager.SelectedVersionData.Title == _title)
                {
                    sdkManager.InstallDueDependicies();
                }
            }
        }
    }
}