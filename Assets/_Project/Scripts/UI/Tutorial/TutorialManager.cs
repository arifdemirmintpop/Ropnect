using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using tiplay;
using UnityEngine;


public class TutorialManager : MonoBehaviour
{
    [SerializeField] private float duration = 3;
    [SerializeField] private TutorialType tutorialType;

    List<int> numbers = new List<int>();


    private TutorialPanel[] tutorialPanels;

    private void Awake()
    {
        tutorialPanels = GetComponentsInChildren<TutorialPanel>(true);
        CloseAllPanels();
    }

    private void OnEnable()
    {
        EventManager.OnGameStart += OpenDefaultTutorial;
    }

    private void OnDisable()
    {
        EventManager.OnGameStart -= OpenDefaultTutorial;
    }

    private void OpenDefaultTutorial()
    {
        if (!IsTutorialLevel()) return;

        PlayTutorial(tutorialType, duration);
    }

    private void CloseAllPanels()
    {
        foreach (var panel in tutorialPanels)
        {
            panel.gameObject.SetActive(false);
        }

    }

    public void PlayTutorial(TutorialType type, float duration)
    {
        StartCoroutine(TutorialRoutine(type, duration));
    }

    public void CloseTutorial(TutorialType type)
    {
        GetTutorialPanel(type).gameObject.SetActive(false);
    }

    private TutorialPanel GetTutorialPanel(TutorialType type)
    {
        return tutorialPanels.FirstOrDefault(panel => panel.Type == type);
    }

    private IEnumerator TutorialRoutine(TutorialType type, float duration)
    {
        TutorialPanel panel = GetTutorialPanel(type);
        panel.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        panel.gameObject.SetActive(false);
    }

    private bool IsTutorialLevel()
    {
        return GlobalData.Database.LevelDatabase.LevelTextValue <= GameSettings.GetInstance().tutorialLevelCount;
    }
}
