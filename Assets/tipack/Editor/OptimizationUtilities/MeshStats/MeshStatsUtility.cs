using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace tiplay.Toolkit.OptimizationToolkit
{

    public static class MeshStatsUtility
    {
        public static MeshStatsData[] GetAllMeshStats()
        {
            Dictionary<Mesh, List<GameObject>> meshCloneDict = FindMeshes();

            int indices = 0;
            MeshStatsData[] meshStatsArray = new MeshStatsData[meshCloneDict.Count];
            foreach (var item in meshCloneDict)
            {
                meshStatsArray[indices] = new MeshStatsData()
                {
                    clones = meshCloneDict[item.Key].ToArray(),
                    materialCount = item.Key.subMeshCount,
                    sharedMesh = item.Key,
                    trisCount = item.Key.triangles.Length,
                    vertexCount = item.Key.vertexCount,
                    batches = meshCloneDict[item.Key].Count * item.Key.subMeshCount
                };

                indices++;
            }

            return meshStatsArray;
        }

        private static Dictionary<Mesh, List<GameObject>> FindMeshes()
        {
            Dictionary<Mesh, List<GameObject>> meshCloneDict = new Dictionary<Mesh, List<GameObject>>();

            void AddToMeshCloneDictionary(GameObject gameObject, Mesh mesh)
            {
                if (!mesh)
                    return;

                if (!meshCloneDict.ContainsKey(mesh))
                    meshCloneDict.Add(mesh, new List<GameObject>());

                meshCloneDict[mesh].Add(gameObject);
            }

            MeshFilter[] meshFilters = GameObject.FindObjectsOfType<MeshFilter>(true);
            foreach (var meshFilter in meshFilters)
                AddToMeshCloneDictionary(meshFilter.gameObject, meshFilter.sharedMesh);

            SkinnedMeshRenderer[] skinnedMeshes = GameObject.FindObjectsOfType<SkinnedMeshRenderer>(true);
            foreach (var skinnedMesh in skinnedMeshes)
                AddToMeshCloneDictionary(skinnedMesh.gameObject, skinnedMesh.sharedMesh);

            return meshCloneDict;
        }
    }
}