using UnityEditor;
using UnityEngine;

namespace tiplay.BuildProcessors
{
    //[CreateAssetMenu(menuName = "GameDataBuildProcessorData")]
    public class GameDataBuildProcessorData : ScriptableObject
    {
        [SerializeField] private bool resetGameData = false;

        private static GameDataBuildProcessorData instance;
        private static GameDataBuildProcessorData GetInstance()
        {
            instance ??= Resources.Load<GameDataBuildProcessorData>(nameof(GameDataBuildProcessorData));
            return instance;
        }

        public static bool ResetGameData
        {
            get => GetInstance().resetGameData;
            set => GetInstance().resetGameData = value;
        }

        public static void SaveData()
        {
            EditorUtility.SetDirty(GetInstance());
            AssetDatabase.SaveAssetIfDirty(GetInstance());
        }
    }
}