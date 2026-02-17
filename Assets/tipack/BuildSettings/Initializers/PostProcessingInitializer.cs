using System;
using UnityEngine;
using UnityEngine.SceneManagement;
#if TIPLAY_POSTPROCESSING
using UnityEngine.Rendering.PostProcessing;
#endif

namespace tiplay
{
    public static class PostProcessingInitializer
    {
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            InitializePostProcessing();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (loadMode != LoadSceneMode.Single)
                return;

            InitializePostProcessing();
        }

        private static void InitializePostProcessing()
        {
#if TIPLAY_POSTPROCESSING && !UNITY_EDITOR
            bool enabled = GlobalData.GetActiveGameSettings().postProcessEnabled == PostProcessStatus.Enable;

            Camera mainCamera = Camera.main;

            if (!mainCamera) return;

            if (!mainCamera.TryGetComponent(out PostProcessLayer postProcessLayer)) return;

            postProcessLayer.enabled = enabled;
#endif
        }
    }
}
