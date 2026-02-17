using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace tiplay.AudioKit
{
    [CustomPropertyDrawer(typeof(SoundList))]
    public class SoundListPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var _soundCategoryProperty = property.FindPropertyRelative("Category");
            string _soundCategoryDisplayName = ((SoundCategory)_soundCategoryProperty.enumValueIndex).ToString();
            label.text = _soundCategoryDisplayName;
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }


    [CustomPropertyDrawer(typeof(Sound))]
    public class SoundPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var _soundProperty = property.FindPropertyRelative("SoundType");
            string _soundDisplayName = ((SoundType)_soundProperty.enumValueIndex).ToString();
            label.text = _soundDisplayName;
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}