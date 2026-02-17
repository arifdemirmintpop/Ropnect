using System.Linq;
using System.Reflection;
using tiplay.ScriptingDefines;
using UnityEditor;
using UnityEngine;

namespace tiplay.SDKManagement
{
    public class ElephantGameKitSDKManager : SDKManager
    {
        public override string Label => "Elephant Game Kit";

        public override SDKStatusData StatusData => ProjectSDKData.GetInstance().ElephantGameKit;

        public override bool HasSettings => false;

        public override bool IndependentSDK => true;

        public ElephantGameKitSDKManager(SDKWebResponse jsonData) : base(jsonData.ElephantGameKit)
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
            ElephantGameKitCoreUtility.onElephantCoreInstalled?.Invoke();
        }

        public override void OnPackageRemoved()
        {
            ElephantGameKitCoreUtility.onElephantCoreRemoved?.Invoke();
        }

        public override void OpenSettings()
        {
            ElephantGameKitEditorUtility.EditSettings();
        }
    }
}