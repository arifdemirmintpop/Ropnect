using tiplay;
using tiplay.SceneTransitionKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using tiplay.DatabaseSystem;
using System;

public static class LevelManager
{
    private static Database database => GlobalData.Database;
    private static SceneReloadIntent pendingReloadIntent = SceneReloadIntent.None;

    public static void LoadOpeningLevel(Func<bool> IsSceneReady = null)
    {
        LoadSpecificScene("MainScene", IsSceneReady);
    }

    public static void LoadNextLevel()
    {
        pendingReloadIntent = SceneReloadIntent.Win;
        LoadSameLevel();
    }

    public static void LoadSameLevel()
    {
        if (pendingReloadIntent == SceneReloadIntent.None)
            pendingReloadIntent = SceneReloadIntent.RetryOrFail;
        string activeSceneName = SceneManager.GetActiveScene().name;
        LoadSpecificScene(activeSceneName);
    }

    public static void LoadSpecificScene(string _sceneName, Func<bool> IsSceneReady = null)
    {
        AsyncOperation loading = null;
        SceneManager.sceneLoaded += OnSceneLoaded;
        TransitionManager.Play(LoadLevel, IsLevelLoaded, AllowSceneActivision, null);

        void OnSceneLoaded(Scene s, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            var db = GlobalData.Database.LevelDatabase;
            if (pendingReloadIntent == SceneReloadIntent.RetryOrFail)
            {
                //Fail related processes can be managed here.
            }
            else if (pendingReloadIntent == SceneReloadIntent.Win)
            {
                //Win related processes can be managed here.
            }
            pendingReloadIntent = SceneReloadIntent.None;
        }

        void LoadLevel()
        {
            loading = SceneManager.LoadSceneAsync(_sceneName);
            loading.allowSceneActivation = false;
        }

        bool IsLevelLoaded()
        {
            if (loading == null)
                return false;

            if (IsSceneReady == null)
            {
                if (loading.progress >= 0.9f)
                    return true;
                else
                    return false;
            }
            else
            {
                if (loading.progress >= 0.9f && IsSceneReady.Invoke())
                    return true;
                else
                    return false;
            }
        }

        void AllowSceneActivision()
        {
            loading.allowSceneActivation = true;
        }
    }

    public static int GetLevelInOrder()
    {
        LevelDatabase _levelDatabase = GlobalData.Database.LevelDatabase;
        int _levelIndex = _levelDatabase.LevelIndex;
        int _levelInOrder = _levelDatabase.LevelOrder[_levelIndex];
        return _levelInOrder;
    }

    public static void IncreaseLevel()
    {
        database.LevelDatabase.LevelIndex++;
        database.LevelDatabase.LevelTextValue++;

        if (database.LevelDatabase.LevelIndex >= database.LevelDatabase.LevelOrder.Count)
            database.LevelDatabase.LevelIndex = database.LevelDatabase.LoopStartIndex;
    }
    
    public static void LoadSRDebuggerLevel(int _levelIndex, bool _isSpecificLevel = false)
    {
        int _targetLevel = 0;
        if (!_isSpecificLevel)
            _targetLevel = Mathf.Max(0, database.LevelDatabase.LevelTextValue + _levelIndex);
        else if (_isSpecificLevel)
            _targetLevel = _levelIndex;

        int _simulatedLevelIndex = 0;
        int _simulatedLevelTextValue = 1;

        while (_simulatedLevelTextValue < _targetLevel)
        {
            _simulatedLevelIndex++;

            if (_simulatedLevelTextValue < database.LevelDatabase.LevelOrder.Count)
            {
                _simulatedLevelTextValue++;
                continue;
            }

            if (_simulatedLevelIndex >= database.LevelDatabase.LevelOrder.Count)
            {
                _simulatedLevelIndex = database.LevelDatabase.LoopStartIndex;
            }

            int safety = 0;
            while (database.LevelDatabase.Levels[_simulatedLevelIndex].IsSkippedAfterLoop)
            {
                _simulatedLevelIndex++;
                if (_simulatedLevelIndex >= database.LevelDatabase.LevelOrder.Count)
                    _simulatedLevelIndex = database.LevelDatabase.LoopStartIndex;

                safety++;
                if (safety > database.LevelDatabase.LevelOrder.Count)
                {
                    Debug.LogError("There is no valid level");
                    break;
                }
            }
            _simulatedLevelTextValue++;
        }

        database.LevelDatabase.LevelIndex = _simulatedLevelIndex;
        database.LevelDatabase.LevelTextValue = _simulatedLevelTextValue;
        SaveManager.SaveData(database);
        pendingReloadIntent = SceneReloadIntent.Win;
        LoadSameLevel();
    }
}