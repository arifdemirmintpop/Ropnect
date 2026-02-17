using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace tiplay.Toolkit.DesignToolkit
{
    public class Measure : ITool
    {
        private bool sizes = true;
        private bool distance = true;

        public string Title => "Measure";

        public string Shortcut => string.Empty;

        public void OnCreate() { }

        public void OnDestroy() { }

        public void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            ShowDistance();
            ShowSizes(sceneView.camera.transform);
        }

        private void ShowSizes(Transform cameraTransform)
        {
            if (!sizes) return;

            Vector3 minOffset = new Vector3(.01f, 0f);
            Vector3 maxOffset = new Vector3(1, 2f);
            Vector3 cameraUp = cameraTransform.up;
            Vector3 cameraRight = cameraTransform.right;
            Vector3 cameraPosition = cameraTransform.position;

            float maxDistance = 50f;
            float distance;
            Vector3 offset;
            Vector3 position;
            Vector3 heightOffset;
            Vector3 widthOffset;
            Vector3 lengthOffset;
            Bounds bounds;

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                bounds = BoundsExtensions.GetBounds(Selection.transforms[i]);

                position = Selection.transforms[i].position;
                distance = Vector3.Distance(cameraPosition, position);

                if (distance > maxDistance)
                    continue;

                offset = Vector3.Lerp(minOffset, maxOffset, distance / maxDistance);
                heightOffset = cameraRight * offset.x;
                widthOffset = heightOffset + cameraUp * -offset.y;
                lengthOffset = heightOffset + cameraUp * -offset.y * 2f;

                Handles.Label(position + heightOffset, "Height: " + bounds.size.y.ToString("F4"));
                Handles.Label(position + widthOffset, "Width: " + bounds.size.x.ToString("F4"));
                Handles.Label(position + lengthOffset, "Length: " + bounds.size.z.ToString("F4"));
            }
        }

        private void ShowDistance()
        {
            if (!this.distance) return;
            if (!Selection.activeTransform) return;

            Vector3 startPos = Selection.activeTransform.position;
            Vector3 midPos;
            Vector3 endPos;
            float distance;

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                if (Selection.transforms[i] == Selection.activeTransform) continue;

                endPos = Selection.transforms[i].position;
                midPos = Vector3.Lerp(startPos, endPos, .5f);
                distance = Vector3.Distance(startPos, endPos);

                Handles.DrawDottedLine(startPos, endPos, 5);
                Handles.Label(midPos, distance.ToString());
            }
        }

        public void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        public void OnGUI()
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                sizes = EditorGUILayout.Toggle("Size", sizes);
                distance = EditorGUILayout.Toggle("Distance", distance);

                if (scope.changed)
                {
                    SceneView.RepaintAll();
                }
            }

        }
    }
}