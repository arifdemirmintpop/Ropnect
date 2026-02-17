namespace tiplay.ScriptingDefines
{
    public static class FacebookSDKScriptingDefineSymbol
    {
        static string symbol => "TIPLAY_FACEBOOKSDK";

#if TIPLAY_FACEBOOKSDK
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
