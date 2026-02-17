using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;

namespace tiplay.Toolkit.OptimizationToolkit
{
    public class MeshStats : ITool
    {
        private MeshStatsData[] meshStatsArray;

        public string Title => "Mesh Stats";

        public string Shortcut => string.Empty;

        private void ReloadData()
        {
            meshStatsArray = MeshStatsUtility.GetAllMeshStats();
            meshStatsArray = meshStatsArray.OrderBy(meshStat => meshStat.batches).Reverse().ToArray();
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
            EditorGUILayout.HelpBox("Batches values are minimum values. It can be more depending on the light, shadow and post process settings on the scene.", MessageType.Info);

            if (GUILayout.Button("RELOAD"))
                ReloadData();

            MeshStatsEditorGUI.DrawAllMeshStatsOnGUI(meshStatsArray);
        }
    }
}

