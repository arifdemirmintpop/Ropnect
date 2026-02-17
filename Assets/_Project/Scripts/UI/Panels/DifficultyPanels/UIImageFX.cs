using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class UIImageFX : MonoBehaviour
{
    public enum RotationDirection
    {
        Clockwise,
        CounterClockwise
    }

    [Header("Target")]
    [SerializeField] private RectTransform target;

    [Header("Move")]
    [Tooltip("Başlangıç konumundan aşağı indiği mesafe (Unity birimi)")]
    [SerializeField] private float moveDownDistance = 50f;
    [Tooltip("Gidiş süresi (sn). moveSpeed>0 ise override edilir.")]
    [SerializeField] private float moveDuration = 1f;
    [Tooltip("Gidiş hızı (birim/sn). 0 veya negatifse devre dışı.")]
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private Ease moveEase = Ease.OutCubic;

    [Header("Rotation")]
    [Tooltip("Dönüş hızı (derece/sn)")]
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private RotationDirection rotationDirection = RotationDirection.Clockwise;

    [Header("Scale")]
    [Tooltip("Hedef ölçek (1 = orijinal)")]
    [SerializeField] private float targetScale = 1.2f;
    [Tooltip("Scale tween süresi (sn)")]
    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;

    [Header("Fade In")]
    [SerializeField] private bool enableFadeIn = true;
    [Tooltip("Fade‑in süresi (sn)")]
    [SerializeField] private float fadeInDuration = 0.5f;

    [Header("Fade Out")]
    [SerializeField] private bool enableFadeOut = true;
    [Tooltip("Fade‑out başlamadan önce bekleme (sn)")]
    [SerializeField] private float fadeOutDelay = 2f;
    [Tooltip("Fade‑out süresi (sn)")]
    [SerializeField] private float fadeOutDuration = 0.5f;

    private CanvasGroup _canvasGroup;
    private Vector3     _originalPos;
    private Vector3     _originalScale;
    private int         _rotationSign;
    private Vector2 _origAnchoredPos;


    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (target == null) target = GetComponent<RectTransform>();

        _origAnchoredPos = target.anchoredPosition;

        _originalScale = target.localScale;
        _rotationSign = rotationDirection == RotationDirection.Clockwise ? -1 : 1;

        // Başlangıç
        if (enableFadeIn)
            _canvasGroup.alpha = 0f;
        else
            _canvasGroup.alpha = 1f;

        target.localScale = _originalScale;
    }

    private void Start()
    {
        // 1) Fade‑in
        if (enableFadeIn)
            _canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.Linear);

        // 2) Move
        float dur = moveDuration;
        if (moveSpeed > 0f)
            dur = moveDownDistance / moveSpeed;

        target
            .DOAnchorPosY(_origAnchoredPos.y - moveDownDistance, dur)
            .SetEase(moveEase);

        // 3) Scale
        target
            .DOScale(_originalScale * targetScale, scaleDuration)
            .SetEase(scaleEase)
            .OnComplete(() =>
                target
                    .DOScale(_originalScale, scaleDuration)
                    .SetEase(scaleEase)
            );

        // 4) Fade‑out
        if (enableFadeOut)
            StartCoroutine(FadeOutAndDisable());
    }

    private void Update()
    {
        // 5) Rotate
        if (rotationSpeed != 0f)
            target.Rotate(Vector3.forward, _rotationSign * rotationSpeed * Time.deltaTime, Space.Self);
    }

    private IEnumerator FadeOutAndDisable()
    {
        yield return new WaitForSeconds(fadeOutDelay);
        _canvasGroup
            .DOFade(0f, fadeOutDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
