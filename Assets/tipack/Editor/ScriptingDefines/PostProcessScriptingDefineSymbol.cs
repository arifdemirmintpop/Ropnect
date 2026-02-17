using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEditor.PackageManager;

namespace tiplay.ScriptingDefines
{
    public static class PostProcessScriptingDefineSymbol
    {
        static string packageId => "com.unity.postprocessing";
        static string symbol => "TIPLAY_POSTPROCESSING";

        [InitializeOnLoadMethod]
        static void Init()
        {
            PackageManagerController.OnPackageInstalled -= OnPackageInstalled;
            PackageManagerController.OnPackageInstalled += OnPackageInstalled;

            PackageManagerController.OnPackageRemoving -= OnPackageRemoved;
            PackageManagerController.OnPackageRemoving += OnPackageRemoved;


#if !TIPLAY_POSTPROCESSING
            if (PackageManagerController.IsPackageInstalled(packageId))
                ScriptingDefineSymbolManagement.AddDefineSymbol(symbol);
#endif
        }

        private static void OnPackageRemoved(UnityEditor.PackageManager.PackageInfo package)
        {
            if (!package.packageId.StartsWith(packageId))
                return;

            ScriptingDefineSymbolManagement.RemoveDefineSymbol(symbol);
        }

        private static void OnPackageInstalled(UnityEditor.PackageManager.PackageInfo package)
        {
            if (!package.packageId.StartsWith(packageId))
                return;

            ScriptingDefineSymbolManagement.AddDefineSymbol(symbol);
        }
    }
}
