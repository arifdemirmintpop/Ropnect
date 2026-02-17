using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using tiplay.SnapTool;

namespace tiplay.Toolkit.DesignToolkit
{
    public class ObjectInstantiator : ITool
    {
        private EditorWindow window;
        private InstantiatorSettings settings;
        private Texture2D preview;
        private bool repaint;

        public string Title => "Instantiator";

        public string Shortcut => "Shift + ";

        public void OnCreate()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            settings = Resources.Load<InstantiatorSettings>("InstantiatorSettings");
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
            for (int i = 0; i < settings.items.Count; i++)
            {
                if (!settings.items[i].shortcut.IsPressDown())
                    continue;

                Instantiate(settings.items[i].prefab.transform);
                Event.current.Use();
                return;
            }
        }

        public void OnGUI()
        {

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                SnapSettingsEditorUtility.DrawSnapSettingsField(settings.snapSettings);

                for (int i = 0; i < settings.items.Count; i++)
                {
                    if (repaint)
                        break;

                    DrawItemGUI(settings.items[i]);
                }

                if (scope.changed)
                    SaveSettings();
            }

            if (GUILayout.Button("ADD NEW ITEM"))
                AddNewItem();

            ShortcutListener();

            repaint = false;
        }

        private void SaveSettings()
        {
            Undo.RegisterCompleteObjectUndo(settings, "Snapper");
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssetIfDirty(settings);
        }

        private void AddNewItem()
        {
            Undo.RegisterCompleteObjectUndo(settings, "Object Replacer");
            settings.items.Add(new InstantiatorSettings.ReplaceItem());
            SaveSettings();
        }

        private void DrawItemGUI(InstantiatorSettings.ReplaceItem item)
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                DrawItemPreview(item);

                using (new EditorGUILayout.VerticalScope())
                {
                    item.prefab = (GameObject)EditorGUILayout.ObjectField(item.prefab, typeof(GameObject), false, GUILayout.ExpandWidth(true));

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        item.shortcut = ShortcutEditorGUILayout.DrawShortcutField(item.shortcut, false);
                        DrawItemRemoveButton(item);
                    }
                }
            }
        }

        private void DrawItemPreview(InstantiatorSettings.ReplaceItem item)
        {
            if (item.prefab)
            {
                preview = AssetPreview.GetAssetPreview(item.prefab);
                EditorGUILayout.LabelField(new GUIContent(preview), GUILayout.Width(40), GUILayout.Height(40));
            }
        }

        private void DrawItemRemoveButton(InstantiatorSettings.ReplaceItem item)
        {
            if (GUILayout.Button("Remove", GUILayout.Width(80)))
            {
                Undo.RegisterCompleteObjectUndo(settings, "Object Instantiator");
                settings.items.Remove(item);
                SaveSettings();
                repaint = true;
            }
        }

        private void Instantiate(Transform transform)
        {
            if (!GroundRay(out RaycastHit hit))
                return;

            SnapManager.GetSnapLocation(transform, hit, settings.snapSettings, out var position, out var rotation);

            Transform instance = PrefabUtility.InstantiatePrefab(transform) as Transform;
            instance.position = position;
            instance.rotation = rotation;
            instance.SetAsLastSibling();
            Undo.RegisterCreatedObjectUndo(instance.gameObject, "Instantiator");

            Selection.activeObject = instance;
            SceneView.RepaintAll();
        }

        private bool GroundRay(out RaycastHit hit)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            return Physics.Raycast(ray, out hit, Mathf.Infinity,
                settings.snapSettings.layerMask,
                settings.snapSettings.triggerInteraction);
        }
    }
}