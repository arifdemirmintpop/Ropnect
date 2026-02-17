using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Animations;


namespace tiplay.Toolkit.DesignToolkit
{
    //[CreateAssetMenu(menuName = "Object Replace Settings")]
    public class ObjectReplaceSettings : ScriptableObject
    {
        public List<ReplaceItem> items = new List<ReplaceItem>();

        [Serializable]
        public class ReplaceItem
        {
            public GameObject prefab;
            public Shortcut shortcut;
        }
    }
}