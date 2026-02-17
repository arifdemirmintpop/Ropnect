using System.Collections;
using System.Collections.Generic;
using tiplay.AudioKit;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    [SerializeField] private SoundCategory soundCategory = SoundCategory.UserInterface;
    [SerializeField] private SoundType soundType = SoundType.ButtonTapped;
    
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        AudioKitManager.PlaySound(soundType, soundCategory);
    }
}