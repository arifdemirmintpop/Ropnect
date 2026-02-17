#if TIPLAY_GAMEANALYTICS
using GameAnalyticsSDK;
#endif
using tiplay.ScriptingDefines;
using UnityEditor;
using UnityEngine;

namespace tiplay.SDKManagement
{
    public class GameAnalyticsSDKManager : SDKManager
    {
        public override string Label => "Game Analytics";
        public override bool HasSettings => true;
        public override SDKStatusData StatusData => ProjectSDKData.GetInstance().GameAnalytics;
        public override bool IndependentSDK => true;
        public GameAnalyticsSDKManager(SDKWebResponse jsonData) : base(jsonData.GameAnalytics)
        {

        }

        public override void OnPackageInstalled()
        {
            GameAnalyticsScriptingDefineSymbol.Define();
        }

#if TIPLAY_GAMEANALYTICS
        [InitializeOnLoadMethod]
        private static void InitializeGameAnalytics()
        {
            if (GlobalData.GameAnalyticsPrefab)
                return;

            string gameAnalyticsPrefabPath = GameAnalytics.WhereIs("GameAnalytics.prefab", "Prefab");
            GlobalData.GetInstance().gameAnalyticsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(gameAnalyticsPrefabPath);
            GlobalData.SaveData();

            if (GlobalData.GameAnalyticsPrefab == null)
                Debug.LogError("GameAnalytics.prefab not found");
        }
#endif

        public override void OnPackageRemoved()
        {
            GameAnalyticsScriptingDefineSymbol.Remove();
        }

        public override void OpenSettings()
        {
#if TIPLAY_GAMEANALYTICS
            Selection.activeObject = GameAnalytics.SettingsGA;
#endif
        }
    }
}

