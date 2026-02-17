using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using tiplay.EditorUtilities;

namespace tiplay.Toolkit.OptimizationToolkit
{
    public class TextureStats : ITool
    {
        private enum MipmapStatus { Enable, Disable }

        private string searchPath;
        private string filename;
        private TextureStatsData[] textureStatsArray;
        private bool isFiltersExpanded;
        private bool isToolsExpanded;
        private TextureSizeEnum textureSizeFilter = TextureSizeEnum._128;
        private MipmapStatus mipmap = MipmapStatus.Disable;
        private TextureSizeEnum textureSize = TextureSizeEnum._512;
        private TextureImporterType textureType = TextureImporterType.Default;

        public string Title => "Texture Stats";

        public string Shortcut => string.Empty;

        private int GetReloadButtonHeight()
        {
            int height = 50;

            if (isFiltersExpanded)
                height += 60;

            if (isToolsExpanded)
                height += 60;

            return height;
        }

        private void ReloadData()
        {
            textureStatsArray = TextureStatsUtility.GetAllTextureStats(searchPath, filename, textureSizeFilter);
        }

        public void OnDestroy() { }

        public void OnCreate() { }

        public void OnEnable()
        {
            ReloadData();
        }

        public void OnDisable() { }

        public void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    ToolsGUI();
                    FiltersGUI();
                }

                if (GUILayout.Button("RELOAD", GUILayout.Width(100), GUILayout.Height(GetReloadButtonHeight())))
                    ReloadData();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("REFRESH ALL", GUILayout.ExpandWidth(true), GUILayout.Height(25)))
                    RefreshAllTextureStats();

                if (GUILayout.Button("SELECT ALL", GUILayout.ExpandWidth(true), GUILayout.Height(25)))
                    SelectAllTextures();

                if (GUILayout.Button("DESELECT ALL", GUILayout.ExpandWidth(true), GUILayout.Height(25)))
                    DeselectAllTextures();
            }

            TextureStatsEditorGUI.DrawFields(textureStatsArray);
        }

        private void RefreshAllTextureStats()
        {
            foreach (var textureStat in textureStatsArray)
                textureStat.Refresh();
        }

        private void SelectAllTextures()
        {
            foreach (var textureStat in textureStatsArray)
                textureStat.isSelected = true;
        }

        private void DeselectAllTextures()
        {
            foreach (var textureStat in textureStatsArray)
                textureStat.isSelected = false;
        }

        private void ToolsGUI()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                isToolsExpanded = EditorGUILayout.Foldout(isToolsExpanded, "Tools", true);

                if (!isToolsExpanded)
                    return;

                MipMapToolGUI();
                TextureSizeToolGUI();
                TextureTypeToolGUI();
            }
        }

        private void TextureSizeToolGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                textureSize = (TextureSizeEnum)EditorGUILayout.EnumPopup("Max Texture Size", textureSize);

                if (GUILayout.Button("APPLY TO SELECTIONS", GUILayout.Width(160)))
                    ApplyTextureSizeChanges();
            }
        }

        private void TextureTypeToolGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                textureType = (TextureImporterType)EditorGUILayout.EnumPopup("Texture Type", textureType);

                if (GUILayout.Button("APPLY TO SELECTIONS", GUILayout.Width(160)))
                    ApplyTextureTypeChanges();
            }
        }

        private void MipMapToolGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                mipmap = (MipmapStatus)EditorGUILayout.EnumPopup("MipMap", mipmap);

                if (GUILayout.Button("APPLY TO SELECTIONS", GUILayout.Width(160)))
                    ApplyMipmapChanges();
            }
        }

        private void ApplyMipmapChanges()
        {
            foreach (var textureStat in textureStatsArray)
            {
                if (!textureStat.isSelected)
                    continue;

                TextureStatsUtility.SetTextureMipmap(textureStat, mipmap == MipmapStatus.Enable);
            }
        }

        private void ApplyTextureTypeChanges()
        {
            foreach (var textureStat in textureStatsArray)
            {
                if (!textureStat.isSelected)
                    continue;

                TextureStatsUtility.SetTextureType(textureStat, textureType);
            }
        }

        private void ApplyTextureSizeChanges()
        {
            foreach (var textureStat in textureStatsArray)
            {
                if (!textureStat.isSelected)
                    continue;

                TextureStatsUtility.SetTextureMaxSize(textureStat, textureSize);
            }
        }

        private void FiltersGUI()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                isFiltersExpanded = EditorGUILayout.Foldout(isFiltersExpanded, "Search Filters", true);

                if (!isFiltersExpanded)
                    return;

                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        searchPath = EditorGUILayout.TextField("Directory", searchPath);
                        filename = EditorGUILayout.TextField("Name", filename);
                        textureSizeFilter = (TextureSizeEnum)EditorGUILayout.EnumPopup("\"Resolution\" Greater Than", textureSizeFilter);
                    }

                    if (GUILayout.Button("SEARCH", GUILayout.Width(100), GUILayout.Height(60)))
                        ReloadData();
                }
            }
        }
    }
}

