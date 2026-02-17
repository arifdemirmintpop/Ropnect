using UnityEngine;
using TMPro;

public class TextWaveEffect : MonoBehaviour
{
    private TMP_Text textMeshPro;  // TextMeshPro bileşenini bağlayın
    public float waveFrequency = 6.0f;  // Dalgalanma sıklığı
    public float waveAmplitude = .1f;  // Dalgalanma genliği
    public float waveSpeed = 1.0f;  // Dalgalanma hızını ayarlayın
    public bool enableXWave = false;  // X ekseninde dalgalanma
    public bool enableZWave = false;  // Z ekseninde dalgalanma

    private float timeElapsed;  // Zamanı takip etmek için

    void Awake()
    {
        textMeshPro = GetComponent<TMP_Text>();  
    }
    void Update()
    {
        timeElapsed += Time.deltaTime * waveSpeed;

        // Metnin her bir karakteri için dalgalanma efekti oluşturun
        for (int i = 0; i < textMeshPro.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textMeshPro.textInfo.characterInfo[i];

            if (charInfo.isVisible)
            {
                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] vertices = textMeshPro.textInfo.meshInfo[materialIndex].vertices;
                Color32[] colors = textMeshPro.textInfo.meshInfo[materialIndex].colors32;  // Renkler

                // Dalgalanma efektine özel hızlandırılmış hareket için sinüs ve kosinüs kombinasyonu
                float wave = Mathf.Sin((timeElapsed + i * 0.1f) * waveFrequency) * waveAmplitude;

                // X veya Z ekseninde dalgalanma
                if (enableXWave)
                {
                    wave = Mathf.Sin((timeElapsed + i * 0.1f) * waveFrequency) * waveAmplitude;
                    vertices[vertexIndex + 0].x += wave;
                    vertices[vertexIndex + 1].x += wave;
                    vertices[vertexIndex + 2].x += wave;
                    vertices[vertexIndex + 3].x += wave;
                }
                else if (enableZWave)
                {
                    wave = Mathf.Cos((timeElapsed + i * 0.1f) * waveFrequency) * waveAmplitude;
                    vertices[vertexIndex + 0].z += wave;
                    vertices[vertexIndex + 1].z += wave;
                    vertices[vertexIndex + 2].z += wave;
                    vertices[vertexIndex + 3].z += wave;
                }
                else
                {
                    vertices[vertexIndex + 0].y += wave;
                    vertices[vertexIndex + 1].y += wave;
                    vertices[vertexIndex + 2].y += wave;
                    vertices[vertexIndex + 3].y += wave;
                }
            }
        }

        // TextMesh Pro'nun mesh'ini güncelle
        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }
}
