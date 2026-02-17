using UnityEngine;
using UnityEditor;

namespace tiplay.Toolkit
{
    public partial class DatabaseEditor : ITool
    {
        private void VersionDatabasePanel()
        {
            if (IsDatabaseExist("Version Database"))
            {
                Database.VersionDatabase.Version = EditorGUILayout.TextField("Version ", Database.VersionDatabase.Version);
                Database.VersionDatabase.BuildNumber = EditorGUILayout.TextField("Build Number ", Database.VersionDatabase.BuildNumber);
            }
        }
    }
}