using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace tiplay.AudioKit
{
    public class AudioManager : MonoBehaviour
    {
        private List<CategoryObject> categoryObjects = new List<CategoryObject>();

        public void Initialize(List<SoundCategory> _categories)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                GameObject _categoryHolder = new GameObject(_categories[i].ToString());
                _categoryHolder.transform.SetParent(transform);
                categoryObjects.Add(new CategoryObject(_categories[i], _categoryHolder));
            }
        }

        public void InitializeSound(Sound _sound, SoundCategory _category)
        {
            _sound.AudioSource = categoryObjects.Where(x => x.SoundCategory == _category).FirstOrDefault().CategoryHolder.AddComponent<AudioSource>();
            _sound.AudioSource.playOnAwake = false;
            _sound.AudioSource.clip = _sound.AudioClip;
            _sound.AudioSource.loop = _sound.Islooping;
        }
    }
}