using System;
using tiplay.BuildProcessors;
using tiplay.ScriptingDefines;
using UnityEngine;
#if TIPLAY_FACEBOOKSDK
using Facebook.Unity.Editor;
using Facebook.Unity.Settings;
#endif

namespace tiplay.SDKManagement
{
    public class FacebookSDKManager : SDKManager
    {
        public override string Label => "Facebook";
        public override bool HasSettings => true;
        public override SDKStatusData StatusData => ProjectSDKData.GetInstance().Facebook;
        public override bool IndependentSDK => false;
        private FacebookSDKInfoPlistSettingsEditorUtility InfoPlistSettingsDrawer = new FacebookSDKInfoPlistSettingsEditorUtility();

#if TIPLAY_FACEBOOKSDK
        private FacebookSettings _settings;
        private FacebookSettings settings
        {
            get
            {
                _settings ??= Resources.Load<FacebookSettings>(nameof(FacebookSettings));
                return _settings;
            }
        }
#endif

        public FacebookSDKManager(SDKWebResponse jsonData) : base(jsonData.Facebook)
        {

        }

        public FacebookSDKManager(SDKData sdkData) : base(sdkData)
        {

        }

#if TIPLAY_FACEBOOKSDK
        public override void OpenSettings()
        {
            FacebookSettingsEditor.Edit();
        }
#endif

        public override void OnPackageInstalled()
        {
            FacebookSDKScriptingDefineSymbol.Define();
        }

        public override void OnPackageRemoved()
        {
            FacebookSDKScriptingDefineSymbol.Remove();
        }

        public override void OnGUI()
        {
            if (StatusData.IsInstalled)
            {
                FacebookSDKInfoPlistSettingsEditorUtility.DrawProcessorSettings();
            }
        }
    }
}

