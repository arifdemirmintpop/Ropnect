namespace tiplay.ScriptingDefines
{
    public static class ElephantCoreScriptingDefineSymbol
    {
        static string symbol => "TIPLAY_ELEPHANTCORE";

#if TIPLAY_ELEPHANTCORE
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
