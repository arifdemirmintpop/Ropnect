using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class MPBComponent : MonoBehaviour
{
    [System.Serializable]
    public class MaterialSlotSetting
    {
        public string slotName = "Material Slot";
        public bool randomizeColor = false;
        public Color baseColor = Color.white;
        [Range(0f, 1f)] public float metallic = 0f;
        [Range(0f, 1f)] public float smoothness = 0.5f;
        public Texture albedoTexture;
        public Texture normalMap;
    }

    public List<MaterialSlotSetting> slotSettings = new List<MaterialSlotSetting>();

    private Renderer rend;
    private MaterialPropertyBlock mpb;

    void Reset()
    {
        if (rend == null) rend = GetComponent<Renderer>();
        var materials = rend.sharedMaterials;

        slotSettings.Clear();
        for (int i = 0; i < materials.Length; i++)
        {
            var mat = materials[i];
            var setting = new MaterialSlotSetting();
            setting.slotName = $"Slot {i}";

            if (mat != null)
            {
                if (mat.HasProperty("_BaseColor"))
                    setting.baseColor = mat.GetColor("_BaseColor");

                if (mat.HasProperty("_Metallic"))
                    setting.metallic = mat.GetFloat("_Metallic");

                if (mat.HasProperty("_Smoothness"))
                    setting.smoothness = mat.GetFloat("_Smoothness");
                else if (mat.HasProperty("_Glossiness"))
                    setting.smoothness = mat.GetFloat("_Glossiness");

                if (mat.HasProperty("_BaseMap"))
                    setting.albedoTexture = mat.GetTexture("_BaseMap");
                else if (mat.HasProperty("_MainTex"))
                    setting.albedoTexture = mat.GetTexture("_MainTex");

                if (mat.HasProperty("_BumpMap"))
                    setting.normalMap = mat.GetTexture("_BumpMap");
            }

            slotSettings.Add(setting);
        }
    }

    void OnEnable()
    {
        ApplyMPB();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        ApplyMPB();
    }
#endif

    public void ApplyMPB()
    {
        if (rend == null) rend = GetComponent<Renderer>();
        if (mpb == null) mpb = new MaterialPropertyBlock();

        int materialCount = rend.sharedMaterials.Length;

        for (int i = 0; i < materialCount; i++)
        {
            if (i >= slotSettings.Count)
                continue;

            var setting = slotSettings[i];

            mpb.Clear();

            Color finalColor = setting.randomizeColor
                ? Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.8f, 1f)
                : setting.baseColor;

            mpb.SetColor("_BaseColor", finalColor);
            mpb.SetFloat("_Metallic", setting.metallic);
            mpb.SetFloat("_Smoothness", setting.smoothness);

            if (setting.albedoTexture != null)
                mpb.SetTexture("_BaseMap", setting.albedoTexture);
            if (setting.normalMap != null)
                mpb.SetTexture("_BumpMap", setting.normalMap);

            rend.SetPropertyBlock(mpb, i);
        }
    }

    public void RandomizeAll()
    {
        foreach (var setting in slotSettings)
        {
            setting.baseColor = Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.8f, 1f);
        }

        ApplyMPB();
    }
}
