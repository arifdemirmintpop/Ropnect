namespace tiplay.ScriptingDefines
{
    public static class TibugScriptingDefineSymbol
    {
        static string symbol => "TIPLAY_TIBUG";

#if TIPLAY_TIBUG
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
