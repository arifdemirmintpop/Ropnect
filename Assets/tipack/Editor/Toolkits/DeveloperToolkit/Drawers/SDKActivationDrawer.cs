using tiplay.ScriptingDefines;
using UnityEditor;
using UnityEngine;

namespace tiplay.Toolkit.DeveloperToolkit
{
    public static class SDKActivationDrawer
    {
        private enum Status { Enable, Disable };
        private static Status status = Status.Disable;

        public static void DrawGUI()
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                status = SDKScriptingDefineSymbol.IsDefined ? Status.Enable : Status.Disable;
                status = (Status)EditorGUILayout.EnumPopup("SDK Activation", status);

                if (!scope.changed) return;

                if (status == Status.Enable)
                    SDKScriptingDefineSymbol.Define();

                if (status == Status.Disable)
                    SDKScriptingDefineSymbol.Remove();
            }
        }
    }
}


