using System.Collections.Generic;
using System.IO;
using System.Linq;
using tiplay.EditorUtilities;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace tiplay.BuildProcessors
{
    public class GameDataBuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 9999;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!GameDataBuildProcessorData.ResetGameData)
                return;

            DatabaseEditorUtility.LoadBuildValues(GlobalData.Database, GlobalData.UpdaterDatabase);

            GlobalData.Database.UserEngagementDatabase.IsUpdatedUser = false;
            GlobalData.UpdaterDatabase.UserEngagementDatabase.IsUpdatedUser = false;
            GlobalData.Database.UserEngagementDatabase.InstallTimestamp = 0;
            GlobalData.UpdaterDatabase.UserEngagementDatabase.InstallTimestamp = 0;
            GlobalData.Database.UserEngagementDatabase.TotalPlaytime = 0;
            GlobalData.UpdaterDatabase.UserEngagementDatabase.TotalPlaytime = 0;

            GlobalData.Database.VersionDatabase.Version = Application.version.ToString();
            GlobalData.UpdaterDatabase.VersionDatabase.Version = Application.version.ToString();

            GlobalData.Database.VersionDatabase.BuildNumber = GetBuildNumber(report);
            GlobalData.UpdaterDatabase.VersionDatabase.BuildNumber = GetBuildNumber(report);

            EditorUtility.SetDirty(GlobalData.Database);
            EditorUtility.SetDirty(GlobalData.UpdaterDatabase);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private string GetBuildNumber(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.iOS)
                return PlayerSettings.iOS.buildNumber;
            else if (report.summary.platform == BuildTarget.Android)
                return PlayerSettings.Android.bundleVersionCode.ToString();
            else 
                return string.Empty;
        }
    }
}

