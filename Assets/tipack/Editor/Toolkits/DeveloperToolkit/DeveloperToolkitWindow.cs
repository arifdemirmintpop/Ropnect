using UnityEditor;

namespace tiplay.Toolkit.DeveloperToolkit
{
    public class DeveloperToolkitWindow : ToolkitWindow
    {
        [MenuItem("Tiplay/Developer Toolkit", false, 101)]
        static void OpenWindow()
        {
            GetWindow<DeveloperToolkitWindow>("Developer Toolkit");
        }

        protected override ITool[] GetTools()
        {
            return new ITool[]
            {
                new ProjectInitializer(this),
                new AndroidBuilder(),
                new IOSBuilder(),
                new InGameTools(),
                new SDKManagement(),
                new PackageManagement(),
                new DatabaseEditor(this),
                new GameSettingsShortcut(this)
            };
        }
    }
}

