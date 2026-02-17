using System.Collections;
using System.Collections.Generic;
using tiplay;
using tiplay.DatabaseSystem;
using UnityEngine;

public class SDKEventManager
{
    private static Database database => GlobalData.Database;

    public static void SendStartGame()
    {
#if TIPLAY_ENABLE_SDK && TIPLAY_ELEPHANT && !UNITY_EDITOR
        ElephantManager.SendPlayerStartsPlayLevel(database.LevelDatabase.LevelTextValue);      
#endif
#if TIPLAY_ENABLE_SDK && TIPLAY_GAMEANALYTICS && !UNITY_EDITOR
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "level", database.LevelDatabase.LevelTextValue);
#endif
#if TIPLAY_TIBUG && !UNITY_EDITOR
        TibugEventManager.startLevel.Invoke(database.LevelDatabase.LevelTextValue);
#endif
    }

    public static void SendWinGame(float time, int usedMoveCount)
    {
#if TIPLAY_ENABLE_SDK && TIPLAY_ELEPHANT && !UNITY_EDITOR
        ElephantManager.SendPlayerCompletesLevel(database.LevelDatabase.LevelTextValue, time, usedMoveCount);
#endif
#if TIPLAY_ENABLE_SDK && TIPLAY_GAMEANALYTICS && !UNITY_EDITOR
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level", database.LevelDatabase.LevelTextValue);
#endif
#if TIPLAY_TIBUG && !UNITY_EDITOR
        TibugEventManager.finishLevel.Invoke();
#endif
    }

    public static void SendFailGame()
    {
#if TIPLAY_ENABLE_SDK && TIPLAY_ELEPHANT && !UNITY_EDITOR
        ElephantManager.SendPlayerFailsLevel(database.LevelDatabase.LevelTextValue);
#endif
#if TIPLAY_ENABLE_SDK && TIPLAY_GAMEANALYTICS && !UNITY_EDITOR
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "level", database.LevelDatabase.LevelTextValue);
#endif
    }
}
