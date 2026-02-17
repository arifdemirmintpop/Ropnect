using UnityEngine;
using System;

public class CustomEventManager : MonoSingleton<CustomEventManager>
{
    private float timer = 0f;
    private bool timerRunning = false;

    private int moveCount = 0;

    private void OnEnable()
    {
        EventManager.OnGameWin += StopTimer;
        EventManager.OnGameFail += StopTimer;
        //EventManager.OnXClick += IncreaseMoveCount;
    }

    private void OnDisable()
    {
        EventManager.OnGameWin -= StopTimer;
        EventManager.OnGameFail -= StopTimer;
        //EventManager.OnXClick -= IncreaseMoveCount;
    }

    void Start()
    {
        SDKEventManager.SendStartGame();
        EventManager.OnGameStart?.Invoke();
    }

    private void Update()
    {
        if (timerRunning)
        {
            timer += Time.deltaTime;
        }
    }

    #region Move Count
    private void IncreaseMoveCount()
    {
        moveCount++;
    }

    public int GetMoveCount()
    {
        return moveCount;
    }
    #endregion

    #region Timer
    public void StartTimer()
    {
        timerRunning = true;
    }

    private void StopTimer()
    {
        timerRunning = false;
    }

    public float GetCurrentTimer()
    {
        return timer;
    }
    #endregion

}