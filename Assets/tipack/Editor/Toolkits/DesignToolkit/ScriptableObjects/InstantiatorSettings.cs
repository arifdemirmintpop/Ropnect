using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Animations;
using tiplay.SnapTool;

namespace tiplay.Toolkit.DesignToolkit
{
    //[CreateAssetMenu(menuName = "Instantiator Settings")]
    public class InstantiatorSettings : ScriptableObject
    {
        public List<ReplaceItem> items = new List<ReplaceItem>();
        public SnapSettings snapSettings;

        [Serializable]
        public class ReplaceItem
        {
            public GameObject prefab;
            public Shortcut shortcut;
        }
    }
}