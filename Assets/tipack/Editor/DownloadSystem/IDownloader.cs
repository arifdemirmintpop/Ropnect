using UnityEngine.Networking;

namespace tiplay.Downloader
{
    public interface IDownloader
    {
        string uri { get; }
        void OnDownloadStart();
        void OnDownloadProgress(float progress);
        void OnDownloadComplete(DownloadHandler handler);
        void OnDownloadError(string error);
    }
}