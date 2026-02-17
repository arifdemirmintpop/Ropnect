using UnityEngine;
using UnityEditor;

namespace tiplay.Toolkit
{
    public partial class DatabaseEditor : ITool
    {
        private SerializedProperty blockersProperty;
        private void BlockerDatabasePanel()
        {
            if (IsDatabaseExist("Blocker Database"))
            {
                if (serializedDatabase == null)
                    serializedDatabase = new SerializedObject(Database);

                blockersProperty = serializedDatabase.FindProperty("BlockerDatabase.Blockers");
                serializedDatabase.Update();
                GUILayout.Space(5);
                GUILayout.BeginVertical();
                EditorGUILayout.PropertyField(blockersProperty, new GUIContent("Blockers"), true);
                GUILayout.EndVertical();
                serializedDatabase.ApplyModifiedProperties();
            }
        }
    }
}