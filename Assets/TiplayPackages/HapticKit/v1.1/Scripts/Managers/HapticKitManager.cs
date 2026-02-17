using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lofelt.NiceVibrations;
using UnityEngine;
using tiplay.DatabaseSystem;

namespace tiplay.HapticKit
{
    public static class HapticKitManager
    {
        private static HapticKitDatabase hapticKitDatabase;

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            LoadHapticKitDatabase();
            if (!IsHapticKitDatabaseLoaded())
            {
                Debug.LogError("Haptic Database can not loaded");
            }
        }

        private static void LoadHapticKitDatabase()
        {
            hapticKitDatabase = Resources.Load<HapticKitDatabase>("HapticKitDatabase");
        }

        private static bool IsHapticKitDatabaseLoaded()
        {
            return hapticKitDatabase is null ? false : true;
        }

        public static void Vibrate(HapticType _hapticType)
        {
            if (!IsHapticKitDatabaseLoaded()) return;
            if (!DatabaseHelpers.IsVibrationEnable()) return;
            Haptic _haptic = hapticKitDatabase.Haptics.Where(x => x.HapticType == _hapticType).FirstOrDefault();
            if (_haptic is null)
            {
                Debug.LogError("There is no haptic defination as " + _hapticType.ToString());
                return;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
           HapticPatterns.PlayPreset(GetPresetType(_haptic.PlatformPresets.AndroidPreset));
#elif UNITY_IOS && !UNITY_EDITOR
           HapticPatterns.PlayPreset(GetPresetType(_haptic.PlatformPresets.IosPreset));
#elif UNITY_EDITOR
           Debug.Log("**Vibration** iOS Preset Type: " + _haptic.PlatformPresets.IosPreset.ToString());
#endif
        }

        private static HapticPatterns.PresetType GetPresetType(HapticKit.PresetType _presetType)
        {
            return _presetType switch
            {
                HapticKit.PresetType.Failure => HapticPatterns.PresetType.Failure,
                HapticKit.PresetType.HeavyImpact => HapticPatterns.PresetType.HeavyImpact,
                HapticKit.PresetType.LightImpact => HapticPatterns.PresetType.LightImpact,
                HapticKit.PresetType.MediumImpact => HapticPatterns.PresetType.MediumImpact,
                HapticKit.PresetType.RigidImpact => HapticPatterns.PresetType.RigidImpact,
                HapticKit.PresetType.Selection => HapticPatterns.PresetType.Selection,
                HapticKit.PresetType.SoftImpact => HapticPatterns.PresetType.SoftImpact,
                HapticKit.PresetType.Success => HapticPatterns.PresetType.Success,
                HapticKit.PresetType.Warning => HapticPatterns.PresetType.Warning,
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}