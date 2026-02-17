using System.IO;
using UnityEngine;
using UnityEditor;
using tiplay.DatabaseSystem;

namespace tiplay.Toolkit
{
    public partial class DatabaseEditor : ITool
    {
        public DatabaseType DatabaseType;
        public Database Database;
        public Database UpdaterDatabase;
        private SerializedObject serializedDatabase;
        private EditorWindow editorWindow;
        public string Title => "Database";
        public string Shortcut => string.Empty;
        private bool commandPressed;
        private bool aPressed;
        private bool cPressed;

        public DatabaseEditor(EditorWindow _editorWindow)
        {
            editorWindow = _editorWindow;
        }

        public void OnCreate() { }

        public void OnDestroy() { }

        public void OnEnable()
        {
            GetUpdaterDatabase();
            GetDatabase();
        }

        public void OnDisable() { }

        private void GetUpdaterDatabase()
        {
            if (UpdaterDatabase is null)
            {
                UpdaterDatabase = Resources.Load<Database>("DatabaseUpdater");
                EditorPrefs.SetString("UpdaterDatabasePath", AssetDatabase.GetAssetPath(UpdaterDatabase));
            }

            if (EditorPrefs.HasKey("UpdaterDatabasePath"))
                UpdaterDatabase = AssetDatabase.LoadAssetAtPath(EditorPrefs.GetString("UpdaterDatabasePath"), typeof(Database)) as Database;
        }

        private void GetDatabase()
        {
            if (Database is null)
            {
                Database = Resources.Load<Database>("Database");
                EditorPrefs.SetString("DatabasePath", AssetDatabase.GetAssetPath(Database));
            }

            if (EditorPrefs.HasKey("DatabasePath"))
                Database = AssetDatabase.LoadAssetAtPath(EditorPrefs.GetString("DatabasePath"), typeof(Database)) as Database;
        }

         private void RevealInProject()
        {
            if (!Database)
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = Database;
            }
        }

        private void RevealInFinder()
        {
            if (Database is not null)
                EditorUtility.RevealInFinder(Application.persistentDataPath + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + "Database.txt");
        }

        private void SaveAsDefaultValues()
        {
            DatabaseEditorUtility.SaveAsDefaultValues(Database);
            SaveScriptableAssets();
        }

        private void LoadDefaultValues()
        {
            DatabaseEditorUtility.LoadDefaultValues(Database);
            SaveScriptableAssets();
        }

        private void ExportRemoteJson()
        {
            DatabaseEditorUtility.ExportJson(Database);
        }

        private void SaveScriptableAssets()
        {
            EditorUtility.SetDirty(Database);
            BackupData();
            EditorUtility.SetDirty(UpdaterDatabase);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void BackupData()
        {
            UpdaterDatabase.BlockerDatabase = Database.BlockerDatabase;
            UpdaterDatabase.InventoryDatabase = Database.InventoryDatabase;
            UpdaterDatabase.LevelDatabase = Database.LevelDatabase;
            UpdaterDatabase.PreferenceDatabase = Database.PreferenceDatabase;
            UpdaterDatabase.UserEngagementDatabase = Database.UserEngagementDatabase;
            UpdaterDatabase.VersionDatabase = Database.VersionDatabase;
        }
    }
}