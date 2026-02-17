using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using tiplay.DatabaseSystem;
using tiplay;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class LevelDifficultyDisplay : MonoBehaviour
{
    [SerializeField] private List<Image> difficultyImages = new List<Image>();
    [SerializeField] private float displayDuration = 1.5f;
    [SerializeField] private float fadeDuration = 0.5f;
    private LevelDatabase levelDatabase;
    private CanvasGroup _canvasGroup;
    private int _activeIdx = -1;

    private void Awake()
    {
        levelDatabase = GlobalData.Database.LevelDatabase;
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
        foreach (var img in difficultyImages)
            img.gameObject.SetActive(false);
    }

    private void Start()
    {
        ShowDifficulty();
    }

    private void ShowDifficulty()
    {
        int lvl = levelDatabase.LevelOrder[LevelManager.GetLevelInOrder()];

        if (levelDatabase.Levels[lvl].Difficulty == LevelDifficulty.None)
            return;

        _activeIdx = (int)levelDatabase.Levels[lvl].Difficulty - 1;

        if (_activeIdx < 0 || _activeIdx >= difficultyImages.Count)
            return;

        // InputManager.Instance.DisableInteraction();
        for (int i = 0; i < difficultyImages.Count; i++)
            difficultyImages[i].gameObject.SetActive(i == _activeIdx);
        DOTween.Sequence()
            .Append(_canvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.Linear))
            .AppendInterval(displayDuration)
            .Append(_canvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                // InputManager.Instance.EnableInteraction();
                if (_activeIdx >= 0)
                    difficultyImages[_activeIdx].gameObject.SetActive(false);
            });
    }
}
