using UnityEditor;
using UnityEngine;

namespace tiplay.MoneyKit
{
    public static class MoneyEditorUtility
    {
        [MenuItem("Tiplay/Money Settings")]
        private static void OpenSettings()
        {
            EditorUtility.OpenPropertyEditor(MoneySettings.GetInstance());
        }

        [MenuItem("GameObject/Tiplay Object/Money Bar")]
        private static void CreateMoneyBar()
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(MoneyKitData.MoneyBar, Selection.activeTransform);
            Undo.RegisterCreatedObjectUndo(instance, "Money Bar");
            Selection.activeObject = instance;
        }
    }
}
