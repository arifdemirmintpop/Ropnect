using UnityEditor;
using UnityEngine;
using System;

public class RenamePopupWindow : EditorWindow
{
    private string baseName = "Item";
    private Action<string> onConfirm;

    public static void Show(string title, Action<string> onConfirm)
    {
        var window = CreateInstance<RenamePopupWindow>();
        window.titleContent = new GUIContent(title);
        window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 250, 80);
        window.onConfirm = onConfirm;
        window.ShowUtility();
    }

    private void OnGUI()
    {
        GUILayout.Label("Base name for elements:", EditorStyles.boldLabel);
        baseName = EditorGUILayout.TextField(baseName);

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OK"))
        {
            onConfirm?.Invoke(baseName);
            Close();
        }
        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
        GUILayout.EndHorizontal();
    }
}
