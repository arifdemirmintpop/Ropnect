using tiplay.EditorUtilities;
using tiplay.ScriptingDefines;
using tiplay.SDKManagement;
using UnityEditor;
using UnityEngine;

namespace tiplay.Toolkit.DeveloperToolkit
{
    public static class TibugActivationDrawer
    {
        private enum Status { Enable, Disable };
        private static Status status = Status.Disable;

        public static void DrawGUI()
        {
            if (!ProjectSDKData.GetInstance().Tibug.IsInstalled)
                return;

            using (new EditorGUILayout.HorizontalScope())
            {
                using (var scope = new EditorGUI.ChangeCheckScope())
                {
                    status = TibugScriptingDefineSymbol.IsDefined ? Status.Enable : Status.Disable;
                    status = (Status)EditorGUILayout.EnumPopup("Tibug Activation", status);

                    if (scope.changed)
                    {
                        if (status == Status.Enable)
                            TibugEditorUtility.EnableTibug();

                        if (status == Status.Disable)
                            TibugEditorUtility.DisableTibug();
                    }
                }
            }
        }
    }
}


