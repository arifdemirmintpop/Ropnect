using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace tiplay.PackExporter
{

    public class TipackExportUtility : MonoBehaviour
    {
        [MenuItem("Tiplay/Export Tipack", priority = 0)]
        static void ExportTipack()
        {
            var paths = TipackExportData.GetInstance().ExportList.Select(AssetDatabase.GetAssetPath).ToArray();

            string exportPath = EditorUtility.SaveFilePanel("Export Path", "~/Desktop", "tipack.unitypackage", "unitypackage");

            AssetDatabase.ExportPackage(paths, exportPath, ExportPackageOptions.Recurse);
        }
    }
}
