using UnityEngine;
using System.Collections;

namespace tiplay.GameToolkit
{
    [AddComponentMenu("")]
    public class ChronometerGUIText : MonoBehaviour
    {
        static ChronometerGUIText instance;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            instance = new GameObject("InGameChronometer").AddComponent<ChronometerGUIText>();
            DontDestroyOnLoad(instance);
        }
#endif

        GUIStyle textStyle = new GUIStyle();

        private void OnGUI()
        {
            if (!ChronometerSettingsData.DisplayInGame) return;

            textStyle.fontSize = ChronometerSettingsData.FontSize;
            textStyle.normal.textColor = ChronometerSettingsData.TextColor;
            textStyle.alignment = ChronometerSettingsData.TextAlignment;
            GUI.Label(new Rect(0, 0, Screen.width, 200), Chronometer.GetTime(), textStyle);
        }
    }
}

