using UnityEngine;
using UnityEditor;
using System;

namespace tiplay.Toolkit.OptimizationToolkit
{
    public static class MeshStatsEditorGUI
    {
        public static void DrawAllMeshStatsOnGUI(MeshStatsData[] meshStatsArray)
        {
            Array.ForEach(meshStatsArray, DrawMeshStatsOnGUI);
        }

        private static void DrawMeshStatsOnGUI(MeshStatsData meshStats)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    DrawMeshOnGUI(meshStats.sharedMesh);
                    DrawMeshDetailsOnGUI(meshStats);
                }

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    DrawGameObjectDetailsHeaderOnGUI(meshStats);

                    if (!meshStats.isExpanded)
                        return;

                    DrawGameObjectsOnGUI(meshStats.clones);
                }
            }
        }

        private static void DrawGameObjectDetailsHeaderOnGUI(MeshStatsData meshStats)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                meshStats.isExpanded = EditorGUILayout.Foldout(meshStats.isExpanded, "GameObjects");

                if (GUILayout.Button("Select All", GUILayout.ExpandWidth(false)))
                    Selection.objects = meshStats.clones;
            }
        }

        private static void DrawGameObjectsOnGUI(GameObject[] gameObjects)
        {
            GUI.enabled = false;

            foreach (var gameObject in gameObjects)
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), false);

            GUI.enabled = true;
        }

        private static void DrawMeshDetailsOnGUI(MeshStatsData meshStats)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label($"(Vertex: {meshStats.vertexCount})", GUILayout.ExpandWidth(false));
                GUILayout.Label($"(Tris: {meshStats.trisCount})", GUILayout.ExpandWidth(false));
                GUILayout.Label($"(Material Count: {meshStats.materialCount})", GUILayout.ExpandWidth(false));
                GUILayout.Label($"(Clone Count: {meshStats.clones.Length})", GUILayout.ExpandWidth(false));
                GUILayout.Label($"(Batches: {meshStats.batches} )", GUILayout.ExpandWidth(false));
            }
        }

        private static void DrawMeshOnGUI(Mesh mesh)
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField(mesh, typeof(Mesh), false, GUILayout.Width(150));
            GUI.enabled = true;
        }
    }
}