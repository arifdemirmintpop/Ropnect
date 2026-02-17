using System.Linq;
using System.Reflection;
using tiplay.ScriptingDefines;
using UnityEditor;
using UnityEngine;

namespace tiplay.SDKManagement
{
    public class ElephantSDKManager : SDKManager
    {
        public override string Label => "Elephant";

        public override SDKStatusData StatusData => ProjectSDKData.GetInstance().Elephant;

        public override bool HasSettings => ElephantEditorUtility.IsElephantInstalled;

        public override bool IndependentSDK => false;

        public ElephantSDKManager(SDKWebResponse jsonData) : base(jsonData.Elephant)
        {

        }

        public override void OnGUI()
        {
#if TIPLAY_ELEPHANTCORE
            if (GUILayout.Button("Show Elephant Manager"))
                ElephantSdkManager.SdkManager.ShowSdkManager();
#endif
        }

        public override void OnPackageInstalled()
        {
            ElephantCoreUtility.onElephantCoreInstalled?.Invoke();
        }

        public override void OnPackageRemoved()
        {
            ElephantCoreUtility.onElephantCoreRemoved?.Invoke();
        }

        public override void OpenSettings()
        {
            ElephantEditorUtility.EditSettings();
        }
    }
}

