#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(MPBComponent))]
[CanEditMultipleObjects]
public class MPBComponentEditor : Editor
{
    SerializedProperty slotSettings;

    // Foldout durumlarını tutmak için (Editor içi)
    private List<bool> foldouts = new List<bool>();

    void OnEnable()
    {
        slotSettings = serializedObject.FindProperty("slotSettings");

        // Foldout listesi boyutunu ayarla
        foldouts = new List<bool>(new bool[slotSettings.arraySize]);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Material Slot Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Her materyal slotunu göster
        for (int i = 0; i < slotSettings.arraySize; i++)
        {
            SerializedProperty slot = slotSettings.GetArrayElementAtIndex(i);
            SerializedProperty slotName = slot.FindPropertyRelative("slotName");
            SerializedProperty randomizeColor = slot.FindPropertyRelative("randomizeColor");
            SerializedProperty baseColor = slot.FindPropertyRelative("baseColor");
            SerializedProperty metallic = slot.FindPropertyRelative("metallic");
            SerializedProperty smoothness = slot.FindPropertyRelative("smoothness");
            SerializedProperty albedoTexture = slot.FindPropertyRelative("albedoTexture");
            SerializedProperty normalMap = slot.FindPropertyRelative("normalMap");

            foldouts[i] = EditorGUILayout.Foldout(foldouts[i], slotName.stringValue, true, EditorStyles.foldoutHeader);

            if (foldouts[i])
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(randomizeColor, new GUIContent("Randomize Color"));
                EditorGUILayout.PropertyField(baseColor, new GUIContent("Base Color"));

                EditorGUILayout.Slider(metallic, 0f, 1f, new GUIContent("Metallic"));
                EditorGUILayout.Slider(smoothness, 0f, 1f, new GUIContent("Smoothness"));

                EditorGUILayout.PropertyField(albedoTexture, new GUIContent("Albedo Texture"));
                EditorGUILayout.PropertyField(normalMap, new GUIContent("Normal Map"));

                EditorGUI.indentLevel++;
                EditorGUILayout.Space();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Uygula (Apply MPB)"))
        {
            foreach (var t in targets)
            {
                ((MPBComponent)t).ApplyMPB();
                EditorUtility.SetDirty(t);
            }
        }

        if (GUILayout.Button("Rastgele Renk Oluştur (Randomize All)"))
        {
            foreach (var t in targets)
            {
                ((MPBComponent)t).RandomizeAll();
                ((MPBComponent)t).ApplyMPB();
                EditorUtility.SetDirty(t);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
