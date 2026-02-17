using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelSerializer))]
public class LevelSerializerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelSerializer serializer = (LevelSerializer)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Save JSON"))
        {
            string json = serializer.ExportLevelToJson();
            serializer.SaveJsonToFile(json);
        }

        if (GUILayout.Button("Load JSON"))
        {
            string json = serializer.LoadJsonFromFile();
            if (!string.IsNullOrEmpty(json))
            {
                serializer.LoadLevelFromJson(json);
                Debug.Log("Level loaded from JSON.");
            }
        }

         if (GUILayout.Button("Clean Scene"))
        {
            serializer.CleanScene();
        }
    }
}
