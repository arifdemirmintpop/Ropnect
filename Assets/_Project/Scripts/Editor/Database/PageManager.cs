
using UnityEngine;
using UnityEditor;
using tiplay.DatabaseSystem;

namespace tiplay.Toolkit
{
    public partial class DatabaseEditor : ITool
    {
        public void OnGUI()
        {
            HandleShortcuts();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Database", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            TopMenuFirstLine();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            TopMenuSecondLine();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            TopMenuThirdLine();
            GUILayout.EndHorizontal();
            ShowData();
        }

        private void HandleShortcuts()
        {
            Event e = Event.current;

            if (e.type == EventType.KeyDown)
            {
                if (e.command) commandPressed = true;
                if (e.keyCode == KeyCode.A && commandPressed) aPressed = true;
                if (e.keyCode == KeyCode.C && commandPressed) cPressed = true;

                if (commandPressed && aPressed && cPressed)
                {
                    DatabaseType = DatabaseType.LevelDatabase;
                    e.Use();
                }
            }

            if (e.type == EventType.KeyUp)
            {
                if (e.keyCode == KeyCode.C) cPressed = false;
                if (e.keyCode == KeyCode.A) aPressed = false;
                if (e.command || e.control) commandPressed = false;
            }
        }

        private void TopMenuFirstLine()
        {
            if (Database is not null)
            {
                if (GUILayout.Button("Reveal Database"))
                    RevealInProject();
                if (GUILayout.Button("Reveal In Finder"))
                    RevealInFinder();
            }
            else if (Database is null)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Database Asset is null!", EditorStyles.boldLabel);
                GUILayout.Label("Please generate database at 'Assets/Resources/' path.", EditorStyles.label);
                GUILayout.EndVertical();
            }
        }

        private void TopMenuSecondLine()
        {
            if (Database is not null)
            {
                if (GUILayout.Button("Save As Default Values"))
                    SaveAsDefaultValues();
                if (GUILayout.Button("Load Default Values"))
                    LoadDefaultValues();
            }
            else if (Database is null)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Database Asset is null!", EditorStyles.boldLabel);
                GUILayout.Label("Please generate database at 'Assets/Resources/' path.", EditorStyles.label);
                GUILayout.EndVertical();
            }
        }

        private void TopMenuThirdLine()
        {
            if (Database is not null)
            {
                if (GUILayout.Button("Export Remote JSON"))
                    ExportRemoteJson();
            }
            else if (Database is null)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Database Asset is null!", EditorStyles.boldLabel);
                GUILayout.Label("Please generate database at 'Assets/Resources/' path.", EditorStyles.label);
                GUILayout.EndVertical();
            }
        }

        private bool IsDatabaseExist(string type)
        {
            GUILayout.Space(30f);
            if (Database is null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.Label(type + " is not found!", EditorStyles.boldLabel);
                GUILayout.EndHorizontal();
                return false;
            }
            else return true;
        }

        private void ShowData()
        {
            GUILayout.Space(30);
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            DatabaseType = (DatabaseType)EditorGUILayout.EnumPopup("Database Type", DatabaseType);
            EditorGUI.EndChangeCheck();
            GUILayout.Space(80);
            GUILayout.EndHorizontal();
            if (DatabaseType == DatabaseType.BlockerDatabase)
                BlockerDatabasePanel();
            else if (DatabaseType == DatabaseType.InventoryDatabase)
                InventoryDatabasePanel();
            else if (DatabaseType == DatabaseType.LevelDatabase)
                LevelDatabasePanel();
            else if (DatabaseType == DatabaseType.PreferenceDatabase)
                PreferenceDatabasePanel();
            else if (DatabaseType == DatabaseType.UserEngagementDatabase)
                UserEngagementDatabasePanel();
            else if (DatabaseType == DatabaseType.VersionDatabase)
                VersionDatabasePanel();
        }
    }
}