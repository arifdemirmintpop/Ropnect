using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tiplay.AudioKit
{
    //[CreateAssetMenu(menuName = "Data/Audio Database")]
    public class AudioKitDatabase : ScriptableObject
    {
        public List<SoundList> SoundCategories;
    }
}