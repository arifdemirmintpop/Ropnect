using System;

[Serializable]
public class TransitionRequest
{
    public Action OnSceneLoadStarted;
    public Func<bool> WaitForAsyncLoading;
    public Action AllowSceneActivation;
    public Action OnSceneLoadCompleted;

    public TransitionRequest(Action _onSceneLoadStarted, Func<bool> _waitForAsyncLoading, Action _allowSceneActivation, Action _onSceneLoadCompleted)
    {
        OnSceneLoadStarted = _onSceneLoadStarted;
        WaitForAsyncLoading = _waitForAsyncLoading;
        AllowSceneActivation = _allowSceneActivation;
        OnSceneLoadCompleted = _onSceneLoadCompleted;
    }
}