using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;
using Newtonsoft.Json;

// SDK paketlerini importlarken importun bitip bitmediğini bu sınıf üzerinden kontrol ediyorum
// Unity'nin kendi callbackleri mevcut fakat paket importladıktan sonra recompile ettiği için sanırım dinlemeyi bırakıyordu
namespace tiplay.SDKManagement
{
    public static class SDKImportDatabase
    {
        [Serializable]
        private class ImportDatabase
        {
            public List<SDKImportData> sdkDatas = new List<SDKImportData>();
        }

        [Serializable]
        public class SDKImportData
        {
            public string packageName;
            public bool installed;

            public SDKImportData(string packageName)
            {
                this.packageName = packageName;
            }
        }

        private static ImportDatabase database;

        private static void LoadDatabase()
        {
            string defaultJson = EditorJsonUtility.ToJson(new ImportDatabase());
            string jsonData = EditorPrefs.GetString("PackageImportData_" + Application.identifier, defaultJson);
            database = JsonConvert.DeserializeObject<ImportDatabase>(jsonData);
        }

        private static void SaveDatabase()
        {
            string jsonData = EditorJsonUtility.ToJson(database);
            EditorPrefs.SetString("PackageImportData_" + Application.identifier, jsonData);
        }

        [InitializeOnLoadMethod]
        static void CheckImportedPackageStatus()
        {
            LoadDatabase();
            AssetDatabase.importPackageCompleted += AssetDatabase_importPackageCompleted;
        }

        private static void AssetDatabase_importPackageCompleted(string packageName)
        {
            if (!TryGetSDKImportData(packageName, out SDKImportData sdkImportData))
                return;

            sdkImportData.installed = true;
            SaveDatabase();
        }

        public static void AddDatabase(SDKVersionData version)
        {
            database.sdkDatas.Add(new SDKImportData(version.PackageName));
            SaveDatabase();
        }

        public static void AddDependentSDKToDatabase(SDKVersionData version)
        {
            database.sdkDatas.Add(new SDKImportData(version.PackageName));
            SaveDatabase();
            AssetDatabase_importPackageCompleted(version.PackageName);
        }

        public static void Remove(SDKImportData sdkImportData)
        {
            database.sdkDatas.Remove(sdkImportData);
            SaveDatabase();
        }

        public static bool TryGetSDKImportData(string packageName, out SDKImportData sdkImportData)
        {
            foreach (var sdkData in database.sdkDatas)
            {
                if (sdkData.packageName == packageName)
                {
                    sdkImportData = sdkData;
                    return true;
                }
            }

            sdkImportData = null;
            return false;
        }

        public static bool TryGetSDKImportData(SDKData sdk, out SDKImportData sdkImportData)
        {
            foreach (var sdkVersion in sdk.Versions)
            {
                if (TryGetSDKImportData(sdkVersion.PackageName, out sdkImportData))
                {
                    return true;
                }
            }

            sdkImportData = null;
            return false;
        }
    }
}

