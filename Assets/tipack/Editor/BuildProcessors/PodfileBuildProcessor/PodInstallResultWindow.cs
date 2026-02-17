using UnityEngine;
using UnityEditor;

namespace tiplay
{
    public class PodInstallResultWindow : EditorWindow
    {
        private TerminalResponse response;

        public static void ShowWindow(TerminalResponse response)
        {
            var window = GetWindow<PodInstallResultWindow>(true, "Pod Install Result");
            window.minSize = new Vector2(600, 100);
            window.response = response;
        }

        private void OnGUI()
        {
            GUIStyle style = new GUIStyle();
            style.margin = new RectOffset(10, 10, 10, 10);
            style.wordWrap = true;
            style.fontSize = 12;

            style.normal.textColor = new Color(0.035f, .55f, 0.01f);
            GUILayout.Label(response.Result, style);
            GUILayout.Space(10);

            style.normal.textColor = new Color(1, .4f, 0);
            GUILayout.Label(response.Error, style);
        }
    }
}