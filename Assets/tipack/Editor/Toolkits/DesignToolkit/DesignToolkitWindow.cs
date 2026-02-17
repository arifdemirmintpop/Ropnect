using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;

namespace tiplay.Toolkit.DesignToolkit
{
    public class DesignToolkitWindow : ToolkitWindow
    {
        [MenuItem("Tiplay/Design Toolkit", false, 100)]
        static void OpenWindow()
        {
            GetWindow<DesignToolkitWindow>("Design Toolkit");
        }

        protected override ITool[] GetTools()
        {
            return new ITool[]
            {
                new GroundSnapper(),
                new ObjectPlacer(),
                new ObjectReplacer(),
                new ObjectInstantiator(),
                new Measure(),
                new ChronometerTool(this),
                new DatabaseEditor(this)
            };
        }
    }
}

