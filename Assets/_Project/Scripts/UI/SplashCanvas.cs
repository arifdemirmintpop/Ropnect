using System;
using System.Collections;
using tiplay;
using UnityEngine;
using UnityEngine.UI;

public class SplashCanvas : MonoBehaviour
{
    [SerializeField] private Text versionText;
    
    private void OnEnable()
    {
        EventManager.OnUpdateUserChecked += OpenVersionText;
    }

    private void OnDisable()
    {
        EventManager.OnUpdateUserChecked -= OpenVersionText;
    }

    private void OpenVersionText()
    {
        string _versionText = "v ";
        if (!String.IsNullOrEmpty(GlobalData.Database.VersionDatabase.Version))
            _versionText += GlobalData.Database.VersionDatabase.Version;
        if (!String.IsNullOrEmpty(GlobalData.Database.VersionDatabase.BuildNumber))
            _versionText += " (" + GlobalData.Database.VersionDatabase.BuildNumber + ")";
        versionText.text = _versionText;
        versionText.gameObject.SetActive(true);
    }
}
