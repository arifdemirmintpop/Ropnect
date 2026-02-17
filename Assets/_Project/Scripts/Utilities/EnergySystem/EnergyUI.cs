using UnityEngine;
using TMPro;
using System;

public class EnergyUI : MonoBehaviour
{
    public TMP_Text energyText;
    public TMP_Text timerText;

    void Update()
    {
        int energy = EnergySystem.Instance.CurrentEnergy;
        float seconds = EnergySystem.Instance.SecondsToNextEnergy;

        energyText.text = $"Energy: {energy}/5";

        if (energy < EnergySystem.Instance.maxEnergy)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            timerText.text = $"Refill in: {time.Minutes:D2}:{time.Seconds:D2}";
        }
        else
        {
            timerText.text = "Full";
        }

        if (Input.GetKeyDown(KeyCode.Space)) EnergySystem.Instance.UseEnergy();
    }
}
