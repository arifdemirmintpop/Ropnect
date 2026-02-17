using UnityEngine;
using System.Collections;

namespace tiplay
{
    public enum PostProcessStatus { Enable, Disable };

    //[CreateAssetMenu(menuName = "Build Settings Data")]
    public class BuildSettingsData : ScriptableObject
    {
        public int frameRate = 60;
        public PostProcessStatus postProcessEnabled = PostProcessStatus.Enable;
        public ShadowQuality shadowQuality = ShadowQuality.All;


#if UNITY_EDITOR
        public void SaveData()
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
        }
#endif
    }
}

