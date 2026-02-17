using UnityEditor;
using UnityEngine;

namespace tiplay.GameToolkit
{
    public static class ChronometerEditorUtility
    {
        public static void DrawSettingsField(ChronometerSettingsData settingsData)
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("GUI Settings", EditorStyles.boldLabel);

                    bool displayInGame = EditorGUILayout.Toggle("Display In Game", settingsData.displayInGame);
                    int fontSize = EditorGUILayout.IntSlider("Font Size", settingsData.fontSize, 0, 200);
                    Color textColor = EditorGUILayout.ColorField("Text Color", settingsData.textColor);
                    TextAnchor textAlignment = (TextAnchor)EditorGUILayout.EnumPopup("Text Alignment", settingsData.textAlignment);

                    if (!scope.changed) return;

                    Undo.RegisterCompleteObjectUndo(settingsData, "Chronometer Settings");

                    settingsData.displayInGame = displayInGame;
                    settingsData.fontSize = fontSize;
                    settingsData.textColor = textColor;
                    settingsData.textAlignment = textAlignment;

                    EditorUtility.SetDirty(settingsData);
                    AssetDatabase.SaveAssetIfDirty(settingsData);
                }

            }
        }
    }
}

