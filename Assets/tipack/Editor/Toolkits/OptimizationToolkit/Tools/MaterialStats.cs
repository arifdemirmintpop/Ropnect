using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using NUnit.Framework.Constraints;
using tiplay.EditorUtilities;

namespace tiplay.Toolkit.OptimizationToolkit
{
    public class MaterialStats : ITool
    {
        private int vertexCount;
        private int cloneCount;
        private bool toolsExpanded;
        private MaterialStatsData[] materialStatsArray;

        public string Title => "Material Stats";

        public string Shortcut => string.Empty;

        private void ReloadData()
        {
            materialStatsArray = MaterialStatsUtility.GetAllMaterialStats();
            materialStatsArray = materialStatsArray.OrderBy(materialStat => materialStat.totalUsingCount).Reverse().ToArray();
        }

        public void OnDestroy() { }

        public void OnCreate() { }

        public void OnEnable()
        {
            ReloadData();
        }

        public void OnDisable() { }

        public void OnGUI()
        {
            //EditorGUILayout.HelpBox("Unity, Sub Mesh Vertex Count değeri 256'nın üzerinde olan materyallerin GPU Instancing özelliğini aktif etmeyi öneriyor. Bu vertex count değerinin altında kalan ve sahnede çok fazla sayıda bulunan nesnelerde GPU Instancing aktif edilirse batches değerini düşürüyor fakat unity tavsiye etmiyor.", MessageType.Info, true);
            DrawGPUInstancingTool();

            GUILayout.Space(2);

            if (GUILayout.Button("RELOAD", GUILayout.Height(30)))
                ReloadData();

            GUILayout.Space(2);

            MaterialStatsEditorGUI.DrawMaterialStatsOnGUI(materialStatsArray);

        }

        void DrawGPUInstancingTool()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                toolsExpanded = EditorGUILayout.Foldout(toolsExpanded, "Material Tools", true);

                if (!toolsExpanded)
                    return;

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("If \"Clone Count\" is Larger Than ", GUILayout.ExpandWidth(false));
                    cloneCount = EditorGUILayout.IntField(cloneCount, GUILayout.ExpandWidth(true));

                    if (GUILayout.Button("ENABLE GPU INSTANCING"))
                    {
                        EnableGPUInstancing();
                    }
                }
            }
        }

        private void EnableGPUInstancing()
        {
            foreach (var materialStat in materialStatsArray)
            {
                if (materialStat.totalUsingCount > cloneCount)
                {
                    GPUInstancingEditorUtility.SetGPUInstancing(materialStat.material, true);
                }
            }
        }
    }
}

