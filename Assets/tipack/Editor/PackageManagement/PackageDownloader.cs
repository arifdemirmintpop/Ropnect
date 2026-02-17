using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEditor;
using tiplay.Downloader;

namespace tiplay.PackageManagement
{
    public class PackageDownloader : IDownloader
    {
        public delegate void DownloadCompleteCallback(Package package, string downloadPath);

        private Package package;

        string IDownloader.uri => package.DownloadURL;

        public event DownloadCompleteCallback downloadComplete;

        public PackageDownloader(Package package)
        {
            this.package = package;
        }

        void IDownloader.OnDownloadComplete(DownloadHandler handler)
        {
            EditorUtility.ClearProgressBar();

            string fileName = package.Title + ".unitypackage";

            if (File.Exists(fileName))
                File.Delete(fileName);

            string downloadPath = Application.persistentDataPath + "/Downloaded Packages";

            if (!Directory.Exists(downloadPath))
                Directory.CreateDirectory(downloadPath);

            string fullPath = downloadPath + "/" + fileName;

            File.WriteAllBytes(fullPath, handler.data);
            downloadComplete?.Invoke(package, fullPath);
        }

        void IDownloader.OnDownloadError(string error)
        {
            Debug.LogError(error);
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