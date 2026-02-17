using UnityEditor;
using UnityEngine;
using System;

public static class ProjectRenameTool
{
    private static double lastRunTime = -1;

    [MenuItem("Assets/Rename Selected Assets", false, 100)]
    public static void RenameSelectedAssets()
    {
        // Çoklu çağrıyı engelle
        if (EditorApplication.timeSinceStartup - lastRunTime < 0.5f)
            return;

        lastRunTime = EditorApplication.timeSinceStartup;

        UnityEngine.Object[] selectedAssets = Selection.objects;

        if (selectedAssets.Length == 0)
        {
            Debug.LogWarning("No assets selected.");
            return;
        }

        RenameInputPopup.ShowPopupForAssets(selectedAssets);
    }

    [MenuItem("Assets/Rename Selected Assets", true)]
    public static bool ValidateMenu()
    {
        return Selection.objects.Length > 0;
    }
}
