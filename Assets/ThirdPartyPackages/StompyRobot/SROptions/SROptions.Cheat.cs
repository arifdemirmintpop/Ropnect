#if !DISABLE_SRDEBUGGER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SRDebugger;
using SRDebugger.Services;
using System.ComponentModel;

public partial class SROptions
{
    [Category("Level Options"), Sort(1)]
    public void LoadNextLevel()
    {
        LevelManager.LoadSRDebuggerLevel(1, false);
    }

    [Category("Level Options"), Sort(0)]
    public void LoadPreviousLevel()
    {
        LevelManager.LoadSRDebuggerLevel(-1, false);
    }

    private int _selectedLevel = 1;
    [Category("Level Options") , Sort(2)]
    [NumberRange(1,999)]
    public int LevelIndex
    {
        get => _selectedLevel;
        set => _selectedLevel = value;
    }

    [Category("Level Options"), Sort(3)]
    public void LoadSelectedLevelIndex()
    {
        LevelManager.LoadSRDebuggerLevel(_selectedLevel, true);
    }
}
#endif