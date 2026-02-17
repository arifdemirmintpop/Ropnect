using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tiplay.RemoteExporterTool;

namespace tiplay.DatabaseSystem
{
    #region Database Classes
    [Serializable]
    public class BannerDatabase
    {
        public bool IsBannerDelayCompleted;
        public float BannerCountdown;
    }

    [Serializable]
    public class BlockerDatabase
    {
        public List<BlockerData> Blockers;
    }

    [Serializable]
    public class InterstitialDatabase
    {
        public bool IsFirstInterstitialShown;
    }

    [Serializable]
    public class InventoryDatabase
    {
        public float Money;
    }

    [Serializable]
    public class LevelDatabase
    {
        public int LevelIndex;
        public int LevelTextValue;
        [Remote] public int LoopStartIndex;
        [Remote] public List<int> LevelOrder;
        public List<LevelData> Levels;
    }

    [Serializable]
    public class PreferenceDatabase
    {
        public bool HasAudio;
        public bool IsVibrationEnable;
        public bool IsAudioEnable;

        public bool IsMusicEnable;
    }

    [Serializable]
    public class UserEngagementDatabase
    {
        public bool IsUpdatedUser;
        [PreservedField] public long InstallTimestamp;
        [PreservedField] public long TotalPlaytime;
    }

    [Serializable]
    public class VersionDatabase
    {
        public string Version;
        public string BuildNumber;
    }
    #endregion

    #region Sub Classes
    [Serializable]
    public class BlockerData
    {
        public string Name;
        public int StartLevel;
        public int EndLevel;
        public string Description;
    }

    [Serializable]
    public class LevelData
    {
        [Remote(true)] public string Name;
        [Remote] public LevelDifficulty Difficulty;
        [Remote] public bool IsSkippedAfterLoop;
        public int Duration;
        public int MoveCount;
        public int SlotCount;
    }
    #endregion
}