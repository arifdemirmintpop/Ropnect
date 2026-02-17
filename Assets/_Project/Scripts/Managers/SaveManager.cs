using System;
using System.IO;
using tiplay.AudioKit;
using tiplay.HapticKit;
using UnityEngine;

public class SaveManager
{
    public static void SaveData<T>(T data) where T : ScriptableObject
    {
            string dataPath = Application.persistentDataPath + "/Data";
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);
            var json = JsonUtility.ToJson(data);
            File.WriteAllText(dataPath + Path.DirectorySeparatorChar + data.name + ".txt", json);
    }

    public static void LoadData<T>(T data) where T : ScriptableObject
    {
        string dataPath = Application.persistentDataPath + "/Data";
        string fullPath = dataPath + Path.DirectorySeparatorChar + data.name + ".txt";
        if (File.Exists(fullPath))
        {
            var json = File.ReadAllText(fullPath);
            JsonUtility.FromJsonOverwrite(json, data);
        }
        else
            SaveData(data);
        
        EventManager.OnDataLoaded?.Invoke();
    }
}