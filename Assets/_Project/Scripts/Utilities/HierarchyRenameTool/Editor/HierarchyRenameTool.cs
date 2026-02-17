using UnityEditor;
using UnityEngine;
using System;

public static class HierarchyRenameTool
{
    private static double lastRunTime = -1;

    [MenuItem("GameObject/Rename Selected Objects", false, 0)]
    public static void RenameSelectedObjects()
    {
        if (EditorApplication.timeSinceStartup - lastRunTime < 0.5f)
            return;

        lastRunTime = EditorApplication.timeSinceStartup;

        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("There is no object selected");
            return;
        }

        RenameInputPopup.ShowPopup(selectedObjects);
    }
}
