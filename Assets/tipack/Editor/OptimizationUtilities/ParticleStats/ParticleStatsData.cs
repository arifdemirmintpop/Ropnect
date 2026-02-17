using UnityEngine;
using UnityEditor;
using tiplay.EditorUtilities;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

namespace tiplay.Toolkit.OptimizationToolkit
{
    public class ParticleStatsData
    {
        public bool isSelected;
        public bool isExpanded;
        public bool isPrefab;
        public string path;

        public ParticleSystem particle;
        public ParticleSystem sourcePrefab;

        public ParticleStatsData[] childParticles;

        public int ParticleMaxCount
        {
            get => particle.main.maxParticles;
        }

        public ParticleStatsData(ParticleSystem particle)
        {
            this.particle = particle;
            isPrefab = string.IsNullOrEmpty(particle.gameObject.scene.name);
            childParticles = ParticleStatsUtility.GetChildParticles(particle);

            if (isPrefab)
                sourcePrefab = particle;

            if (!isPrefab)
                sourcePrefab = PrefabEditorUtility.FindSourcePrefab(particle);
        }

        public void SetParticleMaxCount(int maxCount)
        {
            CreateUndo();
            ParticleSystem.MainModule main = particle.main;
            main.maxParticles = maxCount;
        }

        public void SaveParticle()
        {
            SavePrefabParticle();
            SaveInstancedParticle();
        }

        public void CreateUndo()
        {
            CreatePrefabParticleUndo();
            CreateInstancedParticleUndo();
        }

        private void CreateInstancedParticleUndo()
        {
            Undo.RegisterCompleteObjectUndo(particle, "Particle");
        }

        private void SaveInstancedParticle()
        {
            EditorUtility.SetDirty(particle.gameObject);
        }

        private void CreatePrefabParticleUndo()
        {
            if (!isPrefab) return;
            Undo.RegisterCompleteObjectUndo(particle, "Particle");
        }

        private void SavePrefabParticle()
        {
            if (!isPrefab) return;
            PrefabUtility.SavePrefabAsset(particle.transform.root.gameObject);
        }
    }


}

