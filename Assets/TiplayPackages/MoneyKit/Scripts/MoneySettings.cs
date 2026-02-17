using UnityEngine;

namespace tiplay.MoneyKit
{
    //[CreateAssetMenu(menuName = "Money Settings")]
    public class MoneySettings : ScriptableObject
    {
        [Tooltip("1000 or 1K")]
        [SerializeField] private bool abbreviations = true;
        [SerializeField] private string symbol = "$";

        [Header("Default Settings")]
        [SerializeField] private int defaultDecimals = 1;

        [Header("Thousand Settings")]
        [SerializeField] private int thousandDecimals = 1;
        [SerializeField] private string thousandSuffix = "K";

        [Header("Million Settings")]
        [SerializeField] private int millionDecimals = 2;
        [SerializeField] private string millionSuffix = "M";

        [Header("Billion Settings")]
        [SerializeField] private int billionDecimals = 2;
        [SerializeField] private string billionSuffix = "B";

        public static bool Abbreviations => GetInstance().abbreviations;
        public static string Symbol => GetInstance().symbol;

        public static int DefaultDecimals => GetInstance().defaultDecimals;

        public static int ThousandDecimals => GetInstance().thousandDecimals;
        public static string ThousandSuffix => GetInstance().thousandSuffix;

        public static int MillionDecimals => GetInstance().millionDecimals;
        public static string MillionSuffix => GetInstance().millionSuffix;

        public static int BillionDecimals => GetInstance().billionDecimals;
        public static string BillionSuffix => GetInstance().billionSuffix;

        private static MoneySettings instance;
        public static MoneySettings GetInstance()
        {
            instance ??= Resources.Load<MoneySettings>(nameof(MoneySettings));
            return instance;
        }
    }
}
