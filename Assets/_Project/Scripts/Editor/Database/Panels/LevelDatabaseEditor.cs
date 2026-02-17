using UnityEngine;
using UnityEditor;

namespace tiplay.Toolkit
{
    public partial class DatabaseEditor : ITool
    {
        private SerializedProperty levelOrderProperty;
        private SerializedProperty levelsProperty;
        private void LevelDatabasePanel()
        {
            if (IsDatabaseExist("Level Database"))
            {
                Database.LevelDatabase.LevelIndex     = EditorGUILayout.IntField("Level Index ", Database.LevelDatabase.LevelIndex);
                Database.LevelDatabase.LevelTextValue = EditorGUILayout.IntField("Level Text Value ", Database.LevelDatabase.LevelTextValue);
                Database.LevelDatabase.LoopStartIndex = EditorGUILayout.IntField("Loop Start Index ", Database.LevelDatabase.LoopStartIndex);

                if (serializedDatabase == null)
                    serializedDatabase = new SerializedObject(Database);

                levelOrderProperty = serializedDatabase.FindProperty("LevelDatabase.LevelOrder");
                levelsProperty = serializedDatabase.FindProperty("LevelDatabase.Levels");

                serializedDatabase.Update();
                GUILayout.Space(5);
                GUILayout.BeginVertical();
                EditorGUILayout.PropertyField(levelOrderProperty, new GUIContent("Level Order"), true);
                EditorGUILayout.PropertyField(levelsProperty, new GUIContent("Levels"), true);
                GUILayout.EndVertical();
                serializedDatabase.ApplyModifiedProperties();
            }
        }
    }
}