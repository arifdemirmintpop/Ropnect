using System.Collections;
using tiplay.AudioKit;
using tiplay.HapticKit;
using UnityEngine;
using UnityEngine.UI;

public class FailPanel : UIPanel
{

    public override void OnAwake()
    {
        button.onClick.AddListener(ButtonClicked);
    }

    private void OnEnable()
    {
        EventManager.OnGameFail += Open;
    }

    private void OnDisable()
    {
        EventManager.OnGameFail -= Open;
    }

    public override void Open()
    {
        base.Open();

        AudioKitManager.PlaySound(SoundType.Fail, SoundCategory.UserInterface);
    }

    public override void ButtonClicked()
    {
        if (buttonClicked) return;
        buttonClicked = true;

        SendSDKEvent();

        HapticKitManager.Vibrate(HapticType.ButtonTapped);

        LevelManager.LoadSameLevel();
    }

    private void SendSDKEvent()
    {
        float timer = CustomEventManager.Instance.GetCurrentTimer();
        int moveCount = CustomEventManager.Instance.GetMoveCount();
        SDKEventManager.SendFailGame();
    }
}