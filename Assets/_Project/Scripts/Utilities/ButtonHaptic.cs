using UnityEngine;
using UnityEngine.UI;
using tiplay.HapticKit;

[RequireComponent(typeof(Button))]
public class ButtonHaptic : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(PlayHaptic); 
    }

    private void PlayHaptic()
    {
        HapticKitManager.Vibrate(HapticType.ButtonTapped);
    }
}