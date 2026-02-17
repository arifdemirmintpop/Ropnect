using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;
using System;
using System.Reflection;

namespace tiplay.EditorUtilities
{
    public static class TextureEditorUtility
    {
        public static TextureImporter GetTextureImporter(string path)
        {
            return AssetImporter.GetAtPath(path) as TextureImporter;
        }

        public static int GetCompressedSizeLong(Texture texture)
        {
            if (texture == null)
                return 0;

            Assembly unityEditor = Assembly.Load("UnityEditor.dll");
            Type textureUtil = unityEditor.GetType("UnityEditor.TextureUtil");
            BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public;
            MethodInfo storeSizeMethod = textureUtil.GetMethod("GetStorageMemorySize", bindingAttr);
            return (int)storeSizeMethod.Invoke(null, new object[] { texture });
        }

        public static string GetCompressedSize(Texture texture)
        {
            return EditorUtility.FormatBytes(GetCompressedSizeLong(texture));
        }

        public static long GetRuntimeSizeLong(Texture texture)
        {
            return Profiler.GetRuntimeMemorySizeLong(texture);
        }

        public static string GetRuntimeSize(Texture texture)
        {
            return EditorUtility.FormatBytes(GetRuntimeSizeLong(texture));
        }
    }
}

