using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tiplay.HapticKit
{
    [Serializable]
    public class Haptic
    {
        public HapticType HapticType;
        public HapticPlatformPresets PlatformPresets;
    }

    [Serializable]
    public class HapticPlatformPresets
    {
        public PresetType AndroidPreset;
        public PresetType IosPreset;
    }
}