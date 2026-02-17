using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

public static class ComponentFinder
{
#if UNITY_EDITOR
    public enum SearchMode
    {
        Scene,
        PrefabAssets
    }

    /// <summary>
    /// Finds all prefabs that contain a component with the specified name
    /// </summary>
    public static void FindPrefabsWithComponent(string componentName, out List<GameObject> foundPrefabs, out List<string> foundPrefabPaths)
    {
        foundPrefabs = new List<GameObject>();
        foundPrefabPaths = new List<string>();
        if (string.IsNullOrEmpty(componentName))
        {
            Debug.LogWarning("Component name is empty!");
            return;
        }
        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab")
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .ToArray();
        foreach (string prefabPath in prefabPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null)
            {
                if (HasComponent(prefab, componentName))
                {
                    foundPrefabs.Add(prefab);
                    foundPrefabPaths.Add(prefabPath);
                }
            }
        }
    }

    /// <summary>
    /// Finds all scene GameObjects that contain a component with the specified name
    /// </summary>
    public static void FindSceneObjectsWithComponent(string componentName, out List<GameObject> foundObjects, out List<string> foundObjectPaths)
    {
        foundObjects = new List<GameObject>();
        foundObjectPaths = new List<string>();
        if (string.IsNullOrEmpty(componentName))
        {
            Debug.LogWarning("Component name is empty!");
            return;
        }
        // Search all loaded scenes
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;
            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (var root in rootObjects)
            {
                foreach (var go in root.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject))
                {
                    if (HasComponent(go, componentName))
                    {
                        foundObjects.Add(go);
                        foundObjectPaths.Add(scene.name + "/" + GetGameObjectPath(go));
                    }
                }
            }
        }
    }

    private static string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform current = obj.transform;
        while (current.parent != null)
        {
            current = current.parent;
            path = current.name + "/" + path;
        }
        return path;
    }

    private static bool HasComponent(GameObject obj, string componentName)
    {
        Component[] allComponents = obj.GetComponentsInChildren<Component>(true);
        foreach (Component component in allComponents)
        {
            if (component != null && component.GetType().Name.Equals(componentName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    public static int RemoveComponentFromPrefab(GameObject prefab, string componentName)
    {
        int removedCount = 0;
        string prefabPath = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        if (prefabRoot == null)
            return 0;
        Undo.RegisterFullObjectHierarchyUndo(prefabRoot, $"Remove {componentName} from prefab");
        var allComponents = prefabRoot.GetComponentsInChildren<Component>(true);
        foreach (var comp in allComponents)
        {
            if (comp != null && comp.GetType().Name.Equals(componentName, System.StringComparison.OrdinalIgnoreCase))
            {
                Undo.DestroyObjectImmediate(comp);
                removedCount++;
            }
        }
        if (removedCount > 0)
        {
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
        }
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        return removedCount;
    }

    public static int RemoveComponentFromSceneObjects(List<GameObject> objects, string componentName)
    {
        int removedCount = 0;
        foreach (var go in objects)
        {
            var allComponents = go.GetComponentsInChildren<Component>(true);
            foreach (var comp in allComponents)
            {
                if (comp != null && comp.GetType().Name.Equals(componentName, System.StringComparison.OrdinalIgnoreCase))
                {
                    Undo.DestroyObjectImmediate(comp);
                    removedCount++;
                }
            }
        }
        return removedCount;
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(MonoScript))]
public class ComponentFinderEditor : Editor
{
    // Boş, artık kullanılmıyor.
}

public class ComponentFinderWindow : EditorWindow
{
    private string componentNameToSearch = "";
    private Vector2 scrollPosition;
    private List<GameObject> foundObjects = new List<GameObject>();
    private List<string> foundObjectPaths = new List<string>();
    private bool isSearching = false;
    private string searchStatus = "";
    private ComponentFinder.SearchMode _searchMode = ComponentFinder.SearchMode.PrefabAssets;
    private ComponentFinder.SearchMode searchMode
    {
        get => _searchMode;
        set
        {
            if (_searchMode != value)
            {
                _searchMode = value;
                ClearResults();
            }
        }
    }
    private string addComponentTypeName = "";

    [MenuItem("Tools/Component Finder")]
    public static void ShowWindow()
    {
        ComponentFinderWindow window = GetWindow<ComponentFinderWindow>("Component Finder");
        window.minSize = new Vector2(400, 500);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Component Finder", EditorStyles.largeLabel);
        EditorGUILayout.Space();

        // Search Section
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Search Settings", EditorStyles.boldLabel);

        componentNameToSearch = EditorGUILayout.TextField("Component Name", componentNameToSearch);
        searchMode = (ComponentFinder.SearchMode)EditorGUILayout.EnumPopup("Search Mode", searchMode);

        EditorGUILayout.BeginHorizontal();

        GUI.enabled = !isSearching && !string.IsNullOrEmpty(componentNameToSearch);
        if (GUILayout.Button("Search", GUILayout.Height(30)))
        {
            SearchObjects();
        }
        GUI.enabled = true;

        if (GUILayout.Button("Clear Results", GUILayout.Height(30)))
        {
            ClearResults();
        }

        EditorGUILayout.EndHorizontal();

        if (!string.IsNullOrEmpty(searchStatus))
        {
            EditorGUILayout.HelpBox(searchStatus, MessageType.Info);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // Results Section
        if (foundObjects.Count > 0)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Found {foundObjects.Count} {(searchMode == ComponentFinder.SearchMode.PrefabAssets ? "prefabs" : "scene objects")}", EditorStyles.boldLabel);

            // Add Component Section
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Add Component to All Found Objects", EditorStyles.boldLabel);
            addComponentTypeName = EditorGUILayout.TextField("Component Type Name", addComponentTypeName);
            GUI.enabled = !isSearching && !string.IsNullOrEmpty(addComponentTypeName);
            if (GUILayout.Button($"Add '{addComponentTypeName}' to All", GUILayout.Height(24)))
            {
                AddComponentToAllFoundObjects();
            }
            GUI.enabled = true;
            EditorGUILayout.Space();

            // Remove/Delete Buttons
            GUI.enabled = !isSearching && !string.IsNullOrEmpty(componentNameToSearch);
            if (searchMode == ComponentFinder.SearchMode.PrefabAssets)
            {
                if (GUILayout.Button($"Remove '{componentNameToSearch}' Component from All", GUILayout.Height(30)))
                {
                    RemoveComponentFromAllPrefabs();
                }
                if (GUILayout.Button($"Delete All Found Prefabs", GUILayout.Height(30)))
                {
                    DeleteAllFoundPrefabs();
                }
            }
            else if (searchMode == ComponentFinder.SearchMode.Scene)
            {
                if (GUILayout.Button($"Remove '{componentNameToSearch}' Component from All in Scene", GUILayout.Height(30)))
                {
                    RemoveComponentFromAllSceneObjects();
                }
                if (GUILayout.Button($"Delete All Found Scene Objects", GUILayout.Height(30)))
                {
                    DeleteAllFoundSceneObjects();
                }
            }
            GUI.enabled = true;
            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < foundObjects.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{(searchMode == ComponentFinder.SearchMode.PrefabAssets ? "Prefab" : "Object")} {i + 1}:", EditorStyles.boldLabel);
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeObject = foundObjects[i];
                    EditorGUIUtility.PingObject(foundObjects[i]);
                }
                if (searchMode == ComponentFinder.SearchMode.PrefabAssets && GUILayout.Button("Open", GUILayout.Width(60)))
                {
                    AssetDatabase.OpenAsset(foundObjects[i]);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.ObjectField(searchMode == ComponentFinder.SearchMode.PrefabAssets ? "Prefab" : "Object", foundObjects[i], typeof(GameObject), false);
                EditorGUILayout.LabelField($"Path: {foundObjectPaths[i]}", EditorStyles.miniLabel);

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        else if (!isSearching && !string.IsNullOrEmpty(searchStatus))
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("No objects found with the specified component.", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.EndVertical();
        }
    }

    private void SearchObjects()
    {
        if (string.IsNullOrEmpty(componentNameToSearch))
        {
            EditorUtility.DisplayDialog("Error", "Please enter a component name to search for.", "OK");
            return;
        }

        isSearching = true;
        searchStatus = "Searching...";
        Repaint();

        foundObjects.Clear();
        foundObjectPaths.Clear();

        try
        {
            if (searchMode == ComponentFinder.SearchMode.PrefabAssets)
            {
                ComponentFinder.FindPrefabsWithComponent(componentNameToSearch, out foundObjects, out foundObjectPaths);
                searchStatus = $"Search completed! Found {foundObjects.Count} prefabs containing component '{componentNameToSearch}'";
            }
            else if (searchMode == ComponentFinder.SearchMode.Scene)
            {
                ComponentFinder.FindSceneObjectsWithComponent(componentNameToSearch, out foundObjects, out foundObjectPaths);
                searchStatus = $"Search completed! Found {foundObjects.Count} scene objects containing component '{componentNameToSearch}'";
            }
        }
        catch (System.Exception e)
        {
            searchStatus = $"Error during search: {e.Message}";
            Debug.LogError($"ComponentFinder search error: {e}");
        }
        finally
        {
            isSearching = false;
        }

        Repaint();
    }

    private void RemoveComponentFromAllPrefabs()
    {
        if (foundObjects.Count == 0 || string.IsNullOrEmpty(componentNameToSearch))
        {
            EditorUtility.DisplayDialog("Warning", "No prefab or component to remove.", "OK");
            return;
        }
        int totalRemoved = 0;
        int totalPrefabs = 0;
        for (int i = 0; i < foundObjects.Count; i++)
        {
            int removed = ComponentFinder.RemoveComponentFromPrefab(foundObjects[i], componentNameToSearch);
            if (removed > 0)
            {
                totalRemoved += removed;
                totalPrefabs++;
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Operation Complete", $"Removed '{componentNameToSearch}' component from {totalPrefabs} prefab(s), total {totalRemoved} component(s) removed.", "OK");
        SearchObjects();
    }

    private void RemoveComponentFromAllSceneObjects()
    {
        if (foundObjects.Count == 0 || string.IsNullOrEmpty(componentNameToSearch))
        {
            EditorUtility.DisplayDialog("Warning", "No object or component to remove.", "OK");
            return;
        }
        int totalRemoved = ComponentFinder.RemoveComponentFromSceneObjects(foundObjects, componentNameToSearch);
        EditorSceneManager.MarkAllScenesDirty();
        EditorUtility.DisplayDialog("Operation Complete", $"Removed '{componentNameToSearch}' component from scene objects. Total {totalRemoved} component(s) removed.", "OK");
        SearchObjects();
    }

    private void DeleteAllFoundPrefabs()
    {
        if (foundObjects.Count == 0)
        {
            EditorUtility.DisplayDialog("Warning", "No prefabs to delete.", "OK");
            return;
        }
        if (!EditorUtility.DisplayDialog("Delete All Prefabs?", $"Are you sure you want to delete all {foundObjects.Count} found prefabs? This cannot be undone!", "Delete", "Cancel"))
        {
            return;
        }
        int deleted = 0;
        for (int i = 0; i < foundObjects.Count; i++)
        {
            string path = AssetDatabase.GetAssetPath(foundObjects[i]);
            if (!string.IsNullOrEmpty(path) && AssetDatabase.DeleteAsset(path))
            {
                deleted++;
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Operation Complete", $"Deleted {deleted} prefab(s).", "OK");
        SearchObjects();
    }

    private void DeleteAllFoundSceneObjects()
    {
        if (foundObjects.Count == 0)
        {
            EditorUtility.DisplayDialog("Warning", "No scene objects to delete.", "OK");
            return;
        }
        if (!EditorUtility.DisplayDialog("Delete All Scene Objects?", $"Are you sure you want to delete all {foundObjects.Count} found scene objects? This cannot be undone!", "Delete", "Cancel"))
        {
            return;
        }
        int deleted = 0;
        foreach (var go in foundObjects)
        {
            if (go != null)
            {
                Undo.DestroyObjectImmediate(go);
                deleted++;
            }
        }
        EditorSceneManager.MarkAllScenesDirty();
        EditorUtility.DisplayDialog("Operation Complete", $"Deleted {deleted} scene object(s).", "OK");
        SearchObjects();
    }

    private void ClearResults()
    {
        foundObjects.Clear();
        foundObjectPaths.Clear();
        searchStatus = "";
        Repaint();
    }

    private void AddComponentToAllFoundObjects()
    {
        if (foundObjects.Count == 0 || string.IsNullOrEmpty(addComponentTypeName))
        {
            EditorUtility.DisplayDialog("Warning", "No objects or component type specified.", "OK");
            return;
        }
        int added = 0;
        if (searchMode == ComponentFinder.SearchMode.PrefabAssets)
        {
            for (int i = 0; i < foundObjects.Count; i++)
            {
                string path = AssetDatabase.GetAssetPath(foundObjects[i]);
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
                if (prefabRoot == null) continue;
                Undo.RegisterFullObjectHierarchyUndo(prefabRoot, $"Add {addComponentTypeName} to prefab");
                var type = GetTypeByName(addComponentTypeName);
                if (type != null && prefabRoot.GetComponent(type) == null)
                {
                    prefabRoot.AddComponent(type);
                    added++;
                }
                foreach (var child in prefabRoot.GetComponentsInChildren<Transform>(true))
                {
                    if (type != null && child.gameObject.GetComponent(type) == null)
                    {
                        // Optionally add to children as well (comment out if not desired)
                        // child.gameObject.AddComponent(type);
                        // added++;
                    }
                }
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else if (searchMode == ComponentFinder.SearchMode.Scene)
        {
            var type = GetTypeByName(addComponentTypeName);
            foreach (var go in foundObjects)
            {
                if (go != null && type != null && go.GetComponent(type) == null)
                {
                    Undo.AddComponent(go, type);
                    added++;
                }
            }
        }
        if (added > 0)
        {
            EditorUtility.DisplayDialog("Operation Complete", $"Added '{addComponentTypeName}' to {added} object(s).", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Operation Complete", $"No components were added. (Maybe they already exist or type is invalid)", "OK");
        }
        SearchObjects();
    }

    private System.Type GetTypeByName(string typeName)
    {
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(typeName);
            if (type != null && typeof(Component).IsAssignableFrom(type))
                return type;
        }
        // Try Unity built-in types
        var unityType = System.Type.GetType(typeName);
        if (unityType != null && typeof(Component).IsAssignableFrom(unityType))
            return unityType;
        // Try with UnityEngine namespace
        unityType = System.Type.GetType("UnityEngine." + typeName + ", UnityEngine");
        if (unityType != null && typeof(Component).IsAssignableFrom(unityType))
            return unityType;
        return null;
    }
}
#endif
