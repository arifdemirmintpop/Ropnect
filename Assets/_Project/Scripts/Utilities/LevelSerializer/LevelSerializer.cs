using UnityEngine;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelSerializer : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public List<LevelObjectData> objects = new List<LevelObjectData>();
    }

    [System.Serializable]
    public class LevelObjectData
    {
        public string prefabName;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    public string fileName = "Level 1";

    // Runtime için kullanılan obje listesi
    private List<GameObject> levelObjects = new List<GameObject>();

    public string ExportLevelToJson()
    {
        LevelData levelData = new LevelData();

        foreach (Transform child in transform)
        {
            string prefabName = GetPrefabName(child.gameObject);

            if (string.IsNullOrEmpty(prefabName))
            {
                Debug.LogWarning("Prefab adı alınamadı: " + child.name);
                continue;
            }

            LevelObjectData lod = new LevelObjectData();
            lod.prefabName = prefabName;
            lod.position = child.position;
            lod.rotation = child.rotation;
            lod.scale = child.localScale;

            levelData.objects.Add(lod);
        }

        string json = JsonUtility.ToJson(levelData, prettyPrint: true);
        return json;
    }

    private string GetPrefabName(GameObject obj)
    {
#if UNITY_EDITOR
        string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
        if (!string.IsNullOrEmpty(path))
        {
            return Path.GetFileNameWithoutExtension(path);
        }
        else
        {
            return null;
        }
#else
        return obj.name.Replace("(Clone)", "").Trim(); // Runtime için
#endif
    }

#if UNITY_EDITOR
    private GameObject GetPrefabByName(string prefabName)
    {
        string[] guids = AssetDatabase.FindAssets(prefabName + " t:Prefab");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null && prefab.name == prefabName)
                return prefab;
        }
        return null;
    }
#else
    public GameObject GetPrefabByName(string prefabName)
    {
        // Prefabların Resources/Prefabs içinde olduğundan emin olun
        return Resources.Load<GameObject>("Prefabs/" + prefabName);
    }
#endif

    public void LoadLevelFromJson(string json)
    {
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);
        int jsonCount = levelData.objects.Count;
        int currentCount = transform.childCount;

#if UNITY_EDITOR
        Undo.RegisterCompleteObjectUndo(this, "Load Level JSON");
#endif

        // Liste güncellemesi (runtime için)
        levelObjects.Clear();
        foreach (Transform t in transform)
        {
            levelObjects.Add(t.gameObject);
        }

        int commonCount = Mathf.Min(jsonCount, currentCount);

        for (int i = 0; i < commonCount; i++)
        {
            LevelObjectData objData = levelData.objects[i];
            GameObject oldObj = transform.GetChild(i).gameObject;

#if UNITY_EDITOR
            GameObject prefab = GetPrefabByName(objData.prefabName);
            if (prefab != null)
            {
                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
                newObj.transform.position = objData.position;
                newObj.transform.rotation = objData.rotation;
                newObj.transform.localScale = objData.scale;

                if (oldObj != null)
                {
                    Undo.DestroyObjectImmediate(oldObj);
                }
            }
            else
            {
                Debug.LogWarning("Prefab bulunamadı: " + objData.prefabName);
            }
#else
            GameObject prefab = GetPrefabByName(objData.prefabName);
            if (prefab != null)
            {
                if (oldObj != null)
                    Destroy(oldObj);

                GameObject newObj = Instantiate(prefab, objData.position, objData.rotation, transform);
                newObj.transform.localScale = objData.scale;
                levelObjects[i] = newObj;
            }
#endif
        }

        if (jsonCount > currentCount)
        {
            for (int i = currentCount; i < jsonCount; i++)
            {
                LevelObjectData objData = levelData.objects[i];
#if UNITY_EDITOR
                GameObject prefab = GetPrefabByName(objData.prefabName);
                if (prefab != null)
                {
                    GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
                    newObj.transform.position = objData.position;
                    newObj.transform.rotation = objData.rotation;
                    newObj.transform.localScale = objData.scale;
                }
                else
                {
                    Debug.LogWarning("Prefab bulunamadı: " + objData.prefabName);
                }
#else
                GameObject prefab = GetPrefabByName(objData.prefabName);
                if (prefab != null)
                {
                    GameObject newObj = Instantiate(prefab, objData.position, objData.rotation, transform);
                    newObj.transform.localScale = objData.scale;
                    levelObjects.Add(newObj);
                }
#endif
            }
        }
        else if (currentCount > jsonCount)
        {
            for (int i = jsonCount; i < currentCount; i++)
            {
#if UNITY_EDITOR
                Undo.DestroyObjectImmediate(transform.GetChild(i).gameObject);
#else
                Destroy(levelObjects[i]);
#endif
            }
#if !UNITY_EDITOR
            levelObjects.RemoveRange(jsonCount, currentCount - jsonCount);
#endif
        }
    }

    public string LoadJsonFromFile()
    {
        // Build ortamında dosyalar readonly olabileceği için persistentDataPath kullanıldı
        string folderPath = Path.Combine(Application.persistentDataPath, "LevelSerializer/Data");
        string fullPath = Path.Combine(folderPath, fileName + ".txt");

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            Debug.Log("JSON loaded from: " + fullPath);
            return json;
        }
        else
        {
            Debug.LogError("File not found: " + fullPath);
            return null;
        }
    }

    public void SaveJsonToFile(string json)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "LevelSerializer/Data");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("LevelData klasörü oluşturuldu: " + folderPath);
        }

        string fullPath = Path.Combine(folderPath, fileName + ".txt");
        File.WriteAllText(fullPath, json);
        Debug.Log("JSON saved to: " + fullPath);
    }

    public void CleanScene()
    {
        // Editör API kullanılmadan doğrudan sahne temizliği
        foreach (Transform child in transform)
        {
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(child.gameObject);
#else
            Destroy(child.gameObject);
#endif
        }

#if !UNITY_EDITOR
        levelObjects.Clear();
#endif
    }
}
