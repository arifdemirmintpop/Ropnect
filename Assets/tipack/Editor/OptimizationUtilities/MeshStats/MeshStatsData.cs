using UnityEngine;

namespace tiplay.Toolkit.OptimizationToolkit
{
    public class MeshStatsData
    {
        public bool isExpanded;
        public Mesh sharedMesh;
        public int materialCount;
        public int vertexCount;
        public int trisCount;
        public int batches;
        public GameObject[] clones;
    }
}