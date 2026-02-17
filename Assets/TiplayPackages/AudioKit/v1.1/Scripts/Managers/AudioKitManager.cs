using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lofelt.NiceVibrations;
using tiplay.DatabaseSystem;
using UnityEngine;

namespace tiplay.AudioKit
{
    public static class AudioKitManager
    {
        private static AudioKitDatabase audioKitDatabase;
        private static AudioManager audioManager;

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            if (!DatabaseHelpers.HasAudio()) return;
            LoadAudioKitDatabase();
            if (!IsAudioKitDatabaseLoaded())
            {
                Debug.LogError("Audio Database can not loaded");
                return;
            }
            InitializeAudioManager();
            InitializeSounds();
        }

        private static void LoadAudioKitDatabase()
        {
            audioKitDatabase = Resources.Load<AudioKitDatabase>("AudioKitDatabase");
        }

        private static bool IsAudioKitDatabaseLoaded()
        {
            return audioKitDatabase is null ? false : true;
        }

        private static void InitializeAudioManager()
        {
            audioManager = new GameObject("Audio Manager (Instance)").AddComponent<AudioManager>();
            List<SoundCategory> _categories = new List<SoundCategory>();
            for (int i = 0; i < audioKitDatabase.SoundCategories.Count; i++)
            {
                _categories.Add(audioKitDatabase.SoundCategories[i].Category);
            }
            audioManager.Initialize(_categories);
            GameObject.DontDestroyOnLoad(audioManager);
        }

        private static void InitializeSounds()
        {
            for (int i = 0; i < audioKitDatabase.SoundCategories.Count; i++)
            {
                for (int j = 0; j < audioKitDatabase.SoundCategories[i].Sounds.Count; j++)
                {
                    audioManager.InitializeSound(audioKitDatabase.SoundCategories[i].Sounds[j], audioKitDatabase.SoundCategories[i].Category);
                }
            }
        }

        public static void PlaySound(SoundType _soundType, SoundCategory _soundCategory)
        {
            Sound _sound = GetSound(_soundType, _soundCategory);
            if (_sound != null)
            {
                _sound.AudioSource.volume = _sound.Volume * (1f + Random.Range(-_sound.VolumeVariance / 2f, _sound.VolumeVariance / 2f));
                _sound.AudioSource.pitch = _sound.Pitch * (1f + Random.Range(-_sound.PitchVariance / 2f, _sound.PitchVariance / 2f));

                _sound.AudioSource.PlayOneShot(_sound.AudioSource.clip);
            }
        }

        public static void PlaySoundWithCustomPitch(SoundType _soundType, SoundCategory _soundCategory, float _customPitch)
        {
            Sound _sound = GetSound(_soundType, _soundCategory);
            if (_sound != null)
            {
                _sound.AudioSource.volume = _sound.Volume * (1f + Random.Range(-_sound.VolumeVariance / 2f, _sound.VolumeVariance / 2f));
                _sound.AudioSource.pitch = _customPitch * (1f + Random.Range(-_sound.PitchVariance / 2f, _sound.PitchVariance / 2f));

                _sound.AudioSource.PlayOneShot(_sound.AudioSource.clip);
            }
        }

        public static void StopSound(SoundType _soundType, SoundCategory _soundCategory)
        {
            Sound _sound = GetSound(_soundType, _soundCategory);
            if (_sound != null)
            {
                _sound.AudioSource.Stop();
            }
        }

        private static Sound GetSound(SoundType _soundType, SoundCategory _soundCategory)
        {
            if (!IsAudioKitDatabaseLoaded()) return null;
            if (!DatabaseHelpers.IsAudioEnable()) return null;
            SoundList _soundList = audioKitDatabase.SoundCategories.Where(x => x.Category == _soundCategory).FirstOrDefault();
            Sound _sound = _soundList.Sounds.Where(x => x.SoundType == _soundType).FirstOrDefault();
            if (_sound is null)
            {
                Debug.LogError("There is no sound defination as " + _soundType.ToString() + " in category; " + _soundCategory.ToString());
                return null;
            }
            else return _sound;
        }

        public static void AudioPreferenceIsChanged()
        {
            AudioListener.pause = !DatabaseHelpers.IsAudioEnable();
        }
    }
}

