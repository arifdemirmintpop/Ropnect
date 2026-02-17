using UnityEditor;

namespace tiplay.ScriptingDefines
{
    public class StandaloneScriptingDefineManager : ScriptingDefineManager
    {
        public override BuildTargetGroup buildTarget => BuildTargetGroup.Standalone;
    }
}
