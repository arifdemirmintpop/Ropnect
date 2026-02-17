using UnityEngine;
using UnityEditor;
using tiplay.EditorUtilities;

namespace tiplay.Toolkit.OptimizationToolkit
{
    public class TextureStatsData
    {
        public bool isSelected;
        public bool isExpanded;
        public Texture texture;
        public Vector2 resolution;
        public string path;

        public string runtimeSize;
        public long runtimeSizeLong;

        public long compressedSizeLong;
        public string compressedSize;

        public TextureImporter importer;
        public TextureSizeEnum maxSize;
        public TextureImporterType textureType;

        public TextureStatsData(Texture texture, TextureImporter importer)
        {
            this.texture = texture;
            this.importer = importer;
            Refresh();
        }

        public void Refresh()
        {
            textureType = importer.textureType;
            maxSize = TextureSizeEnumUtility.IntToTextureSize(importer.maxTextureSize);
            runtimeSizeLong = TextureEditorUtility.GetRuntimeSizeLong(texture);
            runtimeSize = TextureEditorUtility.GetRuntimeSize(texture);
            compressedSizeLong = TextureEditorUtility.GetCompressedSizeLong(texture);
            compressedSize = TextureEditorUtility.GetCompressedSize(texture);
            resolution = new Vector2(texture.width, texture.height);
        }
    }


}

