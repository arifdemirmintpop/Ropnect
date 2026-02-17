using tiplay.ScriptingDefines;

namespace tiplay.SDKManagement
{
    public class ExternalDependencySDKManager : SDKManager
    {
        public override string Label => "External Dependency";

        public override SDKStatusData StatusData => ProjectSDKData.GetInstance().ExternalDependency;

        public override bool HasSettings => false;

        public override bool IndependentSDK => false;

        public ExternalDependencySDKManager(SDKWebResponse jsonData) : base(jsonData.ExternalDependency)
        {

        }

        public ExternalDependencySDKManager(SDKData sdkData) : base(sdkData)
        {

        }

        public override void OnPackageInstalled()
        {
            ExternalDependencyScriptingDefineSymbol.Define();
        }

        public override void OnPackageRemoved()
        {
            ExternalDependencyScriptingDefineSymbol.Remove();
        }
    }
}

