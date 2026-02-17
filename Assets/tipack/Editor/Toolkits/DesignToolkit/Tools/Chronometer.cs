using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityEditor;
using System;
using System.Text;
using tiplay.GameToolkit;

namespace tiplay.Toolkit.DesignToolkit
{
    public class ChronometerTool : ITool
    {
        private ToolkitWindow window;
        private bool autoStart = true;
        private bool autoPause = true;
        private bool autoStop = true;

        public string Title => "Chronometer";

        public string Shortcut => string.Empty;

        public ChronometerTool(ToolkitWindow window)
        {
            this.window = window;
        }

        public void OnCreate()
        {
            EditorApplication.update += UpdateGUI;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.pauseStateChanged += OnPauseStateChanged;
        }

        private void OnPauseStateChanged(PauseState state)
        {
            if (!autoPause)
                return;

            if (state == PauseState.Paused)
                Chronometer.Stop();

            if (state == PauseState.Unpaused && Application.isPlaying)
                Chronometer.Start();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (autoStart && state == PlayModeStateChange.EnteredPlayMode)
                Chronometer.Restart();

            if (autoStop && state == PlayModeStateChange.ExitingPlayMode)
                Chronometer.Stop();
        }

        public void OnDestroy()
        {
            EditorApplication.update -= UpdateGUI;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.pauseStateChanged -= OnPauseStateChanged;
        }

        private void UpdateGUI()
        {
            if (Chronometer.IsRunning)
                window.Repaint();
        }

        public void OnDisable()
        {

        }

        public void OnEnable()
        {

        }

        public void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Auto Start", GUILayout.Width(80));
                autoStart = EditorGUILayout.Toggle(autoStart);
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Auto Pause", GUILayout.Width(80));
                autoPause = EditorGUILayout.Toggle(autoPause);
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Auto Stop", GUILayout.Width(80));
                autoStop = EditorGUILayout.Toggle(autoStop);
            }

            GUILayout.Label("Chronometer: " + Chronometer.GetTime());

            if (Chronometer.ElapsedMilliseconds == 0)
                if (GUILayout.Button("Start"))
                    Chronometer.Restart();

            if (Chronometer.ElapsedMilliseconds > 0)
            {
                if (!Chronometer.IsRunning)
                    if (GUILayout.Button("Continue"))
                        Chronometer.Start();

                if (Chronometer.IsRunning)
                    if (GUILayout.Button("Pause"))
                        Chronometer.Stop();

                if (GUILayout.Button("Restart"))
                    Chronometer.Restart();

                if (GUILayout.Button("Reset"))
                    Chronometer.Reset();
            }

            ChronometerEditorUtility.DrawSettingsField(ChronometerSettingsData.GetInstance());
        }
    }
}

