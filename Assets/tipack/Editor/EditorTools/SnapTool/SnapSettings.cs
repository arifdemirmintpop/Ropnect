using UnityEngine;
using System;

namespace tiplay.SnapTool
{
    [Serializable]
    public class SnapSettings
    {
        public bool raySettingsExpanded;
        public Space raySpace;
        public LayerMask layerMask;
        public QueryTriggerInteraction triggerInteraction;
        //public bool addBoundsOffset;

        public bool snapSettingsExpanded;
        public Vector3 positionOffset;
        public bool keepRotation = true;
        public Vector3 rotationAngleOffset;
    }
}