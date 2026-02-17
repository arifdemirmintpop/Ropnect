namespace tiplay.ScriptingDefines
{
    public static class ExternalDependencyScriptingDefineSymbol
    {
        static string symbol => "TIPLAY_EXTERNALDEPENDENCY";

#if TIPLAY_EXTERNALDEPENDENCY
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