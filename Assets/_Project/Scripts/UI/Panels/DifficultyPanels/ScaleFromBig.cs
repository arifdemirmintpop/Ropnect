using UnityEngine;
using System.Collections;

public class ScaleAndMove : MonoBehaviour
{
    [Header("Animasyon Süresi (sn)")]
    [SerializeField] private float duration = 1f;
    [Header("Geri Hareketten Önce Bekle (sn)")]
    [SerializeField] private float waitBeforeReverse = 1.5f;

    // Başlangıç ve bitiş değerleri
    private Vector3 _startScale;
    private Vector3 _endScale = Vector3.one;
    private Vector3 _startPos;
    private Vector3 _endPos;
    public int startPos = 150;
    public int endPos = 0;

    private void Awake()
    {
        // Scale: 1.5x başla → 1x bitir
        _startScale = Vector3.one * 1.5f;
        transform.localScale = _startScale;

        // Pozisyon: Y = startPos → endPos (X,Z sabit)
        var p = transform.localPosition;
        _startPos = new Vector3(p.x, startPos, p.z);
        _endPos   = new Vector3(p.x, endPos,   p.z);
        transform.localPosition = _startPos;
    }

    private void Start()
    {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        // --- İLERİ ANİMASYON ---
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));

            transform.localScale    = Vector3.Lerp(_startScale, _endScale, t);
            transform.localPosition = Vector3.Lerp(_startPos,   _endPos,   t);

            yield return null;
        }
        // kesinleştir
        transform.localScale    = _endScale;
        transform.localPosition = _endPos;

        // --- BEKLEME ---
        yield return new WaitForSeconds(waitBeforeReverse);

        // --- GERİ ANİMASYON ---
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));

            // ters yönde Lerp: end→start
            transform.localScale    = Vector3.Lerp(_endScale,    _startScale, t);
            transform.localPosition = Vector3.Lerp(_endPos,      _startPos,   t);

            yield return null;
        }
        // kesinleştir
        transform.localScale    = _startScale;
        transform.localPosition = _startPos;
    }
}
