using UnityEngine;

namespace tiplay.PackExporter
{
    [CreateAssetMenu(menuName = "ExportData")]
    public class TipackExportData : ScriptableObject
    {
        [SerializeField] private Object[] exportList;

        public Object[] ExportList => exportList;

        private static TipackExportData instance;
        public static TipackExportData GetInstance()
        {
            instance ??= Resources.Load<TipackExportData>(nameof(TipackExportData));
            return instance;
        }
    }
}
