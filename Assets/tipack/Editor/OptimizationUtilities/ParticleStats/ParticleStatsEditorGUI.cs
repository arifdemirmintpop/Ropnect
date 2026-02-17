using UnityEngine;
using UnityEditor;
using System;
using tiplay.EditorUtilities;
using System.Linq;
using System.Collections.Generic;

namespace tiplay.Toolkit.OptimizationToolkit
{
    public static class ParticleStatsEditorGUI
    {
        public static void DrawParticleStats(HierarchicalParticle[] hierarchicalParticles)
        {
            Array.ForEach(hierarchicalParticles, DrawHierarchicalParticle);
        }

        public static void DrawParticleStats(ParticleStatsData[] particleStats)
        {
            Array.ForEach(particleStats, DrawParticleStatsData);
        }

        private static void DrawHierarchicalParticle(HierarchicalParticle hierarchy)
        {
            DrawParticleStatsData(hierarchy.source);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(25);
                using (new EditorGUILayout.VerticalScope())
                {
                    foreach (var instance in hierarchy.instances)
                        DrawParticleStatsData(instance);
                }
            }
        }

        private static void DrawParticleStatsData(ParticleStatsData particleStat)
        {
            if (!particleStat.particle)
                return;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label(particleStat.isPrefab ? "(Prefab)" : "(Instance)", EditorStyles.boldLabel);
                DrawParticle(particleStat);

                DrawSubParticles(particleStat);
            }
        }

        private static void DrawParticle(ParticleStatsData particleStat)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                DrawSelectBox(particleStat);
                DrawParticleField(particleStat.particle);
                DrawMaxParticlesField(particleStat);
            }
        }

        private static void DrawSubParticles(ParticleStatsData particleStat)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(20);
                using (new EditorGUILayout.VerticalScope())
                {
                    foreach (var childParticle in particleStat.childParticles)
                    {
                        DrawParticle(childParticle);
                        DrawSubParticles(childParticle);
                    }
                }
            }
        }

        private static void DrawSelectBox(ParticleStatsData particleStat)
        {
            particleStat.isSelected = EditorGUILayout.Toggle(particleStat.isSelected, GUILayout.Width(15));
        }

        private static void DrawMaxParticlesField(ParticleStatsData particleStat)
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.HorizontalScope(GUILayout.Width(200)))
                {
                    GUILayout.Label("( Max Particles", GUILayout.ExpandWidth(false));
                    int maxParticles = EditorGUILayout.IntField(particleStat.ParticleMaxCount, GUILayout.MaxWidth(60));
                    GUILayout.Label(")", GUILayout.ExpandWidth(false));

                    if (scope.changed)
                    {
                        maxParticles = Mathf.Max(0, maxParticles);
                        particleStat.SetParticleMaxCount(maxParticles);
                        particleStat.SaveParticle();
                    }
                }

            }
        }

        private static void DrawParticleField(ParticleSystem particle)
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField(particle, typeof(ParticleSystem), false, GUILayout.Width(200));
            GUI.enabled = true;
        }
    }


}

