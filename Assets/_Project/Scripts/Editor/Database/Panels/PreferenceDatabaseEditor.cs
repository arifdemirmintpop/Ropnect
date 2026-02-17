using UnityEngine;
using UnityEditor;

namespace tiplay.Toolkit
{
    public partial class DatabaseEditor : ITool
    {
        private void PreferenceDatabasePanel()
        {
            if (IsDatabaseExist("Preference Database"))
            {
                Database.PreferenceDatabase.HasAudio = EditorGUILayout.Toggle("Has Audio ", Database.PreferenceDatabase.HasAudio);
                Database.PreferenceDatabase.IsVibrationEnable = EditorGUILayout.Toggle("Is Vibration Enable ", Database.PreferenceDatabase.IsVibrationEnable);
                Database.PreferenceDatabase.IsAudioEnable = EditorGUILayout.Toggle("Is Audio Enable ", Database.PreferenceDatabase.IsAudioEnable);
                Database.PreferenceDatabase.IsMusicEnable = EditorGUILayout.Toggle("Is Music Enable ", Database.PreferenceDatabase.IsMusicEnable);

            }
        }
    }
}