using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using tiplay.EditorUtilities;

namespace tiplay.Toolkit.OptimizationToolkit
{
    public class ParticleStats : ITool
    {
        private enum MipmapStatus { Enable, Disable }

        private string searchPath;
        private string filename;
        private ParticleStatsData[] particleStatsArray = new ParticleStatsData[0];
        private HierarchicalParticle[] hierarchicalParticles = new HierarchicalParticle[0];
        private bool isFiltersExpanded;
        private bool isToolsExpanded;
        private int maxParticles = 100;
        private int maxParticlesFilter = 100;
        public string Title => "Particle Stats";

        public string Shortcut => string.Empty;

        private int GetReloadButtonHeight()
        {
            int height = 50;

            if (isFiltersExpanded)
                height += 60;

            if (isToolsExpanded)
                height += 20;

            return height / 2;
        }

        private void ReloadFromScene()
        {
            particleStatsArray = ParticleStatsUtility.GetAllParticleStatsFromActiveScene(maxParticlesFilter);
            hierarchicalParticles = ParticleStatsUtility.DoHierarchical(particleStatsArray);
        }

        private void ReloadFromAssets()
        {
            particleStatsArray = ParticleStatsUtility.GetAllParticleStats(searchPath, filename, maxParticlesFilter);
            hierarchicalParticles = ParticleStatsUtility.DoHierarchical(particleStatsArray);
        }

        public void OnDestroy() { }

        public void OnCreate() { }

        public void OnEnable()
        {
            ReloadFromAssets();
        }

        public void OnDisable() { }

        public void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    ToolsGUI();
                    FiltersGUI();
                }

                using (new EditorGUILayout.VerticalScope(GUILayout.Width(150)))
                {
                    if (GUILayout.Button("LOAD FROM SCENE", GUILayout.Width(150), GUILayout.Height(GetReloadButtonHeight())))
                        ReloadFromScene();

                    if (GUILayout.Button("LOAD FROM ASSETS", GUILayout.Width(150), GUILayout.Height(GetReloadButtonHeight())))
                        ReloadFromAssets();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("SELECT ALL", GUILayout.ExpandWidth(true), GUILayout.Height(25)))
                    SelectAllParticles(particleStatsArray);

                if (GUILayout.Button("DESELECT ALL", GUILayout.ExpandWidth(true), GUILayout.Height(25)))
                    DeselectAllParticles(particleStatsArray);
            }

            ParticleStatsEditorGUI.DrawParticleStats(hierarchicalParticles);
        }

        private void SelectAllParticles(ParticleStatsData[] particleStats)
        {
            foreach (var particleStat in particleStats)
            {
                particleStat.isSelected = true;
                SelectAllParticles(particleStat.childParticles);
            }
        }

        private void DeselectAllParticles(ParticleStatsData[] particleStats)
        {
            foreach (var particleStat in particleStats)
            {
                particleStat.isSelected = false;
                DeselectAllParticles(particleStat.childParticles);
            }
        }

        private void ToolsGUI()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                isToolsExpanded = EditorGUILayout.Foldout(isToolsExpanded, "Tools", true);

                if (!isToolsExpanded)
                    return;

                MaxParticlesToolGUI();
            }
        }

        private void MaxParticlesToolGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                maxParticles = EditorGUILayout.IntField("Max Particle Count", maxParticles);

                if (GUILayout.Button("APPLY TO SELECTIONS", GUILayout.Width(160)))
                    ApplyMaxParticlesChanges(particleStatsArray);
            }
        }

        private void ApplyMaxParticlesChanges(ParticleStatsData[] particleStats)
        {
            foreach (var particleStat in particleStats)
            {
                if (particleStat.isSelected)
                {
                    particleStat.SetParticleMaxCount(maxParticles);
                    particleStat.SaveParticle();
                }

                ApplyMaxParticlesChanges(particleStat.childParticles);
            }
        }

        private void FiltersGUI()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                isFiltersExpanded = EditorGUILayout.Foldout(isFiltersExpanded, "Search Filters", true);

                if (!isFiltersExpanded)
                    return;

                searchPath = EditorGUILayout.TextField("Directory", searchPath);
                filename = EditorGUILayout.TextField("Name", filename);
                maxParticlesFilter = EditorGUILayout.IntField("\"Max Particles\" Greater Than", maxParticlesFilter);
            }
        }
    }
}

