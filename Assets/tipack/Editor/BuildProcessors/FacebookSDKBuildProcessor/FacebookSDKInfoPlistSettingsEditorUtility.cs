using UnityEditor;
using UnityEngine;

namespace tiplay.BuildProcessors
{

    public class FacebookSDKInfoPlistSettingsEditorUtility
    {
        private static bool isExpanded = false;

        public static void DrawProcessorSettings()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                isExpanded = EditorGUILayout.Foldout(isExpanded, "info.plist Processor", true);

                if (!isExpanded)
                    return;

                FacebookSDKInfoPlistProcessorData.ProcessorEnabled = EditorGUILayout.Toggle("Processor Enabled", FacebookSDKInfoPlistProcessorData.ProcessorEnabled);

                if (!FacebookSDKInfoPlistProcessorData.ProcessorEnabled)
                    return;

                DrawDisplayNameGUI();
                DrawClientTokenGUI();
            }
        }

        private static void DrawClientTokenGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Client Token", GUILayout.Width(100));

                bool guiEnabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUILayout.TextField(FacebookSDKInfoPlistProcessorData.ClientToken);
                GUI.enabled = guiEnabled;
            }
        }

        private static void DrawDisplayNameGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Display Name", GUILayout.Width(100));

                bool guiEnabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUILayout.TextField(FacebookSDKInfoPlistProcessorData.DisplayName);
                GUI.enabled = guiEnabled;
            }
        }
    }
}

