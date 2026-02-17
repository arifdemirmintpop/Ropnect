using System;
using Newtonsoft.Json;
using tiplay.Downloader;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace tiplay.PackageManagement
{
    public class PackageListJsonDownloader : IDownloader
    {
        public delegate void JsonDownloadCallback(PackageListResponse response);

        string IDownloader.uri => "https://tiowsstoragenew.blob.core.windows.net/packages/packages.json";

        public event JsonDownloadCallback downloadComplete;

        void IDownloader.OnDownloadComplete(DownloadHandler handler)
        {
            PackageListResponse responseData = JsonConvert.DeserializeObject<PackageListResponse>(handler.text);
            downloadComplete?.Invoke(responseData);
        }

        void IDownloader.OnDownloadError(string error)
        {
            Debug.Log(error);
        }

        void IDownloader.OnDownloadProgress(float progress)
        {

        }

        void IDownloader.OnDownloadStart()
        {

        }
    }
}