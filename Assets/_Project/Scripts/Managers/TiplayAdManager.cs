using System;
using System.Collections;
using tiplay;
using tiplay.DatabaseSystem;
using UnityEngine;
using System.Timers;

#if TIPLAY_ENABLE_SDK && TIPLAY_ELEPHANT
using ElephantSDK;
using RollicGames.Advertisements;
#endif

public class TiplayAdManager : MonoBehaviour
{
    public static TiplayAdManager Instance;
    Database database => GlobalData.GetInstance().database;
    private bool sdkInitiliazed = false;
    private float timeScaleBeforePause;

    #region Rewarded Actions
    Action rewardedFinished = null;
    Action rewardedSkipped = null;
    Action rewardedFailed = null;
    #endregion

    #region Interstitial Actions
    Action interstitialCompleted = null;
    #endregion

    #region Remote Variables
    private bool isBannerEnabled;
    private bool isInterstitialEnabled;
    private bool isRewardedEnabled;
    private int bannerDelay;
    private int interstitialDelay;
    private int interstitialInterval;
    private int adFreeDays;
    #endregion

    #region Banner Variables
    private bool isBannerLoaded;
    private bool canShowBanner = true;
    private int bannerRemainingDuration;
    private System.Timers.Timer bannerTimer;
    #endregion

    #region Interstitial Variables
    private float lastTimeAdDisplayed;
    private bool isInterstitialUnlocked;
    private int interstitialRemainingDuration;
    private System.Timers.Timer interstitialTimer;
    #endregion

    void Awake()
    {
        CreateSingleton();
    }

    private void CreateSingleton()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void Initialize()
    {
        if (sdkInitiliazed) return;
#if TIPLAY_ENABLE_SDK && TIPLAY_ELEPHANT && !UNITY_EDITOR
        bool isAutoAdEnable = RemoteConfig.GetInstance().GetBool("gamekit_ads_enabled", true);
        if (!isAutoAdEnable)
        {
            Debug.Log("Init Request");
            RLAdvertisementManager.Instance.init();
            SubscribeEvents();
        }
#endif
        EventManager.OnAdLogicDetermined.Invoke();
        
    }
#if TIPLAY_ENABLE_SDK && TIPLAY_ELEPHANT
    #region Life Cycle
    private void AdSDKInitialized()
    {
        Debug.Log("Initted");
        sdkInitiliazed = true;
        CatchRemoteData();
        if (isBannerEnabled)
            InitializeBanner();
        if (isInterstitialEnabled)
            InitializeInterstitial();
    }

    private void CatchRemoteData()
    {
        isBannerEnabled = RemoteConfig.GetInstance().GetBool("gamekit_banner_enabled", true);
        isInterstitialEnabled = RemoteConfig.GetInstance().GetBool("gamekit_interstitial_enabled", true);
        isRewardedEnabled = RemoteConfig.GetInstance().GetBool("gamekit_rewarded_enabled", false);
        bannerDelay = RemoteConfig.GetInstance().GetInt("gamekit_ads_first_banner_delay", 1200);
        interstitialDelay = RemoteConfig.GetInstance().GetInt("gamekit_ads_first_interstitial_delay", 1200);
        interstitialInterval = RemoteConfig.GetInstance().GetInt("gamekit_ads_interstitial_display_interval", 30);
        adFreeDays = RemoteConfig.GetInstance().GetInt("ad_free_days", 1);
    }

    private void OnApplicationQuit()
    {
        if (sdkInitiliazed)
            UnsubscribeEvents();
    }
    #endregion

    #region Banner Methods
    private void InitializeBanner()
    {
        bannerRemainingDuration = (int)(bannerDelay - database.UserEngagementDatabase.TotalPlaytime);
        if (bannerRemainingDuration > 0)
        {
            bannerTimer = new Timer(1000);
            bannerTimer.Elapsed += OnBannerControl;
            bannerTimer.Start();
        }
        else
        {
            LoadBanner();
        }
    }

    private void OnBannerControl(object sender, ElapsedEventArgs e)
    {
        if (bannerRemainingDuration > 0)
        {
            bannerRemainingDuration--;
        }
        else
        {
            LoadBanner();
            bannerTimer.Stop();
        }
    }

    private void LoadBanner()
    {
        Debug.Log("Banner Load Request");
        if (!CheckAdFreeDayCondition()) return;
        RLAdvertisementManager.Instance.SetBannerBackground("#ffffff");
        RLAdvertisementManager.Instance.loadBanner(false);
    }

    private void BannerLoaded()
    {
        Debug.Log("Banner Loaded");
        isBannerLoaded = true;
        if (canShowBanner)
            ShowBanner();
    }

    public void ShowBanner()
    {
        Debug.Log("Show Banner");
        canShowBanner = true;
        if (!IsSdkInitiliazed()) return;
        if (!isBannerEnabled) return;
        if (!isBannerLoaded) return;
#if !UNITY_EDITOR
        RLAdvertisementManager.Instance.showBanner();
#endif
    }

    public void HideBanner()
    {
        Debug.Log("Hide Banner");
        canShowBanner = false;
        if (!IsSdkInitiliazed()) return;
        if (!isBannerEnabled) return;
        if (!isBannerLoaded) return;
#if !UNITY_EDITOR
        RLAdvertisementManager.Instance.hideBanner();
#endif
    }
    #endregion

    #region Interstitial Methods
    private void InitializeInterstitial()
    {
        interstitialRemainingDuration = (int)(interstitialDelay - database.UserEngagementDatabase.TotalPlaytime);
        if (interstitialRemainingDuration > 0)
        {
            interstitialTimer = new Timer(1000);
            interstitialTimer.Elapsed += OnInterstitialControl;
            interstitialTimer.Start();
        }
        else
        {
            ResetTime();
            isInterstitialUnlocked = true;
        }
    }

    private void OnInterstitialControl(object sender, ElapsedEventArgs e)
    {
        if (interstitialRemainingDuration > 0)
        {
            interstitialRemainingDuration--;
        }
        else
        {
            ResetTime();
            isInterstitialUnlocked = true;
            interstitialTimer.Stop();
        }
    }

    private bool CanShowInterstitial()
    {
#if UNITY_EDITOR
        return false;
#elif !UNITY_EDITOR
        if (!IsSdkInitiliazed()) { Debug.Log("SDK is not initialized"); return false; }
        if (!IsInternetReachable()) { Debug.Log("No internet connection"); return false; }
        if (!isInterstitialEnabled) { Debug.Log("Interstitial disabled"); return false; }
        if (!isInterstitialUnlocked) { Debug.Log("Interstitial delay is not completed"); return false; }
        if (!IsInterstitialIntervalValid()) { Debug.Log("Interval is not enough"); return false; }
        if (!CheckAdFreeDayCondition()) { Debug.Log("Ad Free Condition stuck"); return false; }
        if (!RLAdvertisementManager.Instance.isInterstitialReady()) { Debug.Log("Interstitial not ready"); return false; }
        return true;
#endif
    }

    public void ShowInterstitial(Action interstitialCompleted = null)
    {
        this.interstitialCompleted = interstitialCompleted;
#if UNITY_EDITOR
        this.interstitialCompleted?.Invoke();
#else
        if (CanShowInterstitial())
        {
            RLAdvertisementManager.Instance.showInterstitial();
            PauseGame();
        }
        else
            this.interstitialCompleted?.Invoke();
#endif
    }

    private bool IsInterstitialIntervalValid()
    {
        var timeSinceLastTimeAdDisplayed = Time.realtimeSinceStartup - lastTimeAdDisplayed;
        if ((int)(interstitialInterval - timeSinceLastTimeAdDisplayed) > 0)
            return false;
        return true;
    }
    private void InterstitialShown()
    {
        ResetTime();
        ReturnFromInterstitial();
    }

    private void ReturnFromInterstitial()
    {
        if (interstitialCompleted is not null)
        {
            ContinueGame();
            this.interstitialCompleted?.Invoke();
            interstitialCompleted = null;
        }
    }

    private void ReturnFromInterstitial(IronSourceError error)
    {
        ReturnFromInterstitial();
    }
    #endregion

    #region Rewarded Methods
    public bool CanShowReward()
    {
#if UNITY_EDITOR
        return true;
#else
        if (!IsSdkInitiliazed()) return false;
        if (!IsInternetReachable()) return false;
        if (!RLAdvertisementManager.Instance.isRewardedVideoAvailable()) return false;
        return true;
#endif
    }

    public void ShowRewardAds(Action rewardedFinished = null, Action rewardedSkipped = null, Action rewardedFailed = null)
    {
        this.rewardedFinished = rewardedFinished;
        this.rewardedSkipped = rewardedSkipped;
        this.rewardedFailed = rewardedFailed;

#if UNITY_EDITOR
        this.rewardedFinished?.Invoke();
#else
        if (RLAdvertisementManager.Instance)
        {
            RLAdvertisementManager.Instance.showRewardedVideo();
            PauseGame();
        }
#endif
    }

    public void Claim(RLRewardedAdResult result)
    {
        if (result == RLRewardedAdResult.Finished)
        {
            ResetTime();
            rewardedFinished?.Invoke();
        }
        else if (result == RLRewardedAdResult.Skipped)
            rewardedSkipped?.Invoke();
        else if (result == RLRewardedAdResult.Failed)
            rewardedFailed?.Invoke();
    }

    private void ReturnFromRewarded()
    {
        ContinueGame();
    }

    private void ReturnFromRewarded(string adUnitId, string error)
    {
        ContinueGame();
    }
    #endregion

    #region Common Methods
    private bool IsSdkInitiliazed(bool _canLog = true)
    {
        if (!sdkInitiliazed && _canLog)
            Debug.Log("Rollic SDK is not initialized.");
        return sdkInitiliazed;
    }

    private bool IsInternetReachable()
    {
        bool value = Application.internetReachability == NetworkReachability.NotReachable;
        if (value)
            Debug.Log("There is no internet connection.");
        return !value;
    }

    private bool CheckAdFreeDayCondition()
    {
        var totalDays = (DatabaseHelpers.TakeTimestamp() - database.UserEngagementDatabase.InstallTimestamp) / (60 * 60 * 24);
        Debug.Log("Total Days: " + totalDays + " Ad Free Days: " + adFreeDays + " is it pass? " + (totalDays >= adFreeDays).ToString());
        return totalDays >= adFreeDays;
    }

    private void ResetTime()
    {
        lastTimeAdDisplayed = Time.realtimeSinceStartup;
    }

    private void PauseGame()
    {
        EventManager.OnJoystickInputResetted?.Invoke();
        AudioListener.pause = true;
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0;
    }

    private void ContinueGame()
    {
        Time.timeScale = timeScaleBeforePause == 0 ? 1f : timeScaleBeforePause;
        EventManager.OnJoystickInputResetted?.Invoke();
        AudioListener.pause = !DatabaseHelpers.IsAudioEnable();
    }
    #endregion

    #region Events
    private void SubscribeEvents()
    {
        RLAdvertisementManager.OnRollicAdsSdkInitializedEvent += AdSDKInitialized;

        RLAdvertisementManager.OnRollicAdsAdLoadedEvent += BannerLoaded;

        RLAdvertisementManager.OnRollicAdsInterstitialShownEvent += InterstitialShown;
        RLAdvertisementManager.OnRollicAdsInterstitialDismissedEvent += ReturnFromInterstitial;
        RLAdvertisementManager.OnRollicAdsInterstitialFailedEvent += ReturnFromInterstitial;

        RLAdvertisementManager.Instance.rewardedAdResultCallback += Claim;
        RLAdvertisementManager.OnRollicAdsRewardedVideoShownEvent += ReturnFromRewarded;
        RLAdvertisementManager.OnRollicAdsRewardedVideoClosedEvent += ReturnFromRewarded;
        RLAdvertisementManager.OnRollicAdsRewardedVideoFailedToPlayEvent += ReturnFromRewarded;
    }

    private void UnsubscribeEvents()
    {
        RLAdvertisementManager.OnRollicAdsSdkInitializedEvent -= AdSDKInitialized;

        RLAdvertisementManager.OnRollicAdsAdLoadedEvent -= BannerLoaded;

        RLAdvertisementManager.OnRollicAdsInterstitialShownEvent -= InterstitialShown;
        RLAdvertisementManager.OnRollicAdsInterstitialDismissedEvent -= ReturnFromInterstitial;
        RLAdvertisementManager.OnRollicAdsInterstitialFailedEvent -= ReturnFromInterstitial;

        RLAdvertisementManager.Instance.rewardedAdResultCallback -= Claim;
        RLAdvertisementManager.OnRollicAdsRewardedVideoShownEvent -= ReturnFromRewarded;
        RLAdvertisementManager.OnRollicAdsRewardedVideoClosedEvent -= ReturnFromRewarded;
        RLAdvertisementManager.OnRollicAdsRewardedVideoFailedToPlayEvent -= ReturnFromRewarded;
    }
    #endregion
#endif
}