using System.Collections;
using TMPro;
using UnityEngine;

public class PunchRevealEffect : MonoBehaviour
{
    [Header("Character Animation Settings")]
    public float charDelay = 0.05f;           // Harfler arası gecikme
    public float punchScaleAmount = 1.5f;     // İlk punch büyüklüğü
    public float startScale = 1.3f;           // Her harfin büyük başlayacağı scale
    public float minScaleDuration = 0.3f;     // Son karakterin küçülme süresi
    public float arcHeight = 30f;             // Yay yüksekliği
    public AnimationCurve scaleEase;          // Opsiyonel easing eğrisi

    private TMP_Text tmpText;
    private TMP_TextInfo textInfo;

    private int completedCount = 0;
    private int visibleCharCount = 0;

    void Start()
    {
        tmpText = GetComponent<TMP_Text>();
        tmpText.ForceMeshUpdate();
        textInfo = tmpText.textInfo;

        // Tüm karakterleri görünmez yap
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var colors = textInfo.meshInfo[i].colors32;
            for (int j = 0; j < colors.Length; j++)
                colors[j].a = 0;
        }

        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        // Visible karakter sayısını önceden say
        for (int i = 0; i < textInfo.characterCount; i++)
            if (textInfo.characterInfo[i].isVisible)
                visibleCharCount++;

        StartCoroutine(RevealCharacters());
    }

    IEnumerator RevealCharacters()
    {
        int visibleIndex = 0;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            float totalTime = (visibleCharCount - 1) * charDelay + minScaleDuration;
            float characterDuration = totalTime - (visibleIndex * charDelay);

            StartCoroutine(AnimateCharPunchArc(i, characterDuration));
            visibleIndex++;

            yield return new WaitForSeconds(charDelay);
        }

        // Tüm karakter animasyonları tamamlanana kadar bekle
        while (completedCount < visibleCharCount)
            yield return null;

        // Tüm karakter animasyonu bittiğinde TextWaveEffect'i aktif et
        TextWaveEffect waveEffect = GetComponent<TextWaveEffect>();
        if (waveEffect != null)
            waveEffect.enabled = true;
    }

    IEnumerator AnimateCharPunchArc(int index, float scaleDuration)
    {
        TMP_CharacterInfo charInfo = textInfo.characterInfo[index];
        int matIndex = charInfo.materialReferenceIndex;
        int vertIndex = charInfo.vertexIndex;

        Vector3[] vertices = textInfo.meshInfo[matIndex].vertices;
        Color32[] colors = textInfo.meshInfo[matIndex].colors32;

        Vector3[] originalVerts = new Vector3[4];
        for (int i = 0; i < 4; i++)
            originalVerts[i] = vertices[vertIndex + i];

        Vector3 center = (originalVerts[0] + originalVerts[2]) / 2f;

        // Alpha'yı başta 255 yap
        for (int i = 0; i < 4; i++)
        {
            Color32 c = colors[vertIndex + i];
            c.a = 255;
            colors[vertIndex + i] = c;
        }

        float elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            float t = elapsed / scaleDuration;
            float easedT = scaleEase != null && scaleEase.length > 0 ? scaleEase.Evaluate(t) : t;

            float punch = Mathf.Lerp(punchScaleAmount, 1f, easedT);
            float shrink = Mathf.Lerp(startScale, 1f, easedT);
            float finalScale = punch * shrink;

            float arcOffsetY = Mathf.Sin(Mathf.PI * (1 - easedT)) * arcHeight;

            for (int i = 0; i < 4; i++)
            {
                Vector3 scaled = center + (originalVerts[i] - center) * finalScale;
                scaled.y += arcOffsetY;

                vertices[vertIndex + i] = scaled;
            }

            tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < 4; i++)
        {
            vertices[vertIndex + i] = originalVerts[i];
        }

        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);

        completedCount++;
    }
}
