using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEditor;
using tiplay.Downloader;

namespace tiplay.SDKManagement
{
    public class SDKVersionDownloader : IDownloader
    {
        public delegate void DownloadCompleteCallback(SDKVersionData package, string downloadPath);

        SDKVersionData sdkVersionData;

        string IDownloader.uri => sdkVersionData.DownloadURL;

        public event DownloadCompleteCallback downloadComplete;

        public SDKVersionDownloader(SDKVersionData sdkVersionData)
        {
            this.sdkVersionData = sdkVersionData;
        }

        void IDownloader.OnDownloadComplete(DownloadHandler handler)
        {
            EditorUtility.ClearProgressBar();

            string fileName = sdkVersionData.Title + " - " + sdkVersionData.Version + ".unitypackage";

            if (File.Exists(fileName))
                File.Delete(fileName);

            string downloadPath = Application.persistentDataPath + "/Downloaded Packages";

            if (!Directory.Exists(downloadPath))
                Directory.CreateDirectory(downloadPath);

            string fullPath = downloadPath + "/" + fileName;

            File.WriteAllBytes(fullPath, handler.data);
            downloadComplete?.Invoke(sdkVersionData, fullPath);
        }

        void IDownloader.OnDownloadError(string error)
        {
            EditorUtility.ClearProgressBar();
        }

        void IDownloader.OnDownloadProgress(float progress)
        {
            EditorUtility.DisplayProgressBar("Package Downloading", $"%{(progress * 100).ToString("F0")}", progress);
        }

        void IDownloader.OnDownloadStart()
        {
        }
    }
}