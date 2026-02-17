using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using tiplay.SnapTool;

namespace tiplay.Toolkit.DesignToolkit
{
    public class ObjectPlacer : ITool
    {
        private ObjectPlacerSettings settings;

        public string Title => "Object Placer";

        public string Shortcut => "Shift + " + settings.shortcut;

        public void OnCreate()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            settings = Resources.Load<ObjectPlacerSettings>("ObjectPlacerSettings");
        }

        public void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        public void OnEnable() { }

        public void OnDisable() { }

        private void OnSceneGUI(SceneView sceneView)
        {
            ShortcutListener();
        }

        private void ShortcutListener()
        {
            if (!settings.shortcut.IsPressDown())
                return;

            Place();
            Event.current.Use();
        }

        public void OnGUI()
        {
            ShortcutListener();

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Shortcut", GUILayout.Width(150));
                    settings.shortcut = ShortcutEditorGUILayout.DrawShortcutField(settings.shortcut, false);
                }
                SnapSettingsEditorUtility.DrawSnapSettingsField(settings.snapSettings);

                if (scope.changed)
                    SaveSettings();
            }
        }

        private void SaveSettings()
        {
            Undo.RegisterCompleteObjectUndo(settings, "Snapper");
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssetIfDirty(settings);
        }

        public void Place()
        {
            Transform transform = Selection.activeTransform;

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity,
                settings.snapSettings.layerMask,
                settings.snapSettings.triggerInteraction))
                return;

            Undo.RegisterCompleteObjectUndo(transform, "Ground Snap");

            Vector3 position;
            Quaternion rotation;

            SnapManager.GetSnapLocation(transform, hit, settings.snapSettings, out position, out rotation);
            transform.position = position;
            transform.rotation = rotation;

            SceneView.RepaintAll();
        }
    }
}