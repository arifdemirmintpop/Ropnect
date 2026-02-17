using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Animations;
using tiplay.SnapTool;

namespace tiplay.Toolkit.DesignToolkit
{
    //[CreateAssetMenu(menuName = "Ground Snapper Settings")]
    public class GroundSnapperSettings : ScriptableObject
    {
        public Shortcut shortcut;
        public bool gizmos = true;
        public SnapSettings snapSettings;
    }
}