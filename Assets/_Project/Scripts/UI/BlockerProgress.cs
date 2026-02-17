using UnityEngine;
using UnityEngine.UI;
using TMPro;
using tiplay;
using tiplay.DatabaseSystem;
using DG.Tweening;
using System.Collections.Generic;

public class BlockerProgress : MonoBehaviour
{
    Database database;
    [SerializeField] GameObject blockerPanel;
    [SerializeField] Image blockerSilhouthe;
    [SerializeField] Image blockerImage;
    [SerializeField] TMP_Text blockerText;
    [SerializeField] Transform[] miniShines;
    [SerializeField] Transform bigShine;
    [SerializeField] List<Sprite> blockerSprites;
    int currentBlockerIndex;

    void OnEnable()
    {
        SetBlockerProgress();
    }

    void Awake()
    {
        database = GlobalData.GetInstance().database;
    }

    public void SetBlockerProgress()
    {
        FindCurrentBlockerIndex();
        ManageVisuals();
    }

    private void FindCurrentBlockerIndex()
    {
        currentBlockerIndex = -1;
        int _currentLevel = database.LevelDatabase.LevelTextValue;
        for (int i = 0; i < database.BlockerDatabase.Blockers.Count; i++)
        {
            if (_currentLevel >= database.BlockerDatabase.Blockers[i].StartLevel && database.BlockerDatabase.Blockers[i].EndLevel >= _currentLevel)
            {
                currentBlockerIndex = i;
                break;
            }
        }
    }

    private void ManageVisuals()
    {
        if (currentBlockerIndex == -1) return;
        if (currentBlockerIndex >= blockerSprites.Count) return;

        int _currentLevel = database.LevelDatabase.LevelTextValue;

        blockerSilhouthe.sprite = blockerSprites[currentBlockerIndex];
        blockerImage.sprite = blockerSprites[currentBlockerIndex];
        blockerImage.fillAmount = CalculateProgression(_currentLevel - 1, database.BlockerDatabase.Blockers[currentBlockerIndex].StartLevel, database.BlockerDatabase.Blockers[currentBlockerIndex].EndLevel);
        blockerText.text = $"New Feature Unlock \n %{100 * blockerImage.fillAmount:0}";
        blockerPanel.SetActive(true);

        float _progression = CalculateProgression(_currentLevel, database.BlockerDatabase.Blockers[currentBlockerIndex].StartLevel, database.BlockerDatabase.Blockers[currentBlockerIndex].EndLevel);
        float _delay = _progression == 1 ? 5f : 2f;

        blockerImage.DOFillAmount(_progression, 1).SetEase(Ease.Linear).SetDelay(.5f).OnComplete(() =>
            {
                if (_progression >= 1)
                {
                    blockerImage.transform.DOPunchScale(Vector3.one * .15f, .5f, 1);
                    blockerText.text = $"Unlocked \n<color=#FFA500>{database.BlockerDatabase.Blockers[currentBlockerIndex].Name}</color>";

                    for (int i = 0; i < miniShines.Length; i++)
                    {
                        miniShines[i].DOMove(bigShine.transform.position, .5f);
                    }

                    bigShine.gameObject.SetActive(true);
                    bigShine.transform.DOScale(Vector3.one * 2.5f, .5f).From(0).SetDelay(.45f).OnStart(() =>
                    {
                        for (int i = 0; i < miniShines.Length; i++)
                        {
                            miniShines[i].gameObject.SetActive(false);
                        }
                    });

                }
            }).OnUpdate(() =>
            {
                blockerText.text = $"New Feature Unlock \n %{100 * blockerImage.fillAmount:0}";
            });
    }
    
    private float CalculateProgression(int level, int start, int end)
    {
        int stepCount = end - start + 1;
        int currentStep = level - start + 1;
        float progress = (float)currentStep / stepCount;
        return Mathf.Clamp01(progress);
    }
}
