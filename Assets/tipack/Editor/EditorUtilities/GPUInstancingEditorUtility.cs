using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace tiplay.EditorUtilities
{
    public static class GPUInstancingEditorUtility
    {
        public static void SetGPUInstancing(Material material, bool instancing)
        {
            Undo.RecordObject(material, "Enable GPU Instancing");
            material.enableInstancing = instancing;
            EditorUtility.SetDirty(material);
            AssetDatabase.SaveAssetIfDirty(material);
        }
    }
}