using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using tiplay.RemoteExporterTool;
public static class DatabaseEditorUtility
{
    public static void SaveAsDefaultValues<T>(T data) where T : ScriptableObject
    {
        string defaultDataPath = Path.Combine(Application.dataPath, "Resources/");
        string defaultFilePath = defaultDataPath + data.name + ".json";
        string json = EditorJsonUtility.ToJson(data);
        if (!Directory.Exists(defaultDataPath))
            Directory.CreateDirectory(defaultDataPath);
        if (!File.Exists(defaultFilePath))
        {
            StreamWriter streamWriter = new StreamWriter(defaultFilePath);
            streamWriter.Close();
        }
        File.WriteAllText(defaultFilePath, json);
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void LoadDefaultValues<T>(T data, Action callback = null) where T : ScriptableObject
    {
        string defaultDataPath = Path.Combine(Application.dataPath, "Resources/");
        string defaultFilePath = defaultDataPath + data.name + ".json";
        if (!File.Exists(defaultFilePath))
        {
            Debug.LogError("Could not found Default Values");
            return;
        }
        string json = File.ReadAllText(defaultFilePath);
        EditorJsonUtility.FromJsonOverwrite(json, data);
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        callback?.Invoke();
    }

    public static void LoadBuildValues<T>(T data, T updaterData) where T : ScriptableObject
    {
        string defaultDataPath = Path.Combine(Application.dataPath, "Resources/");
        string defaultFilePath = defaultDataPath + data.name + ".json";
        if (!File.Exists(defaultFilePath))
        {
            Debug.LogError("Could not found Default Values");
            return;
        }
        string json = File.ReadAllText(defaultFilePath);
        EditorJsonUtility.FromJsonOverwrite(json, data);
        EditorJsonUtility.FromJsonOverwrite(json, updaterData);
        EditorUtility.SetDirty(data);
        EditorUtility.SetDirty(updaterData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static async void ExportJson<T>(T data, Action callback = null, string path = "Data") where T : ScriptableObject
    {
        RemoteData remoteData = new RemoteData();
        remoteData.variables.Add("game_version", Application.version);
        RemoteData scriptablesRemoteData = new RemoteData();
        bool hasRemoteVariables = await data.ExportRemoteVariables(scriptablesRemoteData, data);
        if (hasRemoteVariables)
            remoteData.variables.Add(data.GetType().Name, scriptablesRemoteData.variables);

        if (remoteData.variables.Count == 0) return;
        var originalSettings = JsonConvert.DefaultSettings;
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Converters = { new CustomFloatConverter() }
        };
        string json = JsonConvert.SerializeObject(remoteData, Formatting.Indented);
        JsonConvert.DefaultSettings = originalSettings;
        if (!Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + path + Path.DirectorySeparatorChar))
            Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + path + Path.DirectorySeparatorChar);
        File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + path + Path.DirectorySeparatorChar + "RemoteData.json", json); //(fullDataPath, json);
        Debug.Log("Done...");
#if UNITY_EDITOR
        AssetDatabase.Refresh();
        if (path == "Data")
            EditorUtility.RevealInFinder(Application.persistentDataPath + Path.DirectorySeparatorChar + path + Path.DirectorySeparatorChar + "RemoteData.json");
        SplitRemoteDataFile(callback, path);
#endif
    }

    private static void SplitRemoteDataFile(Action callback = null, string path = "Data")
    {
        string jsonText = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + path + Path.DirectorySeparatorChar + "RemoteData.json");
        JObject jsonData = JObject.Parse(jsonText);
        string defaultDataPath = Application.persistentDataPath + Path.DirectorySeparatorChar + path + Path.DirectorySeparatorChar + "ParsedFiles" + Path.DirectorySeparatorChar;
        if (Directory.Exists(defaultDataPath))
            Directory.Delete(defaultDataPath, true);
        Directory.CreateDirectory(defaultDataPath);

        if (jsonData["variables"].SelectToken("Database").SelectToken("LevelDatabase") != null)
        {
            File.WriteAllText(defaultDataPath + "LevelDatabase.json", (jsonData["variables"]["Database"]["LevelDatabase"]).ToString());
        }
        Debug.Log("JSON Parsed");
        callback?.Invoke();
    }
}