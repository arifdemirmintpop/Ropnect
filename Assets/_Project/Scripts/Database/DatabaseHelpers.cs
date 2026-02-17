using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tiplay;
using System;

namespace tiplay.DatabaseSystem
{
    public static class DatabaseHelpers
    {
        private static Database database => GlobalData.Database;

        #region Preference Helpers
        public static bool HasAudio()
        {
            return database.PreferenceDatabase.HasAudio;
        }
        public static bool IsVibrationEnable()
        {
            return database.PreferenceDatabase.IsVibrationEnable;
        }

        public static bool IsAudioEnable()
        {
            return database.PreferenceDatabase.IsAudioEnable;
        }
        #endregion

        #region User Engagement Helpers
        public static long TakeTimestamp()
        {
            return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        #endregion
    }
}
