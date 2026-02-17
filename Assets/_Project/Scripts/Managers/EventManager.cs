using System;
using UnityEngine;

public static class EventManager
{
    #region Game Specific Events
    
    #endregion

    #region GameManagerEvents
    public static Action OnGameStart;
    public static Action OnGameWin;
    public static Action OnGameFail;
    #endregion

    #region Loading Manager Events
    public static Action OnDataLoaded;
    public static Action OnUpdateUserChecked;
    public static Action OnRemoteDataRead;
    public static Action OnAdLogicDetermined;
    public static Action OnSessionDataManaged;
    #endregion

    #region UserInterfaceEvents
    public static Action OnJoystickInputResetted;
    #endregion
}