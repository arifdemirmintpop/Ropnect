using UnityEditor;
using System;
using UnityEngine;

namespace tiplay.ScriptingDefines
{

    public static class ScriptingDefineSymbolManagement
    {
        private static ScriptingDefineManager[] defineManagers = new ScriptingDefineManager[]
        {
            new AndroidScriptingDefineManager(),
            new IOSScriptingDefineManager(),
            new StandaloneScriptingDefineManager()
        };

        public static void AddDefineSymbol(string symbol)
        {
            foreach (var defineManager in defineManagers)
                defineManager.AddDefineSymbol(symbol);
        }

        public static void RemoveDefineSymbol(string symbol)
        {
            foreach (var defineManager in defineManagers)
                defineManager.RemoveDefineSymbol(symbol);
        }

        public static bool HasDefineSymbol(string symbol, BuildTargetGroup targetGroup)
        {
            return Array.IndexOf(GetDefineSymbols(targetGroup), symbol) >= 0;
        }

        public static void SetDefineSymbols(string[] defines, BuildTargetGroup targetGroup)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
            AssetDatabase.Refresh();
        }

        public static string[] GetDefineSymbols(BuildTargetGroup targetGroup)
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out string[] defines);
            return defines;
        }
    }
}
