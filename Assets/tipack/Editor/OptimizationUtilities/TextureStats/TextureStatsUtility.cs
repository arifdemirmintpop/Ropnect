using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using tiplay.EditorUtilities;

namespace tiplay.Toolkit.OptimizationToolkit
{

    public static class TextureStatsUtility
    {
        public static TextureStatsData[] GetAllTextureStats(string searchFolder, string filename, TextureSizeEnum minTextureSize)
        {
            string[] textureGuids = AssetDatabase.FindAssets($"t:Texture {filename}", new string[] { "Assets/" + searchFolder });
            string[] texturePaths = new string[textureGuids.Length];
            for (int i = 0; i < textureGuids.Length; i++)
                texturePaths[i] = AssetDatabase.GUIDToAssetPath(textureGuids[i]);

            List<TextureStatsData> textureStats = new List<TextureStatsData>();
            for (int i = 0; i < texturePaths.Length; i++)
            {
                Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(texturePaths[i]);

                if (Mathf.Max(texture.width, texture.height) < minTextureSize.ToInt())
                    continue;

                TextureImporter importer = TextureEditorUtility.GetTextureImporter(texturePaths[i]);

                if (importer == null)
                    continue;

                textureStats.Add(new TextureStatsData(texture, importer));
            }


            return textureStats.OrderBy(textureStat => textureStat.runtimeSizeLong).Reverse().ToArray();
        }

        public static void SetTextureMaxSize(TextureStatsData textureStat, TextureSizeEnum textureSize)
        {
            textureStat.maxSize = textureSize;
            textureStat.importer.maxTextureSize = textureSize.ToInt();
            textureStat.importer.SaveAndReimport();
        }

        public static void SetTextureType(TextureStatsData textureStat, TextureImporterType textureType)
        {
            textureStat.textureType = textureType;
            textureStat.importer.textureType = textureType;
            textureStat.importer.SaveAndReimport();
        }

        public static void SetTextureMipmap(TextureStatsData textureStat, bool mipmap)
        {
            textureStat.importer.mipmapEnabled = mipmap;
            textureStat.importer.SaveAndReimport();
        }
    }


}

