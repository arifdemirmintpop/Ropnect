using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tiplay.HapticKit
{
    //[CreateAssetMenu(menuName = "Data/Haptic Database")]
    public class HapticKitDatabase : ScriptableObject
    {
        public List<Haptic> Haptics;
    }
}