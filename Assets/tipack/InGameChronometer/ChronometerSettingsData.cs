using UnityEngine;

namespace tiplay.GameToolkit
{
    //[CreateAssetMenu(menuName = "Chronometer Settings Data")]
    public class ChronometerSettingsData : ScriptableObject
    {
        private static ChronometerSettingsData instance;

        public bool displayInGame = false;
        public int fontSize = 32;
        public Color textColor = Color.white;
        public TextAnchor textAlignment = TextAnchor.MiddleCenter;

        public static bool DisplayInGame => GetInstance().displayInGame;
        public static int FontSize => GetInstance().fontSize;
        public static Color TextColor => GetInstance().textColor;
        public static TextAnchor TextAlignment => GetInstance().textAlignment;

        public static ChronometerSettingsData GetInstance()
        {
            instance ??= Resources.Load<ChronometerSettingsData>(nameof(ChronometerSettingsData));
            return instance;
        }

    }
}

