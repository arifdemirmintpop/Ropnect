using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace tiplay.PackageManagement
{
    //[CreateAssetMenu(menuName = "Data/Package Database")]
    public class PackageDatabase : ScriptableObject
    {
        [SerializeField] private List<string> installedPackages = new List<string>();

        private static PackageDatabase instance;
        private static PackageDatabase GetInstance()
        {
            instance ??= Resources.Load<PackageDatabase>(nameof(PackageDatabase));
            return instance;
        }

        public static bool IsInstalled(Package package)
        {
            return GetInstance().installedPackages.Contains(package.Identifier);
        }

        public static void RemovePackage(Package package)
        {
            GetInstance().installedPackages.Remove(package.Identifier);
            Save();
        }

        public static void AddPackage(Package package)
        {
            if (IsInstalled(package)) return;

            GetInstance().installedPackages.Add(package.Identifier);
            Save();
        }

        public static void Save()
        {
            EditorUtility.SetDirty(GetInstance());
            AssetDatabase.SaveAssetIfDirty(GetInstance());
        }
    }
}