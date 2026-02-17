using System;
using UnityEngine;

namespace tiplay.SDKManagement
{
    [Serializable]
    public class SDKData
    {
        [SerializeField] private SDKVersionData[] versions = new SDKVersionData[0];

        public SDKVersionData[] Versions => versions;

        public SDKVersionData FindByPackageName(string packageName)
        {
            foreach (var versionData in versions)
                if (versionData.PackageName == packageName)
                    return versionData;

            return null;
        }

        public SDKData(SDKVersionData[] _versions)
        {
            versions = _versions;
        }
    }
}