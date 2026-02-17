using UnityEngine;

namespace tiplay
{
    public static class ShadowInitializer
    {
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            InitializeShadows();
        }

        static void InitializeShadows()
        {
#if !UNITY_EDITOR
            QualitySettings.shadows = GlobalData.GetActiveGameSettings().shadowQuality;
#endif
        }
    }
}
