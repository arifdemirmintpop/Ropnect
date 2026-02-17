using System.Collections;
using System.Collections.Generic;
using tiplay;
using UnityEngine;
using TMPro;

public class LevelText : MonoBehaviour
{
    void Start()
    {
        GetComponent<TMP_Text>().text = "Level " + GlobalData.Database.LevelDatabase.LevelTextValue;
    }
}
