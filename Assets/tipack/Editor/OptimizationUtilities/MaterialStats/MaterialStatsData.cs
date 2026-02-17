using UnityEngine;
using System.Collections.Generic;

namespace tiplay.Toolkit.OptimizationToolkit
{
    public class MaterialStatsData
    {
        public bool isExpandedTextureDetails;
        public bool isExpandedMeshDetails;
        public int totalUsingCount;
        public Material material;
        public Dictionary<Mesh, MeshDetail> linkedMeshes = new Dictionary<Mesh, MeshDetail>();
        public Dictionary<string, Texture> textures;

        public class MeshDetail
        {
            public int subMeshIndex;
            public List<GameObject> linkedObjects = new List<GameObject>();
        }
    }
}