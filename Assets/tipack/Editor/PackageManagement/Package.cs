using System;
using Newtonsoft.Json;
using UnityEngine;

namespace tiplay.PackageManagement
{
    [Serializable]
    public class Package
    {
        [SerializeField, JsonProperty] private string title;
        [SerializeField, JsonProperty] private string identifier;
        [SerializeField, JsonProperty] private string description;
        [SerializeField, JsonProperty] private string downloadUrl;
        [SerializeField, JsonProperty] private string documentUrl;

        public string Title => title;
        public string Identifier => identifier;
        public string Description => description;
        public string DownloadURL => downloadUrl;
        public string DocumentURL => documentUrl;
    }
}