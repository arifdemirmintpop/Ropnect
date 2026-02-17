using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Object), true)]
[CanEditMultipleObjects]
public class ArrayRenameEditor : Editor
{
    private SerializedProperty hoveredArrayProp;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty prop = serializedObject.GetIterator();
        bool expanded = true;
        while (prop.NextVisible(expanded))
        {
            expanded = false;

            EditorGUILayout.PropertyField(prop, true);

            if (prop.isArray && prop.propertyType != SerializedPropertyType.String)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();

                if (Event.current.type == EventType.ContextClick && lastRect.Contains(Event.current.mousePosition))
                {
                    hoveredArrayProp = prop.Copy();

                    if (ArrayElementsHaveName(hoveredArrayProp))
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Rename All Elements"), false, () =>
                        {
                            RenamePopupWindow.Show("Enter Base Name", (baseName) =>
                            {
                                RenameAllElements(hoveredArrayProp, baseName);
                            });
                        });
                        menu.ShowAsContext();
                        Event.current.Use();
                    }
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private bool ArrayElementsHaveName(SerializedProperty arrayProp)
    {
        if (arrayProp.arraySize == 0)
            return false;

        var firstElement = arrayProp.GetArrayElementAtIndex(0);
        return firstElement.FindPropertyRelative("name") != null;
    }

    private void RenameAllElements(SerializedProperty arrayProp, string baseName)
    {
        serializedObject.Update();

        for (int i = 0; i < arrayProp.arraySize; i++)
        {
            var element = arrayProp.GetArrayElementAtIndex(i);
            var nameProp = element.FindPropertyRelative("name");

            if (nameProp != null)
            {
                nameProp.stringValue = $"{baseName}{i + 1}";
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
