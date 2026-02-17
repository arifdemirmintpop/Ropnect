using UnityEditor;

namespace tiplay.ScriptingDefines
{
    public static class SDKScriptingDefineSymbol
    {
        static string symbol => "TIPLAY_ENABLE_SDK";

#if TIPLAY_ENABLE_SDK
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
