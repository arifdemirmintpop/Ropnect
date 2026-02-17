using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System;

namespace tiplay.Toolkit
{
    public abstract class ToolkitWindow : EditorWindow
    {
        private int columns => 3;
        private int toolIndices;
        private ITool currentTool => tools[toolIndices];
        private ITool[] tools;
        private Vector2 scroll;
        private List<ITool[]> toolbarTools = new List<ITool[]>();
        private List<string[]> toolbarToolNames = new List<string[]>();

        protected abstract ITool[] GetTools();

        protected virtual void ChangeTool(int indices)
        {
            if (indices < 0 || indices >= tools.Length)
                return;

            currentTool.OnDisable();
            toolIndices = indices;
            currentTool.OnEnable();
        }

        protected virtual void OnEnable()
        {
            tools = GetTools();

            InitializeToolbar();

            for (int i = 0; i < tools.Length; i++)
                tools[i].OnCreate();

            currentTool.OnEnable();
        }

        // Toolları Column sayısına göre gruplara ayırıp listede tutuyoruz
        private void InitializeToolbar()
        {
            int toolbarCount = Mathf.CeilToInt((float)tools.Length / columns);
            for (int i = 0; i < toolbarCount; i++)
            {
                int startIndex = i * columns;
                int endIndex = Mathf.Min(startIndex + columns, tools.Length);

                List<ITool> toolList = new List<ITool>();
                for (int toolIndices = startIndex; toolIndices < endIndex; toolIndices++)
                    toolList.Add(tools[toolIndices]);

                toolbarTools.Add(toolList.ToArray());
                toolbarToolNames.Add(toolList.Select(tool => tool.Title).ToArray());
            }
        }

        protected virtual void OnDisable()
        {
            for (int i = 0; i < tools.Length; i++)
                tools[i].OnDestroy();
        }

        protected virtual void OnGUI()
        {
            DrawToolsGUI();

            if (!HasAnyTool)
                return;

            DrawCurrentToolGUI();
        }

        private void DrawToolsGUI()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Tools", EditorStyles.boldLabel);

                for (int i = 0; i < toolbarTools.Count; i++)
                {
                    int startIndex = i * columns;
                    int endIndex = i * columns + columns;
                    int toolIndex = -1;

                    if (toolIndices >= startIndex && toolIndices < endIndex)
                        toolIndex = toolIndices % columns;

                    using (var scope = new EditorGUI.ChangeCheckScope())
                    {
                        toolIndex = GUILayout.Toolbar(toolIndex, toolbarToolNames[i]);

                        if (scope.changed)
                        {
                            ChangeTool(startIndex + toolIndex);
                        }
                    }
                }
            }
        }

        protected virtual void DrawCurrentToolGUI()
        {
            using (var scope = new EditorGUILayout.ScrollViewScope(scroll))
            {
                currentTool.OnGUI();
                scroll = scope.scrollPosition;
            }
        }

        private bool HasAnyTool => tools.Length > 0;
    }
}

