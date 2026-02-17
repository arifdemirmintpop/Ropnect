namespace tiplay.ScriptingDefines
{
    public static class ElephantScriptingDefineSymbol
    {
        static string symbol => "TIPLAY_ELEPHANT";

#if TIPLAY_ELEPHANT
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
