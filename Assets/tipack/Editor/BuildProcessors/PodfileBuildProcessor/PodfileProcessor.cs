using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using UnityEditor;

namespace tiplay
{

    public class PodfileProcessor : IPostprocessBuildWithReport
    {
        int IOrderedCallback.callbackOrder => 9999;

        // Podfile'daki gerekli düzenlemeleri yapıyor
        private static bool InitPodfile(string buildPath, bool addStaticLinkage)
        {
            string podfilePath = buildPath + "/Podfile";

            if (!File.Exists(podfilePath))
            {
                Debug.LogError("Because podfile not found, PodfileProcessor not runned.");
                return false;
            }

            string[] podfileLines = File.ReadAllLines(podfilePath);
            int startIndices = Array.IndexOf(podfileLines, "target 'Unity-iPhone' do");

            // Podfile'dan gerekli olan satıra kadar olan kısmı listeye alıyoruz
            var lines = podfileLines.Take(startIndices).ToList();

            // Podfile'a "linkage static" satırını ekliyoruz
            if (addStaticLinkage)
                lines.Add("use_frameworks! :linkage => :static");

            // Podfile'ı güncelliyoruz
            File.WriteAllLines(podfilePath, lines);
            Debug.Log("Podfile processed");
            return true;
        }

        private static void RunPodInstall(string buildPath)
        {
            // Pod install komutunu build klasöründe çalıştırır
            var response = Terminal.RunCommand(buildPath, "export LANG=en_US.UTF-8 && /usr/local/bin/pod install");

            PodInstallResultWindow.ShowWindow(response);
            Debug.Log("\"pod install\" result; \n<color=green>" + response.Result + "</color>\n<color=orange>" + response.Error + "</color>");
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.iOS)
                return;

            if (!PodfileProcessorData.ProcessorEnabled)
                return;

            string buildPath = report.summary.outputPath;

            if (!InitPodfile(buildPath, PodfileProcessorData.AddStaticLinkage))
                return;

            if (!PodfileProcessorData.AutoRunPodInstall)
                return;

            RunPodInstall(buildPath);
        }
    }
}