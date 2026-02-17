using UnityEngine;
using System.Collections;
using System;
using static UnityEditor.FilePathAttribute;
using UnityEditor;
using UnityEngine.UIElements;

namespace tiplay.SnapTool
{
    public static class SnapGizmoUtility
    {
        public static void DrawGizmo(Transform[] transforms, SnapSettings settings)
        {
            foreach (var transform in transforms)
                DrawGizmo(transform, settings);
        }

        public static void DrawGizmo(Transform transform, SnapSettings settings)
        {
            Vector3 pivot = transform.TransformPoint(-settings.positionOffset);
            Handles.color = Color.red;
            Handles.DrawWireDisc(pivot, transform.up, .1f);
            Handles.color = Color.green;
            Handles.DrawLine(pivot, pivot + transform.up * .1f);

            if (!SnapManager.GetSnapLocation(transform, settings, out Vector3 position, out Quaternion rotation))
                return;

            position -= rotation * settings.positionOffset;

            Handles.color = Color.white;
            Handles.DrawDottedLine(pivot, position, 5f);
            Handles.DrawWireDisc(position, rotation * Vector3.up, .25f);

            Handles.color = Color.green;
            Handles.DrawLine(position, position + rotation * Vector3.up * .4f);
            Handles.color = Color.blue;
            Handles.DrawLine(position, position + rotation * Vector3.forward * .4f);
            Handles.color = Color.red;
            Handles.DrawLine(position, position + rotation * Vector3.right * .4f);
        }
    }
}