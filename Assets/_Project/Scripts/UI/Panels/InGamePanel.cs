using UnityEngine;

public class InGamePanel : MonoBehaviour
{
   private void OnEnable()
    {
        EventManager.OnGameWin += Close;
        EventManager.OnGameFail += Close;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        EventManager.OnGameWin -= Close;
        EventManager.OnGameFail -= Close;
    }
}
