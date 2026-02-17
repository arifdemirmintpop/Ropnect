using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using tiplay.AudioKit;
public class WinPanel : UIPanel
{
    [SerializeField] TMP_Text rewardText;
    [SerializeField] Transform coinTransform;
    [SerializeField] BlockerProgress blockerProgress;

    public override void OnAwake()
    {
        button.onClick.AddListener(ButtonClicked);
    }

    private void OnEnable()
    {
        EventManager.OnGameWin += Open;
    }

    private void OnDisable()
    {
        EventManager.OnGameWin -= Open;
    }

    void Start()
    {
        int target = -1;

        for (int i = 0; i < database.BlockerDatabase.Blockers.Count; i++)
        {
            if (database.LevelDatabase.LevelTextValue == database.BlockerDatabase.Blockers[i].EndLevel + 1)
                target = i;
        }

        if (target >= 0) BlockerPanelController.Instance.OpenPanel(target);
    }

    public override void Open()
    {
        base.Open();

        SetRewardText();

        blockerProgress.SetBlockerProgress();

        SendSDKEvent();

        LevelManager.IncreaseLevel();

        AudioKitManager.PlaySound(SoundType.Win, SoundCategory.UserInterface);
    }

    public override void ButtonClicked()
    {
        if (buttonClicked) return;
        buttonClicked = true;

        GainReward();
    }

    private void LoadNextLevel()
    {
        DOVirtual.DelayedCall(0.5f, () =>
        {
            LevelManager.LoadNextLevel();
        });
    }

    private void SetRewardText()
    {
        int amount = 0;

        DOTween.To(
            () => amount,
            x => amount = x,
            100,
            1
        ).OnUpdate(() =>
        {
            rewardText.text = amount.ToString("0");
        });
    }

    private void GainReward()
    {
        coinTransform.parent.transform.DOScale(0, .5f);

        ParticleImageManager.Instance.CreateParticleImage(
            coinTransform,
            ParticleImageType.Coin,
            100,
            LoadNextLevel
        );

        AudioKitManager.PlaySound(SoundType.CoinAppear, SoundCategory.UserInterface);
    }

    private void SendSDKEvent()
    {
        float timer = CustomEventManager.Instance.GetCurrentTimer();
        int moveCount = CustomEventManager.Instance.GetMoveCount();
        SDKEventManager.SendWinGame(timer, moveCount);
    }
}
