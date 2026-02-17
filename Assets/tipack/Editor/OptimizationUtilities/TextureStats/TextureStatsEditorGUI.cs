using UnityEngine;
using UnityEditor;
using System;
using tiplay.EditorUtilities;

namespace tiplay.Toolkit.OptimizationToolkit
{
    public static class TextureStatsEditorGUI
    {
        public static void DrawFields(TextureStatsData[] textureStats)
        {
            Array.ForEach(textureStats, DrawField);
        }

        private static void DrawField(TextureStatsData textureStat)
        {
            if (!textureStat.texture)
                return;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    DrawSelectBox(textureStat);
                    DrawTextureField(textureStat.texture);
                    DrawRefreshButton(textureStat);
                    DrawTextureDetailsGUI(textureStat);

                    if (textureStat.importer != null)
                    {
                        DrawMipMapField(textureStat);
                        DrawMaxSizeField(textureStat);
                        DrawTextureTypeField(textureStat);
                    }
                }
            }
        }

        private static void DrawRefreshButton(TextureStatsData textureStat)
        {
            if (GUILayout.Button("↻", GUILayout.Width(25)))
                textureStat.Refresh();
        }

        private static void DrawSelectBox(TextureStatsData textureStat)
        {
            textureStat.isSelected = EditorGUILayout.Toggle(textureStat.isSelected, GUILayout.Width(15));
        }

        private static void DrawMaxSizeField(TextureStatsData textureStat)
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.HorizontalScope(GUILayout.Width(150)))
                {
                    GUILayout.Label("( Max Size", GUILayout.ExpandWidth(false));
                    textureStat.maxSize = (TextureSizeEnum)EditorGUILayout.EnumPopup(textureStat.maxSize, GUILayout.MaxWidth(60));
                    GUILayout.Label(")", GUILayout.ExpandWidth(false));
                }

                if (scope.changed)
                {
                    textureStat.importer.maxTextureSize = textureStat.maxSize.ToInt();
                    textureStat.importer.SaveAndReimport();
                }
            }
        }

        private static void DrawTextureTypeField(TextureStatsData textureStat)
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.HorizontalScope(GUILayout.Width(150)))
                {
                    GUILayout.Label("( Type", GUILayout.ExpandWidth(false));
                    textureStat.textureType = (TextureImporterType)EditorGUILayout.EnumPopup(textureStat.textureType, GUILayout.MaxWidth(60));
                    GUILayout.Label(")", GUILayout.ExpandWidth(false));
                }

                if (scope.changed)
                {
                    textureStat.importer.textureType = textureStat.textureType;
                    textureStat.importer.SaveAndReimport();
                }
            }
        }

        private static void DrawMipMapField(TextureStatsData textureStat)
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.HorizontalScope(GUILayout.Width(90)))
                {
                    GUILayout.Label("(", GUILayout.ExpandWidth(false));
                    textureStat.importer.mipmapEnabled = EditorGUILayout.ToggleLeft("MipMap", textureStat.importer.mipmapEnabled, GUILayout.Width(65));
                    GUILayout.Label(")", GUILayout.ExpandWidth(false));
                }

                if (scope.changed)
                {
                    textureStat.importer.SaveAndReimport();
                }
            }
        }

        private static void DrawTextureDetailsGUI(TextureStatsData textureStat)
        {
            GUILayout.Label($" (Runtime Size: {textureStat.runtimeSize})", GUILayout.Width(180));
            GUILayout.Label($" (Compressed Size: {textureStat.compressedSize})", GUILayout.Width(200));
            GUILayout.Label($" (Resolution: {textureStat.resolution.x}x{textureStat.resolution.y})", GUILayout.Width(180));
        }

        private static void DrawTextureField(Texture texture)
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField(texture, typeof(Texture), false, GUILayout.Width(150));
            GUI.enabled = true;
        }
    }


}

