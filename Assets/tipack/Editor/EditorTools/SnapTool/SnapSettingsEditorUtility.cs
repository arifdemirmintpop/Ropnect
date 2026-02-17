using UnityEngine;
using tiplay.EditorExtensions;
using UnityEditor;

namespace tiplay.SnapTool
{
    public static class SnapSettingsEditorUtility
    {
        private static float labelWidth = 150;

        public static void DrawSnapSettingsField(SnapSettings settings)
        {
            DrawRaycastSettingsGUI(settings);
            DrawSnapSettingsGUI(settings);
        }

        private static void DrawSnapSettingsGUI(SnapSettings settings)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                settings.snapSettingsExpanded = EditorGUILayout.Foldout(settings.snapSettingsExpanded, "Snap Settings", true);
                //settings.addBoundsOffset = EditorGUILayout.Toggle("Include Bounds Offset", settings.addBoundsOffset);

                if (settings.snapSettingsExpanded)
                {
                    DrawKeepRotationField(settings);
                    DrawPositionOffsetField(settings);
                    DrawRotationOffsetField(settings);
                }
            }
        }

        private static void DrawRaycastSettingsGUI(SnapSettings settings)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                settings.raySettingsExpanded = EditorGUILayout.Foldout(settings.raySettingsExpanded, "Raycast Settings", true);

                if (settings.raySettingsExpanded)
                {
                    DrawRayDirectionField(settings);
                    DrawLayerMaskField(settings);
                    DrawTriggerInteractionField(settings);
                }
            }
        }

        private static void DrawRotationOffsetField(SnapSettings settings)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Rotation Offset", GUILayout.Width(labelWidth));
                settings.rotationAngleOffset = EditorGUILayout.Vector3Field(GUIContent.none, settings.rotationAngleOffset);
            }
        }

        private static void DrawPositionOffsetField(SnapSettings settings)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Position Offset", GUILayout.Width(labelWidth));
                settings.positionOffset = EditorGUILayout.Vector3Field(GUIContent.none, settings.positionOffset);
            }
        }

        private static void DrawKeepRotationField(SnapSettings settings)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Keep Rotation", GUILayout.Width(labelWidth));
                settings.keepRotation = EditorGUILayout.Toggle(GUIContent.none, settings.keepRotation);
            }
        }

        private static void DrawTriggerInteractionField(SnapSettings settings)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Trigger Interaction", GUILayout.Width(labelWidth));
                settings.triggerInteraction = (QueryTriggerInteraction)EditorGUILayout.EnumPopup(GUIContent.none, settings.triggerInteraction);
            }
        }

        private static void DrawLayerMaskField(SnapSettings settings)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Layer Mask", GUILayout.Width(labelWidth));
                settings.layerMask = EditorGUIExtensions.LayerMaskField("", settings.layerMask);
            }
        }

        private static void DrawRayDirectionField(SnapSettings settings)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Ray Direction Space", GUILayout.Width(labelWidth));
                settings.raySpace = (Space)EditorGUILayout.EnumPopup(GUIContent.none, settings.raySpace);
            }
        }
    }
}