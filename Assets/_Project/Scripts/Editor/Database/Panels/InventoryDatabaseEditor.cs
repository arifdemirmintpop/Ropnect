using UnityEngine;
using UnityEditor;

namespace tiplay.Toolkit
{
    public partial class DatabaseEditor : ITool
    {
        private void InventoryDatabasePanel()
        {
            if (IsDatabaseExist("Inventory Database"))
            {
                Database.InventoryDatabase.Money = EditorGUILayout.FloatField("Money ", Database.InventoryDatabase.Money);
            }
        }
    }
}