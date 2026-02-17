using System.Collections;
using System.Collections.Generic;
using tiplay;
using tiplay.DatabaseSystem;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;
    private Database database => GlobalData.GetInstance().database;
    private bool isInitialized;
    private long sessionStartTimeStamp;

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

    public void CreateSessionData()
    {
        if (database.UserEngagementDatabase.InstallTimestamp == 0)
        {
            database.UserEngagementDatabase.InstallTimestamp = DatabaseHelpers.TakeTimestamp();
            database.UserEngagementDatabase.TotalPlaytime = 0;
            SaveManager.SaveData(database);
        }
        sessionStartTimeStamp = DatabaseHelpers.TakeTimestamp();
        isInitialized = true;
        EventManager.OnSessionDataManaged?.Invoke();
    }

    private void OnApplicationFocus(bool focusStatus)
    {
        if (!isInitialized) return;
        if (focusStatus)
        {
            sessionStartTimeStamp = DatabaseHelpers.TakeTimestamp();
        }
        else if (!focusStatus)
        {
            var sessionPlayTime = DatabaseHelpers.TakeTimestamp() - sessionStartTimeStamp;
            database.UserEngagementDatabase.TotalPlaytime += sessionPlayTime;
            SaveManager.SaveData(database);
        }
    }
}
