using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace tiplay.HapticKit
{
    [CustomPropertyDrawer(typeof(Haptic))]
    public class HapticKitPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var _hapticTypeProperty = property.FindPropertyRelative("HapticType");
            string _hapticTypeDisplayName = ((HapticType)_hapticTypeProperty.enumValueIndex).ToString();
            label.text = _hapticTypeDisplayName;
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}