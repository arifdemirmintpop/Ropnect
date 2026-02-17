using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tiplay;

public class LoadingManager : MonoBehaviour
{
    private bool isDataLoaded;
    private bool isUpdateUserChecked;
    private bool isRemoteDataRead;
    private bool isAdLogicDetermined;
    private bool isSessionDataManaged;
    private DateTime startDateTime; 

    private void OnEnable()
    {
        EventManager.OnDataLoaded += DataLoaded;
        EventManager.OnUpdateUserChecked += UpdateUserChecked;
        EventManager.OnRemoteDataRead += RemotesLoaded;
        EventManager.OnAdLogicDetermined += AdLogicDetermined;
        EventManager.OnSessionDataManaged += SessionDataManaged;
    }

    private void OnDisable()
    {
        EventManager.OnDataLoaded -= DataLoaded;
        EventManager.OnUpdateUserChecked -= UpdateUserChecked;
        EventManager.OnRemoteDataRead -= RemotesLoaded;
        EventManager.OnAdLogicDetermined -= AdLogicDetermined;
        EventManager.OnSessionDataManaged -= SessionDataManaged;
    }

    private void Start()
    {
        startDateTime = DateTime.Now;
        SaveManager.LoadData(GlobalData.Database);
    }

    private void DataLoaded()
    {
        Debug.Log("Opening Sequence 1/5 Completed. (Data Loaded)");
        isDataLoaded = true;
        GlobalData.CheckUpdateUser();
    }

    private void UpdateUserChecked()
    {
        Debug.Log("Opening Sequence 2/5 Completed. (Updated User Control Completed)");
        isUpdateUserChecked = true;
        Invoke("ReadRemoteData", 0.15f);
        LevelManager.LoadOpeningLevel(IsSceneReady); //If you have specific named opening scene you can add as a parameter otherwise it's going to select current level.
    }

    private void ReadRemoteData()
    {
        RemoteManager.Instance.ReadRemoteData();
    }

    private void RemotesLoaded()
    {
        Debug.Log("Opening Sequence 3/5 Completed. (Remote Data Readed)");
        isRemoteDataRead = true;
        TiplayAdManager.Instance.Initialize();
    }

    private void AdLogicDetermined()
    {
        Debug.Log("Opening Sequence 4/5 Completed. (Ad Logic Determined)");
        isAdLogicDetermined = true;
        SessionManager.Instance.CreateSessionData();
    }

    private void SessionDataManaged()
    {
        Debug.Log("Opening Sequence 5/5 Completed. (Session Data Managed)");
        isSessionDataManaged = true;
    }

    private bool IsSceneReady()
    {
        if ((DateTime.Now - startDateTime).TotalSeconds < Mathf.Max(GameSettings.GetInstance().splashDuration, 2)) return false;
        if (!isDataLoaded) return false;
        if (!isRemoteDataRead) return false;
        if (!isUpdateUserChecked) return false;
        if (!isAdLogicDetermined) return false;
        if (!isSessionDataManaged) return false;
        return true;
    }
}
