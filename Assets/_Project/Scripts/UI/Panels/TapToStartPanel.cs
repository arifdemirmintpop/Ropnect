using UnityEngine;
using UnityEngine.EventSystems;

public class TapToStartPanel : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private bool showOnStart;

    public void OnPointerDown(PointerEventData eventData)
    {
        gameObject.SetActive(false);
        StartGame();
    }

    private void StartGame()
    {
        SDKEventManager.SendStartGame();
        EventManager.OnGameStart?.Invoke();
    }

    // private void Start()
    // {
    //     gameObject.SetActive(showOnStart);

    //     if (!showOnStart) StartGame();
    // }
}
