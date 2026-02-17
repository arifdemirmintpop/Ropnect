using tiplay;
using UnityEngine;
#if TIPLAY_ENABLE_SDK && TIPLAY_GAMEANALYTICS
using GameAnalyticsSDK;
#endif

public class GameAnalyticsInitialize : MonoBehaviour                            //GA SDK eklendikten sonra yorum satırları kaldırılıp loading scene'e GameAnalyticsManager prefabı atılmalı.
{
#if TIPLAY_ENABLE_SDK && TIPLAY_GAMEANALYTICS
    private void Awake()
    {
        Instantiate(GlobalData.GameAnalyticsPrefab);
    }

    void Start()
    {
        GameAnalytics.Initialize();
    }
#endif
}