using UnityEditor;
using System;

namespace tiplay.Toolkit
{
    public class GameSettingsShortcut : ITool
    {
        private Editor gameSettingsEditor;
        private EditorWindow window;

        public string Title => "Game Settings";

        public string Shortcut => string.Empty;

        public GameSettingsShortcut(EditorWindow window)
        {
            this.window = window;
        }

        public void OnCreate() { }

        public void OnDestroy() { }

        public void OnEnable()
        {
            gameSettingsEditor = Editor.CreateEditor(GameSettings.GetInstance());
        }

        public void OnGUI()
        {
            gameSettingsEditor.OnInspectorGUI();
        }

        public void OnDisable()
        {

        }
    }
}