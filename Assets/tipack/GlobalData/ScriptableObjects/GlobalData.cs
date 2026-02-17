using UnityEngine;
using System.Collections;
using tiplay.DatabaseSystem;
using System;
using System.Threading.Tasks;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace tiplay
{
    //[CreateAssetMenu(menuName = "Global Data")]
    public class GlobalData : ScriptableObject
    {
        public BuildSettingsData defaultBuildSettings;
        public BuildSettingsData androidBuildSettings;
        public BuildSettingsData iosBuildSettings;
        public Database database;
        public Database updaterDatabase;
        public GameObject gameAnalyticsPrefab;
        public GameObject tibugPrefab;

        private static GlobalData instance;
        public static GlobalData GetInstance()
        {
            instance ??= Resources.Load<GlobalData>(nameof(GlobalData));
            return instance;
        }

        public static Database Database => GetInstance().database;
        public static Database UpdaterDatabase => GetInstance().updaterDatabase;
        public static GameObject GameAnalyticsPrefab => GetInstance().gameAnalyticsPrefab;
        public static BuildSettingsData AndroidBuildSettings => GetInstance().androidBuildSettings;
        public static BuildSettingsData iOSBuildSettings => GetInstance().iosBuildSettings;
        public static GameObject TibugPrefab => GetInstance().tibugPrefab;

        public static BuildSettingsData GetActiveGameSettings()
        {
#if UNITY_ANDROID
            return AndroidBuildSettings;
#elif UNITY_IOS
            return iOSBuildSettings;
#else
            return GetInstance().defaultBuildSettings;
#endif
        }

#if UNITY_EDITOR
        public static void SaveData()
        {
            EditorUtility.SetDirty(GetInstance());
            AssetDatabase.SaveAssetIfDirty(GetInstance());
        }
#endif

        public static void CheckUpdateUser()
        {
            var databaseVersion = new Version(Database.VersionDatabase.Version);
            var updaterDatabaseVersion = new Version(UpdaterDatabase.VersionDatabase.Version);
            if (databaseVersion.CompareTo(updaterDatabaseVersion) < 0)
                GetInstance().PrepareForUpdateUser();
            EventManager.OnUpdateUserChecked?.Invoke();
        }

        private void PrepareForUpdateUser()
        {
            PreserveDataForUpdateUser();
            SaveManager.SaveData(Database);
            //Open this line if you want to detect updated user in gameplay.
            //Database.UserEngagementDatabase.IsUpdatedUser = true;
        }

        private async void PreserveDataForUpdateUser()
        {
            await PreserveDataProcess(Database.UserEngagementDatabase, UpdaterDatabase.UserEngagementDatabase, "User Engagement Database");
            foreach (var field in Database.GetType().GetFields())
            {
                field.SetValue(Database, field.GetValue(UpdaterDatabase));
            }
        }

        private async Task<bool> PreserveDataProcess(object source, object target, string dataset)
        {
            bool hasPreservedFields = await PreserveManager.PreserveData(source, target);
            Debug.Log("Preservation of " + dataset + "is completed.");
            return hasPreservedFields;
        }
    }
}
