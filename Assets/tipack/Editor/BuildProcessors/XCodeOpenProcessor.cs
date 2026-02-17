using UnityEngine;
using System.Collections;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using UnityEditor;
using tiplay.ScriptingDefines;

namespace tiplay.BuildProcessors
{
    public class XCodeOpenProcessors : IPostprocessBuildWithReport
    {
        int IOrderedCallback.callbackOrder => 10000;

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.iOS)
                return;

            string buildPath = report.summary.outputPath;
            string workspaceFilePath = buildPath + "/Unity-iPhone.xcworkspace";
            bool hasWorkspaceFile = Directory.Exists(workspaceFilePath) || File.Exists(workspaceFilePath);

            if (hasWorkspaceFile)
            {
                Terminal.RunCommand(buildPath, "open \"Unity-iPhone.xcworkspace\"");
                Debug.Log("Unity-iPhone.xcworkspace Opening");
                return;
            }

            if (SDKScriptingDefineSymbol.IsDefined)
            {
                Debug.LogError("Unity-iPhone.xcworkspace is not opening because the file not found");
                return;
            }

            Terminal.RunCommand(buildPath, "open \"Unity-iPhone.xcodeproj\"");
            Debug.Log("Unity-iPhone.xcodeproj Opening");
        }
    }
}