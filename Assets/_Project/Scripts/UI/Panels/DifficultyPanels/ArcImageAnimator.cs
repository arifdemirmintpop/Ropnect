using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ArcImageDotween : MonoBehaviour
{
    [Header("Animasyon Yapılacak UI Image’ların RectTransform’ları")]
    [Tooltip("Sağ taraftan sola doğru gidecek imaj")]
    [SerializeField] private RectTransform imageRight;
    [Tooltip("Sol taraftan sağa doğru gidecek imaj")]
    [SerializeField] private RectTransform imageLeft;

    [Header("Yay Yüksekliği (px)")]
    [SerializeField] private float arcHeight     = 100f;
    [Header("Animasyon Süresi (sn)")]
    [SerializeField] private float duration      = 1f;
    [Header("Geri Hareketten Önce Bekleme Süresi (sn)")]
    [SerializeField] private float waitDuration  = 1.5f;
    [Header("Ease Türü")]
    [SerializeField] private Ease  ease          = Ease.InOutSine;

    private void Start()
    {
        AnimateHalfArc(imageRight);
        AnimateHalfArc(imageLeft);
    }

    private void AnimateHalfArc(RectTransform rt)
    {
        // Başlangıç ve bitiş pozisyonları
        Vector3 start   = rt.localPosition;
        Vector3 end     = new Vector3(-start.x, start.y, start.z);
        // Kontrol noktası: ortada yukarı doğru yükselen tepe
        Vector3 control = (start + end) * 0.5f + Vector3.up * arcHeight;
        float t = 0f;

        // Image bileşenini alıp alpha’yı 1’e ayarla
        Image img = rt.GetComponent<Image>();
        if (img != null)
        {
            var c = img.color;
            img.color = new Color(c.r, c.g, c.b, 1f);
        }

        // Sequence: ileri → bekle → geri (+ fade out)
        Sequence seq = DOTween.Sequence();

        // 1) İleri yarım yay
        seq.Append(
            DOTween.To(() => t, x => t = x, 1f, duration)
                .SetEase(ease)
                .OnUpdate(() =>
                {
                    Vector3 m1 = Vector3.Lerp(start,   control, t);
                    Vector3 m2 = Vector3.Lerp(control, end,     t);
                    rt.localPosition = Vector3.Lerp(m1, m2,      t);
                })
        );

        // 2) Bekle
        seq.AppendInterval(waitDuration);

        // 3) Geri yarım yay
        var reverseTween = DOTween.To(() => t, x => t = x, 0f, duration)
            .SetEase(ease)
            .OnUpdate(() =>
            {
                Vector3 m1 = Vector3.Lerp(start,   control, t);
                Vector3 m2 = Vector3.Lerp(control, end,     t);
                rt.localPosition = Vector3.Lerp(m1, m2,      t);
            });

        seq.Append(reverseTween);

        // 4) Geri hareketle eş zamanlı fade out
        if (img != null)
        {
            seq.Join(img.DOFade(0f, duration).SetEase(ease));
        }
    }
}
