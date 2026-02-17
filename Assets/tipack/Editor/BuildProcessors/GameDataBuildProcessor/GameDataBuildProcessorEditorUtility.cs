using UnityEditor;
using UnityEngine;

namespace tiplay.BuildProcessors
{
    public class GameDataBuildProcessorEditorUtility
    {
        public static void DrawProcessorSettings()
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                GameDataBuildProcessorData.ResetGameData = EditorGUILayout.Toggle("Reset Game Data", GameDataBuildProcessorData.ResetGameData);

                if (!scope.changed) return;

                GameDataBuildProcessorData.SaveData();
            }
        }
    }
}

