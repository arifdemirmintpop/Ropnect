using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace tiplay.BuildProcessors
{
    public class FacebookSDKInfoPlistProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => 9998;

        public void OnPreprocessBuild(BuildReport report)
        {
            FacebookSDKInfoPlistProcessorData.LoadInstance();
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.iOS)
                return;

            if (!FacebookSDKInfoPlistProcessorData.ProcessorEnabled)
                return;

            InitInfoPlist(report.summary.outputPath);
        }

        /// <summary>
        /// IOS Buildde oluşturulan info.plist'e FacebookDisplayName ve FacebookClientToken satırlarını yazdırır
        /// </summary>
        /// <param name="buildPath">Build klasör yolu</param>
        private static void InitInfoPlist(string buildPath)
        {
            string infoPlistPath = buildPath + "/Info.plist";
            List<string> infoPlistLines = File.ReadAllLines(infoPlistPath).ToList();

            // Info plist'in dictionary bloğunun başlangıç satırını buluyoruz
            int dictStartIndice = 0;
            for (int i = 0; i < infoPlistLines.Count; i++)
            {
                if (infoPlistLines[i].Contains("<dict>"))
                {
                    dictStartIndice = i;
                    break;
                }
            }

            // Dict bloğunun başlangıç satırından itibaren
            // FacebookDisplayName ve FacebokClientToken alanlarını ekliyoruz
            int insertIndices = dictStartIndice + 1;
            infoPlistLines.InsertRange(insertIndices, new string[]
            {
                "<key>FacebookDisplayName</key>",
                $"<string>{FacebookSDKInfoPlistProcessorData.DisplayName}</string>",
                "<key>FacebookClientToken</key>",
                $"<string>{FacebookSDKInfoPlistProcessorData.ClientToken}</string>",
            });

            File.WriteAllLines(infoPlistPath, infoPlistLines.ToArray());
            Debug.Log("Info.plist Processed");
        }
    }
}

