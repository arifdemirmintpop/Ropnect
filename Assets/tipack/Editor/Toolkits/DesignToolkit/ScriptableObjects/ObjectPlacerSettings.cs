using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Animations;
using tiplay.SnapTool;

namespace tiplay.Toolkit.DesignToolkit
{
    //[CreateAssetMenu(menuName = "Object Placer Settings")]
    public class ObjectPlacerSettings : ScriptableObject
    {
        public Shortcut shortcut;
        public SnapSettings snapSettings;
    }
}