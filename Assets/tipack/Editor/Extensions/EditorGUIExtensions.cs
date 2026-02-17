using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace tiplay.EditorExtensions
{
    public static class EditorGUIExtensions
    {
        /// <summary>
        /// LayerMask türündeki değişkenleri EditorGUI'de gösterir
        /// </summary>
        /// <param name="label">EditorGUI'de gösterilecek değişkenin başlığı</param>
        /// <param name="layerMask">EditorGUI'de gösterilecek LayerMask değişkeni</param>
        /// <returns></returns>
        public static LayerMask LayerMaskField(string label, LayerMask layerMask)
        {
            string[] layerNames = new string[32];

            for (int i = 0; i < 32; i++)
                layerNames[i] = LayerMask.LayerToName(i);

            layerMask.value = EditorGUILayout.MaskField(label, layerMask.value, layerNames);
            return layerMask;
        }
    }
}

