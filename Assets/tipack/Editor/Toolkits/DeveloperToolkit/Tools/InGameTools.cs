using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#if TIPLAY_POSTPROCESSING
using UnityEngine.Rendering.PostProcessing;
#endif

namespace tiplay.Toolkit.DeveloperToolkit
{
    public class InGameTools : ITool
    {
        private bool postProcessing
        {
            get => EditorPrefs.GetBool("PostProcess_" + Application.identifier, true);
            set => EditorPrefs.SetBool("PostProcess_" + Application.identifier, value);
        }

        public string Title => "InGame Editor";

        public string Shortcut => string.Empty;

        public void OnCreate()
        {
            EditorSceneManager.sceneLoaded += EditorSceneManager_SceneLoaded;
            UpdatePostProcessingStatus();
        }

        private void EditorSceneManager_SceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            UpdatePostProcessingStatus();
        }

        public void OnDestroy()
        {
            EditorSceneManager.sceneLoaded -= EditorSceneManager_SceneLoaded;
        }

        public void OnEnable() { }
        public void OnDisable() { }

        public void OnGUI()
        {
            DrawGameFPSField();
            DrawPostProcessField();
            WinFailPanelButtons();
        }

        private static void WinFailPanelButtons()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Show Win Panel"))
                    EventManager.OnGameWin?.Invoke();

                if (GUILayout.Button("Show Fail Panel"))
                    EventManager.OnGameFail?.Invoke();
            }
        }

        private void DrawPostProcessField()
        {
#if TIPLAY_POSTPROCESSING
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Post Processing", GUILayout.Width(150));
                    postProcessing = GUILayout.Toolbar(postProcessing ? 0 : 1, new string[] { "Enable", "Disable" }) == 0;
                }

                if (!scope.changed) return;
                if (!Application.isPlaying) return;

                UpdatePostProcessingStatus();
            }
#endif
        }

        private void UpdatePostProcessingStatus()
        {
#if TIPLAY_POSTPROCESSING
            if (!Application.isPlaying) return;

            var mainCamera = Camera.main;

            if (!mainCamera) return;

            var layer = mainCamera.GetComponent<PostProcessLayer>();

            if (!layer) return;

            layer.enabled = postProcessing;
#endif
        }

        private void DrawGameFPSField()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Game FPS", GUILayout.Width(150));

                Application.targetFrameRate = EditorGUILayout.IntField(Application.targetFrameRate);
                Application.targetFrameRate = Mathf.Max(0, Application.targetFrameRate);
            }
        }
    }
}

