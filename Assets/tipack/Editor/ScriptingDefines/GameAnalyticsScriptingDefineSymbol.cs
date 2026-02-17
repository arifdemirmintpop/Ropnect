namespace tiplay.ScriptingDefines
{
    public static class GameAnalyticsScriptingDefineSymbol
    {
        static string symbol => "TIPLAY_GAMEANALYTICS";

#if TIPLAY_GAMEANALYTICS
        public static bool IsDefined => true;
#else
        public static bool IsDefined => false;
#endif

        public static void Define()
        {
            ScriptingDefineSymbolManagement.AddDefineSymbol(symbol);
        }

        public static void Remove()
        {
            ScriptingDefineSymbolManagement.RemoveDefineSymbol(symbol);
        }
    }
}
