using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEditor;
using UnityEditor.PackageManager;
using System.Linq;
using PackageManager = UnityEditor.PackageManager;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using System;
using UnityEditorCoroutineUtilities.Editor;

namespace tiplay
{
    public static class PackageManagerController
    {
        static Queue<string> packageInstallQueue = new Queue<string>();
        static AddRequest installRequest;

        public static event Action<PackageInfo> OnPackageInstalled;
        public static event Action<PackageInfo> OnPackageInstalling;
        public static event Action<PackageInfo> OnPackageRemoved;
        public static event Action<PackageInfo> OnPackageRemoving;

        [InitializeOnLoadMethod]
        static void RegisterEvents()
        {
            PackageManager.Events.registeredPackages -= OnPackagesUpdated;
            PackageManager.Events.registeredPackages += OnPackagesUpdated;

            PackageManager.Events.registeringPackages -= OnPackagesUpdating;
            PackageManager.Events.registeringPackages += OnPackagesUpdating;

            EditorApplication.update -= CheckInstallingStatus;
            EditorApplication.update += CheckInstallingStatus;

            EditorApplication.update -= CheckInstallQueue;
            EditorApplication.update += CheckInstallQueue;
        }

        private static void OnPackagesUpdating(PackageRegistrationEventArgs packages)
        {
            foreach (var package in packages.removed)
                OnPackageRemoving?.Invoke(package);

            foreach (var package in packages.added)
                OnPackageInstalling?.Invoke(package);
        }

        private static void OnPackagesUpdated(PackageRegistrationEventArgs packages)
        {
            foreach (var package in packages.added)
                OnPackageInstalled?.Invoke(package);

            foreach (var package in packages.removed)
                OnPackageRemoved?.Invoke(package);
        }

        public static void InstallPackage(string packageName)
        {
            packageInstallQueue.Enqueue(packageName);
        }

        static void CheckInstallQueue()
        {
            if (EditorApplication.isCompiling)
                return;

            if (EditorApplication.isUpdating)
                return;

            if (installRequest != null)
                return;

            if (packageInstallQueue.Count == 0)
                return;

            string packageName = packageInstallQueue.Dequeue();
            installRequest = Client.Add(packageName);
            EditorUtility.DisplayProgressBar("Package Installing", packageName, 0);
        }

        static void CheckInstallingStatus()
        {

            if (installRequest == null)
                return;

            if (!installRequest.IsCompleted)
                return;

            if (installRequest.Status == StatusCode.Success)
                Debug.Log("Installed: " + installRequest.Result.packageId);

            if (installRequest.Status == StatusCode.Failure)
                Debug.Log("Failure: " + installRequest.Result.packageId);

            EditorUtility.ClearProgressBar();
            installRequest = null;
        }

        public static bool TryGetInstalledPackage(string packageId, out PackageInfo package)
        {
            package = PackageInfo.GetAllRegisteredPackages().FirstOrDefault(package => package.packageId.StartsWith(packageId));
            return package != null;
        }

        public static bool IsPackageInstalled(string packageId)
        {
            return TryGetInstalledPackage(packageId, out PackageInfo package);
        }

        public static void IsInstalledLatestVersion(string packageId, Action<bool> onQueryComplete)
        {
            if (!TryGetInstalledPackage(packageId, out PackageInfo package))
            {
                onQueryComplete?.Invoke(false);
                return;
            }

            EditorCoroutineUtility.StartCoroutine(LatestVersionQueryRoutine(packageId, (latestPackage) =>
            {
                var latestVersion = new Version(latestPackage.version);
                var currentVersion = new Version(package.version);

                onQueryComplete?.Invoke(latestVersion <= currentVersion);
            }), package);
        }

        private static IEnumerator LatestVersionQueryRoutine(string packageId, Action<PackageInfo> onQueryComplete)
        {
            var request = PackageManager.Client.Search(packageId);

            while (request.Status == StatusCode.InProgress) yield return null;

            onQueryComplete?.Invoke(request.Result[0]);
        }
    }
}
