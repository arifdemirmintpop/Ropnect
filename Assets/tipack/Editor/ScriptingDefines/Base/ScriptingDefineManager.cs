using UnityEditor;
using System.Linq;

namespace tiplay.ScriptingDefines
{
    public abstract class ScriptingDefineManager
    {
        public abstract BuildTargetGroup buildTarget { get; }

        public void AddDefineSymbol(string symbol)
        {
            if (HasDefineSymbol(symbol))
                return;

            var symbols = GetDefineSymbols().ToList();
            symbols.Add(symbol);
            SetDefineSymbols(symbols.ToArray());
        }

        public bool RemoveDefineSymbol(string symbol)
        {
            var symbols = GetDefineSymbols().ToList();

            if (symbols.Remove(symbol))
            {
                SetDefineSymbols(symbols.ToArray());
                return true;
            }

            return false;
        }

        public bool HasDefineSymbol(string symbol) => ScriptingDefineSymbolManagement.HasDefineSymbol(symbol, buildTarget);

        private string[] GetDefineSymbols() => ScriptingDefineSymbolManagement.GetDefineSymbols(buildTarget);
        private void SetDefineSymbols(string[] defines) => ScriptingDefineSymbolManagement.SetDefineSymbols(defines, buildTarget);
    }
}
