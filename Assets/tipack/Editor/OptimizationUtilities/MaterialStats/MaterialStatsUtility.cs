using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace tiplay.Toolkit.OptimizationToolkit
{

    public static class MaterialStatsUtility
    {
        public static MaterialStatsData[] GetAllMaterialStats()
        {
            Dictionary<Material, MaterialStatsData> materialDict = new Dictionary<Material, MaterialStatsData>();

            void AddToDictionary(Material material, Mesh mesh, int subMeshIndex, GameObject gameObject)
            {
                if (!material)
                    return;

                if (!mesh)
                    return;

                if (!materialDict.ContainsKey(material))
                {
                    materialDict.Add(material, new MaterialStatsData()
                    {
                        material = material,
                        textures = GetMaterialTextures(material),
                    });
                }

                if (!materialDict[material].linkedMeshes.ContainsKey(mesh))
                    materialDict[material].linkedMeshes.Add(mesh, new MaterialStatsData.MeshDetail() { subMeshIndex = subMeshIndex });

                materialDict[material].totalUsingCount++;
                materialDict[material].linkedMeshes[mesh].linkedObjects.Add(gameObject);
            }

            Renderer[] renderers = GameObject.FindObjectsOfType<Renderer>(true);

            foreach (var renderer in renderers)
            {
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    if (!renderer.TryGetComponent(out MeshFilter meshFilter))
                        continue;

                    AddToDictionary(renderer.sharedMaterials[i], meshFilter.sharedMesh, i, meshFilter.gameObject);
                }
            }

            return materialDict.Values.ToArray();
        }

        private static Dictionary<string, Texture> GetMaterialTextures(Material material)
        {
            Texture texture;
            Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
            string[] texturePropertyNames = material.GetTexturePropertyNames();
            for (int i = 0; i < texturePropertyNames.Length; i++)
            {
                texture = material.GetTexture(texturePropertyNames[i]);

                if (!texture) continue;

                textures.Add(texturePropertyNames[i], texture);
            }

            return textures;
        }
    }
}