using System;
using Newtonsoft.Json;
using UnityEngine;

namespace tiplay.PackageManagement
{
    [Serializable]
    public class PackageListResponse
    {
        [SerializeField, JsonProperty] private Package[] packages = new Package[0];

        public Package[] Packages => packages;
    }
}