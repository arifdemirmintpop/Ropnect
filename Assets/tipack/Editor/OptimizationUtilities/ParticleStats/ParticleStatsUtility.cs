using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using tiplay.EditorUtilities;
using UnityEditorCoroutineUtilities.Editor;
using UnityEditor.SceneManagement;

namespace tiplay.Toolkit.OptimizationToolkit
{

    public static class ParticleStatsUtility
    {
        public static HierarchicalParticle[] DoHierarchical(ParticleStatsData[] particleStats)
        {
            var hierarchicalData = particleStats
                .Where(particleStat => particleStat.isPrefab)
                .Select(particleStat => new HierarchicalParticle(particleStat))
                .ToList();

            HierarchicalParticle hierarchy;
            foreach (var particleStat in particleStats)
            {
                hierarchy = hierarchicalData.FirstOrDefault(item => item.source.particle == particleStat.sourcePrefab);

                if (hierarchy == null)
                {
                    hierarchicalData.Add(new HierarchicalParticle(particleStat));
                    continue;
                }

                if (particleStat.isPrefab) continue;

                hierarchy.instances.Add(particleStat);
            }

            return hierarchicalData.ToArray();
        }

        public static ParticleStatsData[] GetChildParticles(ParticleSystem particle)
        {
            List<ParticleStatsData> childParticles = new List<ParticleStatsData>();

            if (particle)
            {
                foreach (Transform transform in particle.transform)
                    if (transform.TryGetComponent(out ParticleSystem childParticle))
                        childParticles.Add(new ParticleStatsData(childParticle));
            }

            return childParticles.ToArray();
        }

        public static ParticleStatsData[] GetAllParticleStatsFromActiveScene(int maxParticlesFilter)
        {
            var particles = ParticleSystemEditorUtility.FindAllFromActiveScene(true);

            particles = particles.Where(IsRootParticle).ToArray();

            return GetParticleStats(particles, maxParticlesFilter);
        }

        private static bool IsRootParticle(ParticleSystem particle)
        {
            if (particle.transform.parent == null)
                return true;

            return particle.transform.parent.GetComponentInParent<ParticleSystem>(true) == null;
        }

        public static ParticleStatsData[] GetAllParticleStats(string searchFolder, string filename, int maxParticlesFilter)
        {
            string[] particleGuids = AssetDatabase.FindAssets($"t:Prefab {filename}", new string[] { "Assets/" + searchFolder });
            string[] particlePaths = new string[particleGuids.Length];
            for (int i = 0; i < particleGuids.Length; i++)
                particlePaths[i] = AssetDatabase.GUIDToAssetPath(particleGuids[i]);


            ParticleSystem[] particles = particlePaths
                .Select(AssetDatabase.LoadAssetAtPath<ParticleSystem>)
                .Where(particle => particle is ParticleSystem)
                .ToArray();
            return GetParticleStats(particles, maxParticlesFilter);
        }

        private static ParticleStatsData[] GetParticleStats(ParticleSystem[] particles, int maxParticlesFilter)
        {
            particles = particles.Where(particle => particle.main.maxParticles >= maxParticlesFilter).ToArray();
            return particles.Select(particle => new ParticleStatsData(particle)).ToArray();
        }
    }


}

