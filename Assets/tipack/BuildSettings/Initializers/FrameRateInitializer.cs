using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace tiplay
{
    public static class FrameRateInitializer
    {
        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            UpdateTargetFrameRate();
        }

        private static void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            UpdateTargetFrameRate();
        }

        private static void UpdateTargetFrameRate()
        {
#if !UNITY_EDITOR
            int buildFrameRate = GlobalData.GetActiveGameSettings().frameRate;            
            Application.targetFrameRate = buildFrameRate;
#endif
        }
    }
}
