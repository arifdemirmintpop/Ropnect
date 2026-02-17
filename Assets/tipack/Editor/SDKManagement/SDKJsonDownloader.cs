using System;
using Newtonsoft.Json;
using tiplay.Downloader;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

namespace tiplay.SDKManagement
{
    public class SDKJsonDownloader : IDownloader
    {
        public delegate void JsonDownloadCallback(SDKWebResponse response);
        const string testUrl = "https://tiowsstoragenew.blob.core.windows.net/media/media/external/testing/SDKInformations.json";
        const string liveUrl = "https://tiowsstoragenew.blob.core.windows.net/media/media/external/sdk.json";
        string IDownloader.uri => testUrl;

        public event JsonDownloadCallback downloadComplete;

        void IDownloader.OnDownloadComplete(DownloadHandler handler)
        {
            SDKWebResponse responseData = new SDKWebResponse();
            EditorJsonUtility.FromJsonOverwrite(handler.text, responseData);
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