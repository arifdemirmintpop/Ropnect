using tiplay.EditorUtilities;
using tiplay.ScriptingDefines;
using UnityEditor;
using UnityEngine;

namespace tiplay.SDKManagement
{
    public class TibugSDKManager : SDKManager
    {
        public override string Label => "Tibug";

        public override SDKStatusData StatusData => ProjectSDKData.GetInstance().Tibug;

        public override bool HasSettings => true;

        public override bool IndependentSDK => true;

        public TibugSDKManager(SDKWebResponse jsonData) : base(jsonData.Tibug)
        {

        }

#if TIPLAY_TIBUG
        [InitializeOnLoadMethod]
        private static void InitializeTibug()
        {
            if (GlobalData.TibugPrefab)
                return;

            GlobalData.GetInstance().tibugPrefab = TibugEditorUtility.GetPrefab();
            GlobalData.SaveData();

            if (GlobalData.TibugPrefab == null)
                Debug.LogError("Tibug.prefab not found");
        }
#endif

        public override void OpenSettings()
        {
            TibugEditorUtility.EditSettings();
        }

        public override void OnPackageInstalled()
        {
            TibugScriptingDefineSymbol.Define();
        }

        public override void OnPackageRemoved()
        {
            TibugScriptingDefineSymbol.Remove();
        }
    }
}

