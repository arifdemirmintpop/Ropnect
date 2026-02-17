using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using tiplay.EditorUtilities;
using System.Linq;

namespace tiplay.Toolkit.DesignToolkit
{
    public class ObjectReplacer : ITool
    {
        private ObjectReplaceSettings settings;
        private Texture2D preview;
        private bool repaint;

        public string Title => "Object Replacer";

        public string Shortcut => "Shift + ";

        public void OnCreate()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            settings = Resources.Load<ObjectReplaceSettings>("ObjectReplaceSettings");
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

                Change(settings.items[i].prefab.transform);
                Event.current.Use();
                return;
            }
        }

        public void OnGUI()
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                for (int i = 0; i < settings.items.Count; i++)
                {
                    if (repaint)
                        break;

                    DrawItem(settings.items[i]);
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
            settings.items.Add(new ObjectReplaceSettings.ReplaceItem());
            SaveSettings();
        }

        private void DrawItem(ObjectReplaceSettings.ReplaceItem item)
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                DrawItemPreview(item);

                using (new EditorGUILayout.VerticalScope())
                {
                    item.prefab = (GameObject)EditorGUILayout.ObjectField(item.prefab, typeof(GameObject), false, GUILayout.ExpandWidth(true));

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Shortcut", GUILayout.ExpandWidth(false));
                        item.shortcut = ShortcutEditorGUILayout.DrawShortcutField(item.shortcut, false);
                    }
                }

                using (new EditorGUILayout.VerticalScope(GUILayout.Width(80)))
                {
                    DrawReplaceButton(item);
                    DrawRemoveButton(item);
                }
            }
        }

        private void DrawRemoveButton(ObjectReplaceSettings.ReplaceItem item)
        {
            if (GUILayout.Button("Remove"))
            {
                Undo.RegisterCompleteObjectUndo(settings, "Object Replacer");
                settings.items.Remove(item);
                SaveSettings();
                repaint = true;
            }
        }

        private void DrawReplaceButton(ObjectReplaceSettings.ReplaceItem item)
        {
            if (GUILayout.Button("Replace"))
            {
                Change(item.prefab.transform);
                repaint = true;
            }
        }

        private void DrawItemPreview(ObjectReplaceSettings.ReplaceItem item)
        {
            if (item.prefab)
            {
                preview = AssetPreview.GetAssetPreview(item.prefab);
                EditorGUILayout.LabelField(new GUIContent(preview), GUILayout.Width(40), GUILayout.Height(40));
            }
        }

        private void Change(Transform transform)
        {
            GameObject[] selections = Selection.transforms.Select(transform => transform.gameObject).ToArray();
            PrefabEditorUtility.ReplaceAll(selections, transform.gameObject);

            //List<GameObject> instances = new List<GameObject>();

            //for (int i = 0; i < selections.Length; i++)
            //{
            //    if (selections[i].gameObject.scene == null || !selections[i].gameObject.scene.isLoaded)
            //        continue;

            //Transform instance = (Transform)PrefabUtility.InstantiatePrefab(gameObject, selections[i].gameObject.scene);
            //instance.parent = selections[i].parent;
            //instance.localPosition = selections[i].localPosition;
            //instance.localRotation = selections[i].localRotation;
            //instance.SetSiblingIndex(selections[i].GetSiblingIndex());
            //Undo.RegisterCreatedObjectUndo(instance.gameObject, "Prefab Changer");

            //Undo.DestroyObjectImmediate(selections[i].gameObject);
            //instances.Add(instance);
            //}

            //Selection.objects = instances.ToArray();
            //SceneView.RepaintAll();
        }
    }
}