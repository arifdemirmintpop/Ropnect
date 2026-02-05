using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using DG.Tweening;

public class ReelController : MonoBehaviour
{
    public int level = 1;
    public ReelColorId color = ReelColorId.Blue;
    public MeshRenderer[] levelGraphics;
    public MeshRenderer reelGraphic;

    // TextMeshPro field to show level
    public TMP_Text levelText;

    public ReelDataSO reelDataSO;

    public bool isOrder = false;

    // selection lift amount (meters)
    public float selectLift = 1f;
    // selection animation duration
    public float selectDuration = 0.25f;

    // level up animation duration
    public float levelUpDuration = 0.5f;

    Tween selectTween;
    Tween expTween;
    Vector3 originalPosition;
    bool hasOriginalPosition = false;
    bool isSelected = false;

    // runtime exp value used for level-up animation (0..1)
    float currentExp = 0f;

    public void SetData(int newLevel, ReelColorId newColor, bool newIsOrder)
    {
        isOrder = newIsOrder;
        level = newLevel;
        color = newColor;
        UpdateVisual();
    }

    [ContextMenu("UpdateVisual")]
    public void UpdateVisual()
    {
        if (levelGraphics == null || levelGraphics.Length == 0) return;
        if (reelDataSO == null) return;

        if (isOrder)
            reelGraphic.material = reelDataSO.reelDeactiveMaterial;
        else
            reelGraphic.material = reelDataSO.reelActiveMaterial;

        reelGraphic.shadowCastingMode = isOrder ? ShadowCastingMode.Off : ShadowCastingMode.On;


        // find reel data for color
        ReelData data = null;
        foreach (var d in reelDataSO.reelDatas)
        {
            if (d.reelColorId == color)
            {
                data = d; break;
            }
        }
        if (data == null) return;

        for (int i = 0; i < levelGraphics.Length; i++)
        {
            var mr = levelGraphics[i];
            if (mr == null) continue;
            if (i <= level - 1)
            {
                mr.gameObject.SetActive(true);
                mr.shadowCastingMode = isOrder ? ShadowCastingMode.Off : ShadowCastingMode.On;
                mr.sharedMaterial = isOrder ? data.deactiveMaterial : data.activeMaterial;

                if (mr.sharedMaterial != null)
                {
                    mr.sharedMaterial.SetTextureOffset("_BaseMap", new Vector2(0f, 0.5f));
                }
            }
            else
            {
                mr.sharedMaterial = isOrder ? data.deactiveMaterial : data.activeMaterial;
                mr.gameObject.SetActive(false);
            }
        }

        // update level text if assigned
        UpdateLevelText();
    }

    void UpdateLevelText()
    {
        if (levelText == null) return;
        levelText.text = $"Lv {level}";
    }

    public void UpdateExp(float exp)
    {
        if (levelGraphics == null || levelGraphics.Length == 0) return;
        int idx = Mathf.Clamp(level, 0, levelGraphics.Length - 1);
        var mr = levelGraphics[idx];
        mr.gameObject.SetActive(true);

        if (mr == null) return;
        Material mat = mr.material; // instance

        if (mat == null) return;
        float y = Mathf.Lerp(0f, 0.5f, Mathf.Clamp01(exp));
        mat.SetTextureOffset("_BaseMap", new Vector2(0f, y));
    }

    public void LevelUp()
    {
        // animate exp from 0 to 1, then increase level and refresh visuals
        expTween?.Kill();
        currentExp = 0f;
        UpdateExp(currentExp);
        expTween = DOTween.To(() => currentExp, x =>
            {
                currentExp = x; UpdateExp(currentExp);
            }, 1f, levelUpDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                expTween = null;
                // finalize level increase
                level++;
                UpdateVisual();
                // reset exp visual
                currentExp = 0f;
            });
    }

    // Select visual: lift by selectLift using DOTween
    public void Select()
    {
        if (isSelected) return;
        if (!hasOriginalPosition)
        {
            originalPosition = transform.position;
            hasOriginalPosition = true;
        }
        selectTween?.Kill();
        selectTween = transform.DOMoveY(originalPosition.y + selectLift, selectDuration).SetEase(Ease.OutCubic);
        isSelected = true;
    }

    // Deselect visual: return to original position
    public void Deselect()
    {
        if (!isSelected) return;
        if (!hasOriginalPosition)
        {
            // fallback to current position as original
            originalPosition = transform.position;
            hasOriginalPosition = true;
        }
        selectTween?.Kill();
        selectTween = transform.DOMoveY(originalPosition.y, selectDuration).SetEase(Ease.InCubic).OnComplete(() => selectTween = null);
        isSelected = false;
    }

    void OnDestroy()
    {
        selectTween?.Kill();
        expTween?.Kill();
    }
}
