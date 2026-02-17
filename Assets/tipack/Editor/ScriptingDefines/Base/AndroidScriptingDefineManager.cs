using UnityEditor;

namespace tiplay.ScriptingDefines
{
    public class AndroidScriptingDefineManager : ScriptingDefineManager
    {
        public override BuildTargetGroup buildTarget => BuildTargetGroup.Android;
    }
}
