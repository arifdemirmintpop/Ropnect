using UnityEditor;
using UnityEngine;

namespace tiplay
{
    //[CreateAssetMenu(menuName = "PodfileProcessorData")]
    public class PodfileProcessorData : ScriptableObject
    {
        [SerializeField] private bool processorEnabled = false;
        [SerializeField] private bool addStaticLinkage = true;
        [SerializeField] private bool autoRunPodInstall = true;

        private static PodfileProcessorData instance;
        private static PodfileProcessorData GetInstance()
        {
            instance ??= Resources.Load<PodfileProcessorData>(nameof(PodfileProcessorData));
            return instance;
        }

        public static bool ProcessorEnabled
        {
            get => GetInstance().processorEnabled;
            set => GetInstance().processorEnabled = value;
        }

        public static bool AddStaticLinkage
        {
            get => GetInstance().addStaticLinkage;
            set => GetInstance().addStaticLinkage = value;
        }

        public static bool AutoRunPodInstall
        {
            get => GetInstance().autoRunPodInstall;
            set => GetInstance().autoRunPodInstall = value;
        }

        public static void SaveData()
        {
            EditorUtility.SetDirty(GetInstance());
            AssetDatabase.SaveAssetIfDirty(GetInstance());
        }
    }
}