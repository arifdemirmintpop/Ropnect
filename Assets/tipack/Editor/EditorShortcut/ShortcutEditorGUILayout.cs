using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using static UnityEditor.Progress;

namespace tiplay
{
    public static class ShortcutEditorGUILayout
    {
        private static string[] assistantKeyNames = new string[]
        {
            "Shift", "Control", "Option\\Alt", "Command\\Windows"
        };

        private static AssistantKey DrawAssistantKeyField(string label, AssistantKey assistantKey)
        {
            return (AssistantKey)EditorGUILayout.Popup(label, (int)assistantKey, assistantKeyNames);
        }

        public static Shortcut DrawShortcutField(Shortcut shortcut, bool showLabels)
        {
            shortcut.assistant = DrawAssistantKeyField(showLabels ? "Assistant" : "", shortcut.assistant);
            shortcut.key = (KeyCode)EditorGUILayout.EnumPopup(showLabels ? "Key" : "", shortcut.key);
            return shortcut;
        }
    }
}

