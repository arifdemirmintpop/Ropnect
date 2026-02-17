using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityEditor.Experimental.GraphView;
using tiplay.SnapTool;
using static UnityEditor.Progress;

namespace tiplay.Toolkit.DesignToolkit
{
    public class GroundSnapper : ITool
    {
        private GroundSnapperSettings settings;

        public string Title => "Ground Snapper";

        public string Shortcut => "Shift + " + settings.shortcut;

        public void OnCreate()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            settings = Resources.Load<GroundSnapperSettings>("GroundSnapperSettings");
        }

        public void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        public void OnEnable()
        {

        }

        public void OnDisable()
        {
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            DrawGizmos();
            ShortcutListener();
        }

        private void ShortcutListener()
        {
            if (!settings.shortcut.IsPressDown())
                return;

            Snap();
            Event.current.Use();
        }

        public void OnGUI()
        {
            ShortcutListener();

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Gizmos", GUILayout.Width(150));
                    settings.gizmos = EditorGUILayout.Toggle(settings.gizmos);
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Shortcut", GUILayout.Width(150));
                    settings.shortcut = ShortcutEditorGUILayout.DrawShortcutField(settings.shortcut, false);
                }

                SnapSettingsEditorUtility.DrawSnapSettingsField(settings.snapSettings);

                if (scope.changed)
                {
                    Undo.RegisterCompleteObjectUndo(settings, "Snapper");
                    EditorUtility.SetDirty(settings);
                    AssetDatabase.SaveAssetIfDirty(settings);
                }
            }

            if (GUILayout.Button("Snap"))
                Snap();
        }

        public void Snap()
        {
            foreach (var transform in Selection.transforms)
            {
                if (!SnapManager.GetSnapLocation(transform, settings.snapSettings, out Vector3 position, out Quaternion rotation))
                    continue;

                Undo.RegisterCompleteObjectUndo(transform, "Ground Snap");
                transform.rotation = rotation;
                transform.position = position;
            }

            SceneView.RepaintAll();
        }

        public void DrawGizmos()
        {
            if (!settings.gizmos)
                return;

            if (Event.current.type != EventType.Repaint)
                return;

            SnapGizmoUtility.DrawGizmo(Selection.transforms, settings.snapSettings);
        }
    }
}