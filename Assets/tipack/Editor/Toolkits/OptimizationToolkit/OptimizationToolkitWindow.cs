using UnityEditor;

namespace tiplay.Toolkit.OptimizationToolkit
{
    public class OptimizationToolkitWindow : ToolkitWindow
    {
        [MenuItem("Tiplay/Optimization Toolkit", false, 102)]
        static void OpenWindow()
        {
            GetWindow<OptimizationToolkitWindow>("Optimization Toolkit");
        }

        protected override ITool[] GetTools()
        {
            return new ITool[]
            {
                new TextureStats(),
                new MeshStats(),
                new MaterialStats(),
                new ParticleStats()
            };
        }
    }
}

