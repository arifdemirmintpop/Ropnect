using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace tiplay.AudioKit
{
    public static class AudioKitEditorUtility
    {
        [MenuItem("Tiplay/Audio Kit/Edit Audio Kit Database")]
        private static void EditAudioKitDatabase()
        {
            var _audioKitDatabase = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/TiplayPackages/AudioKit/v1.1/Resources/AudioKitDatabase.asset");
                if (_audioKitDatabase != null)
                {
                    Selection.activeObject = _audioKitDatabase;
                    EditorGUIUtility.PingObject(_audioKitDatabase);
                }
                else if (_audioKitDatabase == null)
                {
                    Debug.LogError("There is no Audio Kit Database");
                }
        }

        [MenuItem("Tiplay/Audio Kit/Edit Sound Enums")]
        private static void EditSoundEnums()
        {
                var _enumHolder = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/TiplayPackages/AudioKit/v1.1/Scripts/Holders/AudioKitEnumHolder.cs");
                if (_enumHolder != null)
                {
                    AssetDatabase.OpenAsset(_enumHolder);
                }
                else if (_enumHolder)
                {
                    Debug.LogError("There is no Audio Kit Enum Holder");
                }
        }
    }
}