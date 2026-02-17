using System;
using UnityEditor;

namespace tiplay.Toolkit
{
    public interface ITool
    {
        string Title { get; }
        string Shortcut { get; }
        void OnCreate();
        void OnEnable();
        void OnDisable();
        void OnDestroy();
        void OnGUI();
    }
}