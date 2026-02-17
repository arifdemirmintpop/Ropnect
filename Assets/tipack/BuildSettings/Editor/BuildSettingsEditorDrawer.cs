using UnityEngine;
using UnityEditor;
using System;

namespace tiplay
{
    public static class BuildSettingsEditorDrawer
    {
        public static void DrawEditorGUI(BuildSettingsData settings)
        {
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var scope = new EditorGUI.ChangeCheckScope())
                {
                    settings.postProcessEnabled = (PostProcessStatus)EditorGUILayout.EnumPopup("Post Process", settings.postProcessEnabled);
                    settings.shadowQuality = (ShadowQuality)EditorGUILayout.EnumPopup("Shadow Quality", settings.shadowQuality);
                    settings.frameRate = EditorGUILayout.IntField("Build FPS Limit", settings.frameRate);

                    if (scope.changed)
                        SaveSettings(settings);
                }
            }
        }

        private static void SaveSettings(BuildSettingsData settings)
        {
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssetIfDirty(settings);
        }
    }
}