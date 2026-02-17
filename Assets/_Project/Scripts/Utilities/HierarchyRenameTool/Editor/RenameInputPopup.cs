using UnityEditor;
using UnityEngine;

public class RenameInputPopup : EditorWindow
{
    private string inputText = "Object";
    private GameObject[] targetGameObjects;
    private UnityEngine.Object[] targetAssets;
    private bool isForAssets = false;

    public static void ShowPopup(GameObject[] selectedObjects)
    {
        var window = CreateInstance<RenameInputPopup>();
        window.titleContent = new GUIContent("Enter Base Name");
        window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 300, 100);
        window.targetGameObjects = selectedObjects;
        window.ShowUtility();
    }

    public static void ShowPopupForAssets(UnityEngine.Object[] selectedAssets)
    {
        var window = CreateInstance<RenameInputPopup>();
        window.titleContent = new GUIContent("Enter Base Name");
        window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 300, 100);
        window.targetAssets = selectedAssets;
        window.isForAssets = true;
        window.ShowUtility();
    }

    private void OnGUI()
    {
        GUILayout.Label("Base name for selected " + (isForAssets ? "assets:" : "objects:"), EditorStyles.label);
        inputText = EditorGUILayout.TextField(inputText);

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OK"))
        {
            string baseName = inputText;

            if (isForAssets && targetAssets != null)
            {
                var assetsToRename = targetAssets;
                EditorApplication.delayCall += () =>
                {
                    for (int i = 0; i < assetsToRename.Length; i++)
                    {
                        string path = AssetDatabase.GetAssetPath(assetsToRename[i]);
                        AssetDatabase.RenameAsset(path, $"{baseName}{i + 1}");
                    }

                    AssetDatabase.SaveAssets();
                    Debug.Log($"{assetsToRename.Length} assets renamed to '{baseName} #' format.");
                };
            }
            else if (targetGameObjects != null)
            {
                var gameObjectsToRename = targetGameObjects;
                EditorApplication.delayCall += () =>
                {
                    for (int i = 0; i < gameObjectsToRename.Length; i++)
                    {
                        Undo.RecordObject(gameObjectsToRename[i], "Rename Selected Objects");
                        gameObjectsToRename[i].name = $"{baseName}{i + 1}";
                        EditorUtility.SetDirty(gameObjectsToRename[i]);
                    }

                    Debug.Log($"{gameObjectsToRename.Length} objects renamed to '{baseName} #' format.");
                };
            }

            CloseWindow();
        }

        if (GUILayout.Button("Cancel"))
        {
            CloseWindow();
        }
        GUILayout.EndHorizontal();
    }

    private void CloseWindow()
    {
        Close();
        GUIUtility.ExitGUI();
    }
}
