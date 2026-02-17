using System.Collections;
using System.Collections.Generic;
using tiplay.AudioKit;
using UnityEngine;

public class SoundPitchManager : MonoBehaviour
{
    [System.Serializable]
    private class SoundSettings
    {
        public SoundType soundType;
        public SoundCategory soundCategory = SoundCategory.InGame;
        public float defaultPitch = 1f;
        public float pitchStep = 0.05f;
        public float resetDelay = 1.5f;
        public float minInterval = 0f;
    }

    private class SoundState
    {
        public float CurrentPitch;
        public float LastPlayTime;
        public Coroutine ResetCoroutine;
        public SoundSettings Settings;

        public SoundState(SoundSettings settings)
        {
            Settings = settings;
            CurrentPitch = settings.defaultPitch;
            LastPlayTime = -999f;
        }
    }

    [Header("Sound Configs")]
    [SerializeField] private List<SoundSettings> soundConfigs = new List<SoundSettings>();

    private Dictionary<SoundType, SoundState> soundStates;

    void Awake()
    {
        soundStates = new Dictionary<SoundType, SoundState>();
        foreach (var config in soundConfigs)
        {
            if (!soundStates.ContainsKey(config.soundType))
                soundStates[config.soundType] = new SoundState(config);
        }
    }

    public void PlaySound(SoundType soundType)
    {
        if (!soundStates.TryGetValue(soundType, out var state))
            return;

        // Prevent too frequent play
        if (Time.time - state.LastPlayTime < state.Settings.minInterval)
            return;

        state.LastPlayTime = Time.time;

        // Play sound
        AudioKitManager.PlaySoundWithCustomPitch(
            state.Settings.soundType,
            state.Settings.soundCategory,
            state.CurrentPitch
        );

        // Increase pitch
        state.CurrentPitch += state.Settings.pitchStep;

        // Restart reset coroutine
        if (state.ResetCoroutine != null)
            StopCoroutine(state.ResetCoroutine);

        state.ResetCoroutine = StartCoroutine(ResetPitchAfterDelay(state));
    }

    private IEnumerator ResetPitchAfterDelay(SoundState state)
    {
        yield return new WaitForSeconds(state.Settings.resetDelay);
        state.CurrentPitch = state.Settings.defaultPitch;
    }
}