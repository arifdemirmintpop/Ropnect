using UnityEditor;
using UnityEngine;
#if TIPLAY_FACEBOOKSDK
using Facebook.Unity.Editor;
using Facebook.Unity.Settings;
#endif

namespace tiplay.BuildProcessors
{
    //[CreateAssetMenu(menuName = "FBSDKInfoPlistProcessorData")]
    public class FacebookSDKInfoPlistProcessorData : ScriptableObject
    {
        [SerializeField] private bool processorEnabled = false;


#if TIPLAY_FACEBOOKSDK
        public static string DisplayName => FacebookSettings.AppLabels[0];
        public static string ClientToken => FacebookSettings.ClientToken;
#else
        public static string DisplayName => string.Empty;
        public static string ClientToken => string.Empty;
#endif

        public static bool ProcessorEnabled
        {
            get => GetInstance().processorEnabled;
            set => GetInstance().processorEnabled = value;
        }

        private static FacebookSDKInfoPlistProcessorData instance;
        private static FacebookSDKInfoPlistProcessorData GetInstance()
        {
            instance ??= Resources.Load<FacebookSDKInfoPlistProcessorData>(nameof(FacebookSDKInfoPlistProcessorData));
            return instance;
        }

        public static void LoadInstance()
        {
            GetInstance();
        }

        public static void SaveData()
        {
            EditorUtility.SetDirty(GetInstance());
            AssetDatabase.SaveAssetIfDirty(GetInstance());
        }
    }
}