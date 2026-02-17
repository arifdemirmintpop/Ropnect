using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using tiplay;
using tiplay.DatabaseSystem;
using UnityEngine;
#if TIPLAY_ENABLE_SDK && TIPLAY_ELEPHANT
using ElephantSDK;
#endif

public class RemoteManager : MonoBehaviour
{
    public static RemoteManager Instance;
    private Database database => GlobalData.GetInstance().database;
    [SerializeField] private bool ignoreRemoteData;

    private void Awake()
    {
        CreateSingleton();
    }

    private void CreateSingleton()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void ReadRemoteData()
    {
        if (!ignoreRemoteData)
        {
            CheckSRDebuggerActivity();
            LoadLevelDatabase();
        }
        EventManager.OnRemoteDataRead?.Invoke();
    }

    private void CheckSRDebuggerActivity()
    {
#if TIPLAY_ENABLE_SDK && TIPLAY_ELEPHANT
        bool isEnable = RemoteConfig.GetInstance().GetBool("SRDebuggerActivity", false);
        if (!isEnable) return;
        SRDebug.Init();
#endif
    }

    private void LoadLevelDatabase()
    {
#if TIPLAY_ENABLE_SDK && TIPLAY_ELEPHANT
        if (RemoteConfig.GetInstance().Get("LevelDatabase") == null)
        {
            Debug.LogError("There is no Level Database on Elephant Remote. (Loading skipped.)");
            return;
        }

        JObject remoteLevelDatabase = JObject.Parse(RemoteConfig.GetInstance().Get("LevelDatabase"));
        if (remoteLevelDatabase.Property("LoopStartIndex") != null)
            database.LevelDatabase.LoopStartIndex = (int)remoteLevelDatabase["LoopStartIndex"];
        if (remoteLevelDatabase.Property("LevelOrder") != null)
        {
            JArray levelOrderArray = (JArray)remoteLevelDatabase["LevelOrder"];
            database.LevelDatabase.LevelOrder.Clear();
            for (int i = 0; i < levelOrderArray.Count; i++)
            {
                database.LevelDatabase.LevelOrder.Add((int)levelOrderArray[i]);
            }
        }

        if (remoteLevelDatabase.Property("Levels") != null)
        {
            JArray levelsArray = (JArray)remoteLevelDatabase["Levels"];
            for (int i = 0; i < database.LevelDatabase.Levels.Count; i++)
            {
                if (levelsArray.Count <= i) continue;
                JObject level = (JObject)remoteLevelDatabase["Levels"][i];
                if (level.Property("Difficulty") != null)
                    database.LevelDatabase.Levels[i].Difficulty = (LevelDifficulty)((int)remoteLevelDatabase["Levels"][i]["Difficulty"]);
                if (level.Property("IsSkippedAfterLoop") != null)
                    database.LevelDatabase.Levels[i].IsSkippedAfterLoop = (bool)remoteLevelDatabase["Levels"][i]["IsSkippedAfterLoop"];
                if (level.Property("Duration") != null)
                    database.LevelDatabase.Levels[i].Duration = (int)remoteLevelDatabase["Levels"][i]["Duration"];
                if (level.Property("MoveCount") != null)
                    database.LevelDatabase.Levels[i].MoveCount = (int)remoteLevelDatabase["Levels"][i]["MoveCount"];
                if (level.Property("SlotCount") != null)
                    database.LevelDatabase.Levels[i].SlotCount = (int)remoteLevelDatabase["Levels"][i]["SlotCount"];
            }
        }
        Debug.Log("Level Database Loaded");
#endif
    }
}
