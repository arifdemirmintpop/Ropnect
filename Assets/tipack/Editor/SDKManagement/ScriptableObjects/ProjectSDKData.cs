using UnityEngine;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace tiplay.SDKManagement
{
    public class ProjectSDKData : ScriptableObject
    {
        [SerializeField] private SDKStatusData elephantGameKit;
        [SerializeField] private SDKStatusData externalDependency;
        [SerializeField] private SDKStatusData facebook;
        [SerializeField] private SDKStatusData gameAnalytics;
        [SerializeField] private SDKStatusData elephant;
        [SerializeField] private SDKStatusData tibug;

        public SDKStatusData ElephantGameKit => elephantGameKit;
        public SDKStatusData ExternalDependency => externalDependency;
        public SDKStatusData Facebook => facebook;
        public SDKStatusData GameAnalytics => gameAnalytics;
        public SDKStatusData Elephant => elephant;
        public SDKStatusData Tibug => tibug;

        private SDKStatusData[] sdkStatusDatas => new SDKStatusData[] {
            elephantGameKit,
            externalDependency,
            facebook,
            gameAnalytics,
            elephant,
            tibug
        };


        private static ProjectSDKData instance;
        public static ProjectSDKData GetInstance()
        {
            instance ??= Resources.Load<ProjectSDKData>(nameof(ProjectSDKData));
            return instance;
        }

        private SDKStatusData[] GetInstalledSDKs()
        {
            return sdkStatusDatas.Where(data => data.IsInstalled).ToArray();
        }

        public bool IsInstallable(SDKVersionData versionData)
        {
            foreach (var dependency in versionData.Dependencies)
            {
                if (!TryGetStatusData(dependency, out var dependencyData)) continue;

                if (!dependencyData.IsInstalled)
                    return false;
            }

            return true;
        }

        public string[] GetDependants(SDKStatusData sdkData)
        {
            if (!sdkData.IsInstalled) return new string[0];

            List<string> packages = new List<string>();
            foreach (var installedSdk in GetInstalledSDKs())
            {
                if (installedSdk == sdkData) continue;

                foreach (var dependency in installedSdk.installedVersion.Dependencies)
                {
                    if (!TryGetStatusData(dependency, out var dependencyData)) continue;

                    if (dependencyData == sdkData)
                        packages.Add(installedSdk.installedVersion.Title);
                }
            }

            return packages.ToArray();
        }

        public bool HasDependants(SDKStatusData sdkData)
        {
            return GetDependants(sdkData).Length > 0;
        }

        public bool TryGetStatusData(string byName, out SDKStatusData statusData)
        {
            var field = GetType().GetField(byName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
            {
                statusData = null;
                return false;
            }

            statusData = field.GetValue(this) as SDKStatusData;
            return true;
        }
    }
}