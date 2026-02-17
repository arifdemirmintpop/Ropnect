using UnityEngine;
using UnityEditor;

namespace tiplay.Toolkit
{
    public partial class DatabaseEditor : ITool
    {
        private string installTimestampTextValue;
        private string totalPlaytimeTextValue;
        private void UserEngagementDatabasePanel()
        {
            if (IsDatabaseExist("User Engagement Database"))
            {
                EditorGUILayout.HelpBox("This variables cannot be edited.", MessageType.Warning);
                GUILayout.Space(30);
                installTimestampTextValue = Database.UserEngagementDatabase.InstallTimestamp.ToString();
                totalPlaytimeTextValue = Database.UserEngagementDatabase.TotalPlaytime.ToString();

                GUI.enabled = false;
                Database.UserEngagementDatabase.IsUpdatedUser = EditorGUILayout.Toggle("Is Updated User", Database.UserEngagementDatabase.IsUpdatedUser);
                installTimestampTextValue = EditorGUILayout.TextField("Install Timestamp ", installTimestampTextValue);
                totalPlaytimeTextValue = EditorGUILayout.TextField("Total Playtime ", totalPlaytimeTextValue);
                GUI.enabled = true;
                GUILayout.Space(30);
                if (GUILayout.Button("Reset Engagement Database"))
                {
                    Database.UserEngagementDatabase.IsUpdatedUser = false;
                    Database.UserEngagementDatabase.InstallTimestamp = 0;
                    Database.UserEngagementDatabase.TotalPlaytime = 0;
                }
            }
        }
    }
}