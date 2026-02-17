using System;
using System.Collections.Generic;
using UnityEngine;

namespace tiplay.AudioKit
{
    [Serializable]
    public class Sound
    {
        public SoundType SoundType;
        public AudioClip AudioClip;
        [Range(0f, 1f)] public float Volume = 0.75f;
        [Range(0f, 1f)] public float VolumeVariance = 0.1f;
        [Range(0.1f, 3f)] public float Pitch = 1f;
        [Range(0f, 1f)] public float PitchVariance = 0.1f;
        public bool Islooping = false;
        [HideInInspector] public AudioSource AudioSource;
    }

    [Serializable]
    public class SoundList
    {
        public SoundCategory Category;
        public List<Sound> Sounds;
    }

    [Serializable]
    public class CategoryObject
    {
        public SoundCategory SoundCategory;
        public GameObject CategoryHolder;

        public CategoryObject(SoundCategory _category, GameObject _holder)
        {
            SoundCategory = _category;
            CategoryHolder = _holder;
        }
    }
}