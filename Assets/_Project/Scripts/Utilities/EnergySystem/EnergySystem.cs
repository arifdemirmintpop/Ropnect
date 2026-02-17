using UnityEngine;
using System;

public class EnergySystem : MonoBehaviour
{
    public static EnergySystem Instance;

    public int maxEnergy = 5;
    public float rechargeDuration = 1800f;

    private int currentEnergy;
    private DateTime lastEnergyTime;

    public int CurrentEnergy => currentEnergy;
    public float SecondsToNextEnergy { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            
        Instance = this;
    }

    private void Start()
    {
        LoadEnergy();
        InvokeRepeating(nameof(UpdateEnergy), 1f, 1f);
    }

    void OnApplicationPause(bool pause)
    {
        if (!pause) LoadEnergy();
    }

    void OnApplicationFocus(bool focus)
    {
        if (focus) LoadEnergy();
    }

    void LoadEnergy()
    {
        currentEnergy = PlayerPrefs.GetInt("energy", maxEnergy);
        string lastTime = PlayerPrefs.GetString("last_energy_time", "");

        if (!string.IsNullOrEmpty(lastTime))
        {
            lastEnergyTime = DateTime.Parse(lastTime);
            TimeSpan elapsed = DateTime.UtcNow - lastEnergyTime;

            int energyToAdd = Mathf.FloorToInt((float)(elapsed.TotalSeconds / rechargeDuration));
            if (energyToAdd > 0)
            {
                currentEnergy = Mathf.Min(currentEnergy + energyToAdd, maxEnergy);
                lastEnergyTime = DateTime.UtcNow.AddSeconds(-(elapsed.TotalSeconds % rechargeDuration));
            }

            SecondsToNextEnergy = rechargeDuration - (float)(elapsed.TotalSeconds % rechargeDuration);
        }
        else
        {
            lastEnergyTime = DateTime.UtcNow;
            SecondsToNextEnergy = rechargeDuration;
        }

        Save();
    }

    void UpdateEnergy()
    {
        if (currentEnergy >= maxEnergy) return;

        SecondsToNextEnergy -= 1f;

        if (SecondsToNextEnergy <= 0f)
        {
            currentEnergy++;
            lastEnergyTime = DateTime.UtcNow;
            SecondsToNextEnergy = rechargeDuration;
            Save();
        }
    }

    public bool UseEnergy()
    {
        if (currentEnergy <= 0) return false;

        currentEnergy--;
        if (currentEnergy == maxEnergy - 1)
        {
            lastEnergyTime = DateTime.UtcNow;
            SecondsToNextEnergy = rechargeDuration;
        }

        Save();
        return true;
    }

    public void AddEnergy(int amount)
    {
        currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
        Save();
    }

    void Save()
    {
        PlayerPrefs.SetInt("energy", currentEnergy);
        PlayerPrefs.SetString("last_energy_time", lastEnergyTime.ToString());
        PlayerPrefs.Save();
    }
}
