using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;
using System.IO;
using tiplay.ScriptingDefines;

namespace tiplay.EditorUtilities
{
    public static class TibugEditorUtility
    {
        private static string tibugRoot = "Assets/tibug";

        public static bool IsTibugEnabled => TibugScriptingDefineSymbol.IsDefined;

        public static void EnableTibug()
        {
            TibugScriptingDefineSymbol.Define();
        }

        public static void DisableTibug()
        {
            TibugScriptingDefineSymbol.Remove();
        }

        public static void EditSettings()
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = GetSettings();
        }

        public static GameObject GetPrefab()
        {
            string tibugPrefabPath = GetAssetPath("Tibug.prefab");
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/" + tibugPrefabPath);
            return prefab;
        }

        public static ScriptableObject GetSettings()
        {
            string settingsPath = GetAssetPath("TibugSettings.asset");
            var settings = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/" + settingsPath);
            return settings;
        }

        private static string GetAssetPath(string assetName)
        {
            string[] assets = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(assetName), new string[] { tibugRoot });

            for (int i = 0; i < assets.Length; i++)
                assets[i] = AssetDatabase.GUIDToAssetPath(assets[i]);

            foreach (var assetPath in assets)
            {
                if (Path.GetFileName(assetPath) != assetName) continue;

                return assetPath.Substring("Assets/".Length);
            }

            return string.Empty;
        }
    }
}