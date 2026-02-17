using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace tiplay.HapticKit
{
    public static class HapticKitEditorUtility
    {
        [MenuItem("Tiplay/Haptic Kit/Edit Haptic Kit Database")]
        private static void EditHapticDatabase()
        {
            var _hapticKitDatabase = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/TiplayPackages/HapticKit/v1.1/Resources/HapticKitDatabase.asset");
                if (_hapticKitDatabase != null)
                {
                    Selection.activeObject = _hapticKitDatabase;
                    EditorGUIUtility.PingObject(_hapticKitDatabase);
                }
                else if (_hapticKitDatabase == null)
                {
                    Debug.LogError("There is no Haptic Kit Database");
                }
        }

        [MenuItem("Tiplay/Haptic Kit/Edit Haptic Type Enums")]
        private static void EditHapticTypeEnums()
        {
                var _enumHolder = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/TiplayPackages/HapticKit/v1.1/Scripts/Holders/HapticKitEnumHolder.cs");
                if (_enumHolder != null)
                {
                    AssetDatabase.OpenAsset(_enumHolder);
                }
                else if (_enumHolder)
                {
                    Debug.LogError("There is no Haptic Kit Enum Holder");
                }
        }
    }
}