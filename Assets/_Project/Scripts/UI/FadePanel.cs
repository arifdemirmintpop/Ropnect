using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadePanel : MonoBehaviour
{
    [SerializeField] private GameObject panelCloseObject;
    [SerializeField] private List<CanvasGroup> canvasGroups;

    private Coroutine closeRoutine;
    private readonly WaitForSeconds waitBg = new(.10f);
    private readonly WaitForSeconds waitElement = new(0.05f);
  

    private IEnumerator OpenPanelRoutine()
    {
        canvasGroups[0].interactable = false;

        if (panelCloseObject)
            panelCloseObject.SetActive(true);

        for (int i = 0; i < canvasGroups.Count; i++)
        {
            canvasGroups[i].alpha = 0;
        }

        for (int i = 0; i < canvasGroups.Count; i++)
        {
            float temp = 0;
            int current = i;

            if (i > 0)
            {
                canvasGroups[i].transform.DOLocalMoveX(canvasGroups[i].transform.localPosition.x - 100, .1f).From();
            }

            DOTween.To(() => temp, x => temp = x, 1, .15f).SetUpdate(true).OnUpdate(() => canvasGroups[current].alpha = temp);
            WaitForSeconds wait;

            if (i > 0)
                wait = waitElement;
            else
                wait = waitBg;

            yield return wait;
        }

        yield return new WaitForSeconds(.01f);

        canvasGroups[0].interactable = true;
    }

    private IEnumerator CloseRoutine()
    {
        for (int i = canvasGroups.Count - 1; i >= 0; i--)
        {
            float temp = 1;
            int current = i;

            if (i > 0)
            {
                canvasGroups[current].transform.DOLocalMoveX(canvasGroups[current].transform.localPosition.x - 100, .14f).SetUpdate(true);
            }

            DOTween.To(() => temp, x => temp = x, 0, .15f).
                SetId("Slide").
                OnUpdate(() => canvasGroups[current].alpha = temp);

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitUntil(() => canvasGroups[0].alpha == 0);

        ResetPanel();

        closeRoutine = null;

        if (panelCloseObject)
            panelCloseObject.SetActive(false);

        gameObject.SetActive(false);
    }

    private void ResetPanel()
    {
        DOTween.Kill("Slide");

        for (int i = 0; i < canvasGroups.Count; i++)
        {
            Vector3 pos = canvasGroups[i].transform.localPosition;
            pos.x = 0;
            canvasGroups[i].transform.localPosition = pos;
        }
    }

    public void Open()
    {
        if (gameObject.activeSelf)
            return;

        gameObject.SetActive(true);
        StartCoroutine(OpenPanelRoutine());

        for (int i = 0; i < canvasGroups.Count; i++)
        {
            canvasGroups[i].interactable = true;

        }
    }

    public void Close(bool isPlayAnimation = true)
    {
        if (!isPlayAnimation)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeSelf || canvasGroups.Count == 0)
            return;

        closeRoutine ??= StartCoroutine(CloseRoutine());

        for (int i = 0; i < canvasGroups.Count; i++)
        {
            canvasGroups[i].interactable = false;

        }
    }
}
