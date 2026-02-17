using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using tiplay;
using tiplay.DatabaseSystem;
using tiplay.HapticKit;
using tiplay.AudioKit;

public enum SettingType { Audio, Music, Haptic }

[System.Serializable]
public class SettingOption
{
    public SettingType type;

    public Slider slider;
    public Image icon;
    public Sprite onSprite;
    public Sprite offSprite;
    public GameObject textOn;
    public GameObject textOff;
}

public class SettingsPanel : MonoBehaviour
{
    [Header("Panel ve Butonlar")]
    [SerializeField] private Image _panelImage;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;

    [Header("Ayar Seçenekleri")]
    [SerializeField] private List<SettingOption> _options;

    [Header("Animasyon Ayarları")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private List<RectTransform> waveTargets = new();
    [SerializeField] private float waveDelayBetween = 0.05f;
    [SerializeField] private float waveMoveAmount = 20f;
    [SerializeField] private float waveDuration = 0.5f;

    private bool _isOpen;
    private SettingsData _settings;

    private void Awake()
    {
        _panelImage.gameObject.SetActive(false);
        _openButton.onClick.AddListener(OpenPanel);
        _closeButton.onClick.AddListener(ClosePanel);

        // Tüm seçenekler için callback'leri ve handle toggle'ı kur
        foreach (var opt in _options)
        {
            opt.slider.onValueChanged.AddListener(val => OnSliderChanged(opt, val));
            SetupHandleToggle(opt.slider);
        }
    }

    private void Start()
    {
        // Veritabanından struct'a yükle
        var db = GlobalData.Database.PreferenceDatabase;
        _settings = new SettingsData
        {
            IsAudioEnabled     = db.IsAudioEnable,
            IsMusicEnabled     = db.IsMusicEnable,
            IsVibrationEnabled = db.IsVibrationEnable
        };

        // Slider ve görselleri başlat
        foreach (var opt in _options)
        {
            bool enabled = GetSettingValue(opt.type);
            opt.slider.value = enabled ? 1f : 0f;
            UpdateOptionVisuals(opt, enabled);
        }
    }

    private void OnDestroy()
    {
        _openButton.onClick.RemoveListener(OpenPanel);
        _closeButton.onClick.RemoveListener(ClosePanel);
        foreach (var opt in _options)
            opt.slider.onValueChanged.RemoveAllListeners();
    }

    private void OpenPanel()
    {
        if (_isOpen) return;
        _isOpen = true;
        Time.timeScale = 0;
        _panelImage.gameObject.SetActive(true);

        var bg = _backgroundImage.rectTransform;
        bg.localScale = Vector3.one;
        float w = bg.rect.width;
        bg.anchoredPosition = new Vector2(w + 100f, 0);

        bg.DOAnchorPos(Vector2.zero, 0.4f)
          .SetEase(Ease.OutBack)
          .SetUpdate(true)
          .OnComplete(() => StartCoroutine(PlayWaveEffect()));
    }

    private void ClosePanel()
    {
        if (!_isOpen) return;
        _isOpen = false;

        var bg = _backgroundImage.rectTransform;
        bg.DOScale(0f, 0.25f)
          .SetEase(Ease.InBack)
          .SetUpdate(true)
          .OnComplete(() =>
          {
              _panelImage.gameObject.SetActive(false);
              Time.timeScale = 1;
              bg.localScale = Vector3.one;
          });
    }

    private void OnSliderChanged(SettingOption opt, float val)
    {
        bool enabled = val >= 0.5f;
        SetSettingValue(opt.type, enabled);
        SaveSettings();
        UpdateOptionVisuals(opt, enabled);

        InteractionPunch(opt.slider.transform);

        AudioKitManager.PlaySound(SoundType.SwitchSettings, SoundCategory.UserInterface);
        HapticKitManager.Vibrate(HapticType.ButtonTapped);
    }

    private void UpdateOptionVisuals(SettingOption opt, bool enabled)
    {
        opt.icon.sprite     = enabled ? opt.onSprite  : opt.offSprite;
        opt.textOn.SetActive(enabled);
        opt.textOff.SetActive(!enabled);
    }

    private bool GetSettingValue(SettingType type) => type switch
    {
        SettingType.Audio  => _settings.IsAudioEnabled,
        SettingType.Music  => _settings.IsMusicEnabled,
        SettingType.Haptic=> _settings.IsVibrationEnabled,
        _ => false
    };

    private void SetSettingValue(SettingType type, bool value)
    {
        switch (type)
        {
            case SettingType.Audio:   _settings.IsAudioEnabled     = value; break;
            case SettingType.Music:   _settings.IsMusicEnabled     = value; break;
            case SettingType.Haptic: _settings.IsVibrationEnabled = value; break;
        }
    }

    private void SaveSettings()
    {
        var db = GlobalData.Database.PreferenceDatabase;
        db.IsAudioEnable     = _settings.IsAudioEnabled;
        db.IsMusicEnable     = _settings.IsMusicEnabled;
        db.IsVibrationEnable = _settings.IsVibrationEnabled;
    }

    private void InteractionPunch(Transform t, float scale = -0.2f, float duration = 0.2f)
    {
        t.DOComplete();
        t.DOPunchScale(Vector3.one * scale, duration, 1, 1)
         .SetUpdate(true);
    }

    private void SetupHandleToggle(Slider slider)
    {
        if (slider.handleRect == null) return;
        var btn = slider.handleRect.gameObject.GetComponent<Button>()
               ?? slider.handleRect.gameObject.AddComponent<Button>();
        btn.onClick.AddListener(() =>
            slider.value = slider.value >= 0.5f ? 0f : 1f
        );
    }

    private IEnumerator PlayWaveEffect()
    {
        foreach (var target in waveTargets)
        {
            if (target == null) continue;
            var orig = target.anchoredPosition;
            var up   = orig + Vector2.up * waveMoveAmount;

            target.DOAnchorPos(up, waveDuration/2f)
                  .SetEase(Ease.OutSine)
                  .SetUpdate(true)
                  .OnComplete(() =>
                      target.DOAnchorPos(orig, waveDuration/2f)
                            .SetEase(Ease.InSine)
                            .SetUpdate(true)
                  );

            yield return new WaitForSecondsRealtime(waveDelayBetween);
        }
    }
}

public struct SettingsData
{
    public bool IsAudioEnabled;
    public bool IsMusicEnabled;
    public bool IsVibrationEnabled;
}
