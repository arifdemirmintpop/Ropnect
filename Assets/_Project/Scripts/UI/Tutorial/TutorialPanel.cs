using System;
using System.Collections;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] private TutorialType type;

    public TutorialType Type => type;

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Close();
        }
    }
}
