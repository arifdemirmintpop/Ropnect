using UnityEditor;

namespace tiplay.ScriptingDefines
{
    public class IOSScriptingDefineManager : ScriptingDefineManager
    {
        public override BuildTargetGroup buildTarget => BuildTargetGroup.iOS;
    }
}
