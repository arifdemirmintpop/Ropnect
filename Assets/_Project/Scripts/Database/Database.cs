using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tiplay.DatabaseSystem
{
    //[CreateAssetMenu(menuName = "Data/Database")]
    [Serializable]
    public class Database : ScriptableObject
    {
        public BlockerDatabase BlockerDatabase;
        public InventoryDatabase InventoryDatabase;
        public LevelDatabase LevelDatabase;
        public PreferenceDatabase PreferenceDatabase;
        public UserEngagementDatabase UserEngagementDatabase;
        public VersionDatabase VersionDatabase;
    }
}