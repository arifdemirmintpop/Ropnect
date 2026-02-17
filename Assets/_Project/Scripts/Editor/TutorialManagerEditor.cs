using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TutorialManager))]
public class TutorialManagerEditor : Editor
{
    private GUIStyle titleStyle;
    private GUIStyle textStyle;

    private void OnEnable()
    {
        textStyle = new GUIStyle()
        {
            wordWrap = true,
            fontSize = 10
        };

        titleStyle = new GUIStyle()
        {
            wordWrap = true,
            fontSize = 12,
            fontStyle = FontStyle.BoldAndItalic
        };
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Invoke anytime for game tutorials", titleStyle);
            GUILayout.Label("TutorialManagement.PlayTutorial(TutorialType type, float duration);", textStyle);
            GUILayout.Label("TutorialManagement.CloseTutorial(TutorialType type);", textStyle);
        }
    }
}

