using UnityEngine;
using UnityEditor;
using System;
using tiplay.EditorUtilities;

namespace tiplay.Toolkit.OptimizationToolkit
{
    public static class MaterialStatsEditorGUI
    {
        public static void DrawMaterialStatsOnGUI(MaterialStatsData[] materialStatsData)
        {
            Array.ForEach(materialStatsData, DrawMaterialStatsOnGUI);
        }

        private static void DrawMaterialStatsOnGUI(MaterialStatsData materialStats)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(materialStats.isExpandedMeshDetails ? "Hide" : "Show", GUILayout.Width(50)))
                        materialStats.isExpandedMeshDetails = !materialStats.isExpandedMeshDetails;

                    DrawMaterialField(materialStats.material);

                    GUILayout.Label($"( Clone Count: {materialStats.totalUsingCount} )", GUILayout.ExpandWidth(false));

                    DrawGPUInstancingField(materialStats.material);
                }

                DrawMaterialTexturesGUI(materialStats);
                DrawLinkedMeshDetailsGUI(materialStats);
            }
        }

        private static void DrawMaterialField(Material material)
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField(material, typeof(Material), false, GUILayout.Width(150));
            GUI.enabled = true;
        }

        private static void DrawLinkedMeshDetailsGUI(MaterialStatsData materialStats)
        {
            if (!materialStats.isExpandedMeshDetails)
                return;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                foreach (var item in materialStats.linkedMeshes)
                    DrawLinkedMeshGUI(item.Key, item.Value);
            }
        }

        private static void DrawLinkedMeshGUI(Mesh mesh, MaterialStatsData.MeshDetail detail)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                DrawMeshField(mesh);
                DrawMeshDetailsGUI(mesh, detail);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(15);

                using (new EditorGUILayout.VerticalScope())
                {
                    DrawLinkedObjectsGUI(detail);
                }
            }
        }

        private static void DrawMeshDetailsGUI(Mesh mesh, MaterialStatsData.MeshDetail detail)
        {
            GUILayout.Label($"( Clone Count: {detail.linkedObjects.Count} )", GUILayout.ExpandWidth(false));
            GUILayout.Label($"( Material Index: {detail.subMeshIndex} )", GUILayout.ExpandWidth(false));
            GUILayout.Label($"( Sub Mesh Vertex Count: {mesh.GetSubMesh(detail.subMeshIndex).vertexCount} )", GUILayout.ExpandWidth(false));
        }

        private static void DrawMeshField(Mesh mesh)
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField(mesh, typeof(Mesh), false, GUILayout.Width(150));
            GUI.enabled = true;
        }

        private static void DrawLinkedObjectsGUI(MaterialStatsData.MeshDetail detail)
        {
            foreach (var linkedObject in detail.linkedObjects)
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField(linkedObject, typeof(GameObject), false, GUILayout.Width(135));
                GUI.enabled = true;
            }
        }

        private static void DrawMaterialTexturesGUI(MaterialStatsData materialStats)
        {
            if (materialStats.textures.Count == 0)
                return;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                materialStats.isExpandedTextureDetails = EditorGUILayout.Foldout(materialStats.isExpandedTextureDetails, "Texture Details");

                if (!materialStats.isExpandedTextureDetails)
                    return;

                foreach (var textureItem in materialStats.textures)
                    DrawTextureFieldGUI(textureItem.Key, textureItem.Value);
            }
        }

        private static void DrawTextureFieldGUI(string textureName, Texture texture)
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField(texture, typeof(Texture), false, GUILayout.Width(150));
            GUI.enabled = true;
        }

        private static void DrawGPUInstancingField(Material material)
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                bool instancing = material.enableInstancing;

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("( GPU Instancing", GUILayout.ExpandWidth(false));
                    instancing = EditorGUILayout.Toggle(instancing, GUILayout.Width(10));
                    GUILayout.Label(")", GUILayout.ExpandWidth(false));
                }

                if (!scope.changed)
                    return;

                GPUInstancingEditorUtility.SetGPUInstancing(material, instancing);
            }
        }
    }
}