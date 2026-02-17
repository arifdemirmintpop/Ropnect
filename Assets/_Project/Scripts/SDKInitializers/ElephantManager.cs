using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_EDITOR && TIPLAY_ENABLE_SDK && TIPLAY_ELEPHANT
using ElephantSDK;
#endif

public static class ElephantManager
{
    public static void EconomyTransaction(string type, int currentLevel, long amount, long finalAmount, string source)
    {
#if !UNITY_EDITOR && TIPLAY_ENABLE_SDK && TIPLAY_ELEPHANT
            return;
            Elephant.Transaction(type, currentLevel, amount, finalAmount, source);
#endif
    }

    public static void SendPlayerStartsPlayLevel(int level)
    {
#if !UNITY_EDITOR && TIPLAY_ENABLE_SDK && TIPLAY_ELEPHANT
            Elephant.LevelStarted(level);
#endif
    }

    public static void SendPlayerCompletesLevel(int level)
    {
#if !UNITY_EDITOR && TIPLAY_ENABLE_SDK && TIPLAY_ELEPHANT
            Elephant.LevelCompleted(level, Params.New().Set("time", time).Set("used_move_count", usedMoveCount));
#endif
    }

    public static void SendPlayerFailsLevel(int level)
    {
#if !UNITY_EDITOR && TIPLAY_ENABLE_SDK && TIPLAY_ELEPHANT
            Elephant.LevelFailed(level);
#endif
    }
}