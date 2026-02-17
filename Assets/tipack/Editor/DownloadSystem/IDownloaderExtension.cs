using System.Collections;
using UnityEditorCoroutineUtilities.Editor;
using UnityEngine.Networking;

namespace tiplay.Downloader
{
    public static class IDownloaderExtension
    {
        public static void Download(this IDownloader downloader)
        {
            EditorCoroutineUtility.StartCoroutine(DownloadRequest(downloader), downloader);
        }

        private static IEnumerator DownloadRequest(IDownloader downloader)
        {
            using (var www = UnityWebRequest.Get(downloader.uri))
            {
                var request = www.SendWebRequest();

                downloader.OnDownloadStart();

                while (!request.isDone)
                {
                    downloader.OnDownloadProgress(request.progress);
                    yield return null;
                }

                if (!string.IsNullOrEmpty(www.error))
                {
                    downloader.OnDownloadError(www.error);
                    yield break;
                }

                downloader.OnDownloadComplete(www.downloadHandler);
            }
        }
    }
}