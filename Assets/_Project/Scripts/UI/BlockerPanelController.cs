using UnityEngine;
using UnityEngine.UI;
using TMPro;
using tiplay;
using tiplay.DatabaseSystem;
using System.Collections.Generic;

public class BlockerPanelController : MonoSingleton<BlockerPanelController>
{
    private Database database;
    [SerializeField] Animator blockerAnimator;
    [SerializeField] FadePanel panel;
    [SerializeField] TMP_Text topText;
    [SerializeField] TMP_Text infoText;
    [SerializeField] Image blockerImage;
    [SerializeField] List<Sprite> blockerSprites;

    void Awake()
    {
        database = GlobalData.GetInstance().database;
    }

    public void OpenPanel(int index)
    {
        panel.gameObject.SetActive(true);
        blockerAnimator.Play(ReturnAnimationName(index));

        topText.text = $"{database.BlockerDatabase.Blockers[index].Name} \n <color=orange>Unlocked</color>";
        infoText.text = database.BlockerDatabase.Blockers[index].Description;
        blockerImage.sprite = blockerSprites[index];
    }

    public void ClosePanel()
    {
        panel.Close();
    }

    private string ReturnAnimationName(int index)
    {
        if (index == 0) return "FixedBlock";
        if (index == 1) return "Label";
        if (index == 2) return "MoveDirection";
        if (index == 3) return "Ice";
        if (index == 4) return "Chain";

        else return "FixedBlock";
    }
}
