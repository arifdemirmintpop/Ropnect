using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using tiplay.EditorUtilities;

namespace tiplay.EditorTools
{
    public class ReplaceEditor : EditorWindow
    {
        private static ReplaceEditor window;

        private GameObject prefab;

        [MenuItem("GameObject/Replace (⇧R) #R", true)]
        static bool OpenWindowValidity()
        {
            return Selection.transforms.Length > 0;
        }

        [MenuItem("GameObject/Replace (⇧R) #R", priority = 0)]
        static void OpenWindow()
        {
            window = GetWindow<ReplaceEditor>(true, "Replacer", true);
            window.maxSize = new Vector2(300, 55);
            window.minSize = new Vector2(300, 55);
        }

        [InitializeOnLoadMethod, InitializeOnEnterPlayMode]
        static void CloseWindow()
        {
            if (window == null)
                return;

            window.Close();
            window = null;
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(12);
                using (new EditorGUILayout.VerticalScope())
                {
                    DrawPrefabField();
                    DrawReplaceButton();
                }
                GUILayout.Space(10);
            }
        }

        private void DrawReplaceButton()
        {
            if (GUILayout.Button("Replace", GUILayout.Height(25)))
            {
                GameObject[] selections = Selection.transforms.Select(transform => transform.gameObject).ToArray();
                PrefabEditorUtility.ReplaceAll(selections, prefab);
                CloseWindow();
            }
        }

        private void DrawPrefabField()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Prefab", GUILayout.ExpandWidth(false));
                prefab = (GameObject)EditorGUILayout.ObjectField(prefab, typeof(GameObject), false);
            }
        }
    }
}
