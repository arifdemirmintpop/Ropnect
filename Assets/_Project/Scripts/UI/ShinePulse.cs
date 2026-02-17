using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Transform))]
public class ShinePulse : MonoBehaviour
{
    [Header("Pulse Ayarları")]
    [Tooltip("Başlangıç ölçeği")]
    public Vector3 initialScale = Vector3.one;
    [Tooltip("Hedef ölçek (örneğin 1.05 = %5 büyüme)")]
    public Vector3 targetScale = Vector3.one * 1.05f;
    [Tooltip("Bir yön için tween süresi (sn)")]
    public float pulseDuration = 0.5f;
    [Tooltip("Her döngü öncesi bekleme süresi (sn)")]
    public float pulseDelay = 0f;
    [Tooltip("Tween eğrisi")]
    public Ease pulseEase = Ease.InOutSine;

    [Header("Rotation Ayarları")]
    [Tooltip("Dönme hızı (derece/sn)")]
    public float rotationSpeed = 30f;
    [Tooltip("Dönüş ekseni")]
    public Vector3 rotationAxis = Vector3.forward;

    [Header("Kontrol")]
    [Tooltip("Animasyonu oynat/kapat")]
    public bool animate = true;

    private Sequence _pulseSequence;
    private Tween _rotationTween;

    private void Awake()
    {
        transform.localScale = initialScale;
    }

    private void OnEnable()
    {
        if (animate)
            StartAnimations();
    }

    private void OnDisable()
    {
        StopAnimations();
    }

    private void StartAnimations()
    {
        // Ölçek pulsa
        if (_pulseSequence == null)
        {
            _pulseSequence = DOTween.Sequence()
                .AppendInterval(pulseDelay)
                .Append(transform.DOScale(targetScale, pulseDuration).SetEase(pulseEase))
                .Append(transform.DOScale(initialScale, pulseDuration).SetEase(pulseEase))
                .SetLoops(-1, LoopType.Restart);
        }
        else
        {
            _pulseSequence.Restart();
        }

        // Sürekli döndürme
        if (_rotationTween == null)
        {
            _rotationTween = transform
                .DORotate(rotationAxis * 360f, 360f / rotationSpeed, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }
        else
        {
            _rotationTween.Restart();
        }
    }

    private void StopAnimations()
    {
        _pulseSequence?.Pause();
        _rotationTween?.Pause();
    }

    private void OnDestroy()
    {
        _pulseSequence?.Kill();
        _rotationTween?.Kill();
    }

    // Inspector’dan veya kodla animate değerini değiştirince tetiklemek için:
    private void OnValidate()
    {
        // if (!Application.isPlaying) return;
        // if (animate) StartAnimations();
        // else         StopAnimations();
    }
}
